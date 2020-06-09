using Microsoft.Extensions.DependencyInjection;
using RegistrationService.Infrastructure.Implemantations;
using RegistrationService.Infrastructure.Implementations;
using RegistrationService.Core.Interfaces;

namespace RegistrationService.Infrastructure
{
    public static class StartupSetup
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<ILicenseQueueService, LicenseQueueService>();
            services.AddSingleton<ILicenseStorageService, LicenseStorageService>();
            services.AddSingleton<ILicenseSignatureWebSocketService, LicenseSignatureWebSocketService>();

            return services;
        }


    }
}
