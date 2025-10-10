
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HqSrv.Configuration
{
    public static class ConfigureBizServices
    {
        public static IServiceCollection AddBizServices(this IServiceCollection services, IConfiguration config)
        {
            //services.AddScoped<SaleServices>();
            return services;
        }
    }
}
