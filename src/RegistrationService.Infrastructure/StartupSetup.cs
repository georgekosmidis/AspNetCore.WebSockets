using Microsoft.Extensions.DependencyInjection;
using RegistrationService.Services;
using RegistrationService.SharedKernel.Interfaces;

namespace RegistrationService.Infrastructure
{
    public static class StartupSetup
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IQueueService<>), typeof(DemoQueueService<>));
            services.AddSingleton(typeof(IStorageService<>), typeof(DemoStorageService<>));

            return services;
        }
    }
}
