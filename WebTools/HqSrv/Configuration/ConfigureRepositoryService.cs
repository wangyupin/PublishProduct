using HqSrv.Repository.MainTableMgmt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using HqSrv.Repository.SettingMgmt;
using HqSrv.Repository.DashBoardMgmt;

namespace HqSrv.Configuration
{
    public static class ConfigureRepositoryService
    {
        public static IServiceCollection AddReopServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<ClientRepository>();
            services.AddScoped<SystemSettingsRepository>();
            services.AddScoped<DashboardRepository>();

            return services;
        }
    }
}
