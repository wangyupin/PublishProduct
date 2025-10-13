using CityAdminDomain.Services.User;
using CityHubCore.Application.Jwt;
using CityHubCore.Infrastructure.DB;
using CityHubCore.Infrastructure.ServiceClient;
using HqSrv.Application.Services;
using HqSrv.Application.Services.ApiKey;
using HqSrv.Application.Services.EcommerceMgmt;
using HqSrv.Factories.Ecommerce;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POVWebDomain.Models.DB.POVWeb;
using POVWebDomain.Models.Enum;
using System.Text;

namespace HqSrv.Configuration {
    public static class ConfigureApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // ============================================
            // Application Services - 用例協調器
            // ============================================

            services.AddScoped<IPublishGoodsApplicationService, PublishGoodsApplicationService>();

            // ============================================
            // Factory Services
            // ============================================

            services.AddScoped<IEcommerceFactoryManager, EcommerceFactoryManager>();


            // ============================================
            // Utility Services
            // ============================================

            services.AddScoped<ApiKeyProvider>();
            services.AddScoped<HttpClientService>();

            return services;
        }
    }
}
