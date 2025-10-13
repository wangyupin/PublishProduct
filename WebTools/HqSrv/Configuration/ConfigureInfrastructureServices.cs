using HqSrv.Domain.Repositories;
using HqSrv.Domain.Services;
using HqSrv.Infrastructure.Repositories;
using HqSrv.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using HqSrv.Infrastructure.ExternalServices;

namespace HqSrv.Configuration
{
    /// <summary>
    /// Infrastructure Services 依賴注入配置
    /// </summary>
    public static class ConfigureInfrastructureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // ============================================
            // Repository 註冊
            // ============================================

            // Domain Repository 介面 → Infrastructure 實作
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IPublishGoodsRepository, PublishGoodsRepository>();

            // Infrastructure Repository 介面 → Infrastructure 實作
            services.AddScoped<IPublishGoodsInfrastructureRepository, PublishGoodsRepository>();

            // ============================================
            // Domain Services 的 Infrastructure 實作
            // ============================================

            // Platform Mapping Service - 在 Infrastructure 實作但實現 Domain 介面
            services.AddScoped<IPlatformMappingService, PlatformMappingService>();

            // ============================================
            // External Services (移動到 Infrastructure)
            // ============================================

            // 91App External API Service
            services.AddScoped<Store91ExternalApiService>();

            // 其他外部服務
            // services.AddScoped<YahooExternalApiService>();
            // services.AddScoped<MomoExternalApiService>();
            // services.AddScoped<ShopeeExternalApiService>();

            // Domain Event Service
            services.AddScoped<IDomainEventService, DomainEventService>();


            return services;
        }
    }
}
