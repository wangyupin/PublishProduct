using HqSrv.Repository.MainTableMgmt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using HqSrv.Repository.SettingMgmt;
using HqSrv.Repository.DashBoardMgmt;
using HqSrv.Repository.EcommerceMgmt;

using HqSrv.Infrastructure.Repositories;
using HqSrv.Domain.Repositories;

namespace HqSrv.Configuration
{
    public static class ConfigureRepositoryService
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services, IConfiguration config)
        {
 
            // 這些是還沒重構的 Repository，保持現有註冊
            services.AddScoped<ClientRepository>();
            services.AddScoped<SystemSettingsRepository>();
            services.AddScoped<DashboardRepository>();
            services.AddScoped<EcommerceStoreRepository>();

            return services;
        }
    }
}
