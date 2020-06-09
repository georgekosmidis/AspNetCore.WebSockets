using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Builder;
using RegistrationService.Core.Settings;
using RegistrationService.Web.Middlewares;
using Microsoft.Extensions.Configuration;

namespace RegistrationService.Web.Extensions
{
    public static class StartupWebSockerExtensions
    {
        public static IApplicationBuilder AddLicenseSignatureWebSocketService(this IApplicationBuilder app, IConfiguration configuration)
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
