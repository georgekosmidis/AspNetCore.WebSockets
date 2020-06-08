using Microsoft.Extensions.DependencyInjection;
using System;
using AutoMapper;

namespace RegistrationService.Web.Extensions
{
    public static class StartupAutoMapperExtension
    {
        public static IServiceCollection RegisterAutoMapper(this IServiceCollection services)
        {
            //https://jimmybogard.com/automapper-usage-guidelines/
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            services.AddAutoMapper(assemblies);
            return services;
        }
    }
}
