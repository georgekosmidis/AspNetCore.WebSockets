using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RegistrationService.Core.Entities;
using RegistrationService.SharedKernel.Interfaces;

namespace RegistrationService.Core.Middlewares
{
    public class LicenseSignatureGeneratorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _url;
        private readonly CancellationToken _appStoppingCancellationToken;
        private readonly IQueueService<LicenseMessage> _queueService;
        private readonly IStorageService<LicenseDataModel> _storageService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public LicenseSignatureGeneratorMiddleware(RequestDelegate next,
            IQueueService<LicenseMessage> queueService,
            IStorageService<LicenseDataModel> storageService,
            IMapper mapper,
            IApplicationLifetime appLifetime,
            ILogger<LicenseSignatureGeneratorMiddleware> logger,
            string url)
        {
            _next = next;
            _appStoppingCancellationToken = appLifetime.ApplicationStopping;
            _queueService = queueService;
            _storageService = storageService;
            _mapper = mapper;
            _logger = logger;
            _url = url;

        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _InvokeAsync(context);
            }
            catch (Exception ex)
            {
                //This just logs in the console, proper logging, including the exception details is need for production
                _logger.LogError(ex.Message);
                throw;
            }
        }

        private async Task _InvokeAsync(HttpContext context)
        {
            if (context.Request.Path != _url)
            {
                await _next.Invoke(context);
                return;
            }

            if (!context.WebSockets.IsWebSocketRequest)
            {
                await WriteErrorResponse(context.Response, HttpStatusCode.BadRequest, "Expected WebSocket Request");
                return;
            }

            if (!context.User.Identity.IsAuthenticated)
            {
#warning Unauthorized Access Allowed!
                //using var rejectionSocket = await context.WebSockets.AcceptWebSocketAsync();
                //await rejectionSocket.CloseAsync((WebSocketCloseStatus)4001, "User must authenticate", _cancellationToken);
                //return;
            }

            using (var webSocket = await context.WebSockets.AcceptWebSocketAsync())
            {

                //Gracefull close
                var requestClosing = new CancellationTokenSource();
                //Create a cancelation token that will be in the canceled state when any of the source tokens are in the canceled state.
                var aggregatedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(_appStoppingCancellationToken, requestClosing.Token).Token;

                //We need parallel loops to send and receive the messages, 
                // since chances are the second loop will be unavailable 
                // while it is waiting for ReceiveAsync to return

                //Task to run send 
                var sendTask = Task.Run(async () =>
                {
                    try
                    {
                        await SendMessagesAsync(webSocket, aggregatedCancellationToken);
                    }
                    finally
                    {
                        requestClosing.Cancel();
                    }
                });

                //Task to run receive
                var receiveTask = Task.Run(async () =>
                {
                    try
                    {
                        await ReceiveMessagesAsync(webSocket, aggregatedCancellationToken);
                    }
                    finally
                    {
                        requestClosing.Cancel();
                    }
                });

                //Never use blocking calls with WebSockets to avoid serious thread issues
                await receiveTask;
                await sendTask;
                await webSocket.CloseAsync(WebSocketCloseStatus.EndpointUnavailable, "Server is stopping...", CancellationToken.None);

            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }

        private async Task ReceiveMessagesAsync(WebSocket webSocket, CancellationToken cancellationToken)
        {
            //Loop until a cancel request is issued
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    WebSocketReceiveResult response;
                    //needs optimization, 4096 is in the options, how big are the messages?
                    var buffer = new byte[4096];
                    var message = new List<byte>();
                    do
                    {
                        //receive entire message and copy it to a list of bytes
                        response = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                        message.AddRange(new ArraySegment<byte>(buffer, 0, response.Count));
                    }
                    while (!response.EndOfMessage);

                    //If the response message is to close, then stop processing
                    if (response.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }
                    //assuming it's returning text
                    else if (response.MessageType == WebSocketMessageType.Text)
                    {

                        //Convert to our data model and update the persistence layer
                        var text = Encoding.UTF8.GetString(message.ToArray());
                        var licenseMessage = JsonConvert.DeserializeObject<LicenseMessage>(text);
                        try
                        {
                            var licenseData = _mapper.Map<LicenseDataModel>(licenseMessage);
                            licenseData.Status = SharedKernel.Messages.MessageStatus.Complete;
                            _storageService.AddOrUpdate(licenseData);
                        }
                        catch
                        {
                            //something went wrong, try again later
                            // todo: needs refinement
                            _queueService.Enqueue(licenseMessage);
                            throw;
                        }
                    }

                }
                catch (OperationCanceledException)
                {
                    // Exit normally
                }
            }
        }

        private async Task SendMessagesAsync(WebSocket webSocket, CancellationToken cancellationToken)
        {
            //Loop until a cancel request is issued
            while (!cancellationToken.IsCancellationRequested)
            {
                //try to dequeue a message 
                if (_queueService.TryDequeue(out var licenseMessage))
                {
                    try
                    {
                        //send the entire message (is it smaller than 4096?)
                        var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(licenseMessage));
                        await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        // Exit normally
                    }
                    catch
                    {
                        //something went wrong, retry later
                        _queueService.Enqueue(licenseMessage);
                        throw;
                    }
                }

            }
        }

        private static async Task WriteErrorResponse(HttpResponse response, HttpStatusCode statusCode, string message)
        {
            response.StatusCode = (int)statusCode;
            response.ContentType = "application/problem+json";

            //TODO: use ProblemDetailsFactory to produce correct problemDetails object
            var problemDetails = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = "WebSocket Error",
                Detail = message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            };
            await response.WriteAsync(JsonConvert.SerializeObject(problemDetails));
        }
    }
}
