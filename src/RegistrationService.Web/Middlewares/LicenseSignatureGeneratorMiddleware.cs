using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RegistrationService.Core.Interfaces;

namespace RegistrationService.Web.Middlewares
{
    public class LicenseSignatureGeneratorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _url;
        private readonly ILogger _logger;
        private readonly ILicenseSignatureWebSocketService _licenseSignatureWebSocketService;
        public LicenseSignatureGeneratorMiddleware(RequestDelegate next,
            ILogger<LicenseSignatureGeneratorMiddleware> logger,
            ILicenseSignatureWebSocketService licenseSignatureWebSocketService,
            string url)
        {
            _next = next;
            _logger = logger;
            _licenseSignatureWebSocketService = licenseSignatureWebSocketService;
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

            // Call the service to accept connections
            await _licenseSignatureWebSocketService.AcceptConnections(context);

            // Call the next delegate/middleware in the pipeline
            await _next(context);
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
