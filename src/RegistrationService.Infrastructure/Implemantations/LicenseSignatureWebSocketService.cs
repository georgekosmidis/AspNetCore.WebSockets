using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RegistrationService.Core.Interfaces;
using RegistrationService.Core.Messages;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RegistrationService.Infrastructure.Implemantations
{
    public class LicenseSignatureWebSocketService : ILicenseSignatureWebSocketService
    {
        private readonly ILicenseQueueService _queueService;
        private readonly ILicenseStorageService _storageService;
        private readonly CancellationToken _appStoppingCancellationToken;
        private readonly IMapper _mapper;

        public LicenseSignatureWebSocketService(ILicenseQueueService queueService, ILicenseStorageService storageService, IMapper mapper, IApplicationLifetime appLifetime)
        {
            _queueService = queueService;
            _storageService = storageService;
            _appStoppingCancellationToken = appLifetime.ApplicationStopping;
            _mapper = mapper;
        }
        public async Task AcceptConnections(HttpContext context)
        {

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
                            _storageService.UpdateMessageReceived(licenseMessage.Id, licenseMessage.SignedLicenseKey);
                        }
                        catch
                        {
                            // todo: needs refinement
                            //something went wrong, try again later
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
    }
}
