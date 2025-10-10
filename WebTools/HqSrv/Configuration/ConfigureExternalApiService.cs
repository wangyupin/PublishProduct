
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace HqSrv.Configuration
{
    public static class ConfigureExternalApiService
    {
        public static IServiceCollection AddExternalApiServices(this IServiceCollection services, IConfiguration config)
        {

            return services;
        }
    }
}
