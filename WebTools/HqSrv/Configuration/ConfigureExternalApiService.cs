
using HqSrv.Application.Services.ExternalApiServices.Store91;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace HqSrv.Configuration
{
    public static class ConfigureExternalApiService
    {
        public static IServiceCollection AddExternalApiServices(this IServiceCollection services, IConfiguration config)
        {

            services.AddScoped<Store91ExternalApiService>();

            return services;
        }
    }
}
