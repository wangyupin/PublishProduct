using HqSrv.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HqSrv.Configuration
{
    /// <summary>
    /// Domain Services 依賴注入配置
    /// </summary>
    public static class ConfigureDomainServices
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            // Domain Services - 業務邏輯服務
            services.AddScoped<IProductValidationService, ProductValidationService>();
            services.AddScoped<IPublishingService, PublishingService>();

            // Platform Mapping Service 在 Infrastructure 層實作，但介面在 Domain 層
            // 這裡先註冊介面，實作在 ConfigureInfrastructureServices 中註冊

            return services;
        }
    }
}