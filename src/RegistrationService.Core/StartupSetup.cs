using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RegistrationService.Core.Middlewares;
using System;
using System.Collections.Generic;
using System.Text;

namespace RegistrationService.Core
{
    public static class StartupSetup
    {
        public static IApplicationBuilder AddLicenseSignatureWebSocket(this IApplicationBuilder app, string url, params string[] origins)
        {
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };

            foreach (var origin in origins)
            {
                webSocketOptions.AllowedOrigins.Add(origin);
            }

            app.UseWebSockets(webSocketOptions);

            app.UseMiddleware<LicenseSignatureGeneratorMiddleware>(url);

            return app;
        }
    }
}
