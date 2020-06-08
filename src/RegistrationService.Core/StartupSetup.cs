using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RegistrationService.Core.Middlewares;
using RegistrationService.Core.Settings;
using System;

namespace RegistrationService.Core
{
    public static class StartupSetup
    {
        public static IApplicationBuilder AddLicenseSignatureWebSocket(this IApplicationBuilder app, IConfiguration configuration)
        {
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };

            //passing the IConfiguration doesn't feel right...
            var settings = new LicenseSignatureWebSocketSettings();
            configuration.GetSection(nameof(LicenseSignatureWebSocketSettings)).Bind(settings);

            foreach (var origin in settings.AllowedOrigins)
            {
                webSocketOptions.AllowedOrigins.Add(origin);
            }

            app.UseWebSockets(webSocketOptions);

            app.UseMiddleware<LicenseSignatureGeneratorMiddleware>(settings.Path);

            return app;
        }


    }
}
