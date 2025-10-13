using HqSrv.Infrastructure.ExternalServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Hosting;
using HqSrv.Repository.EcommerceMgmt;

namespace HqSrv.Factories.Ecommerce
{
    public class EcommerceFactoryManager : IEcommerceFactoryManager
    {
        private readonly IServiceProvider _serviceProvider;

        public EcommerceFactoryManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEcommerceFactory GetFactory(string platform)
        {
            return platform switch
            {
                "91App" => new Ecommerce91Factory(
                    _serviceProvider.GetRequiredService<Store91ExternalApiService>()),

                "OfficialWebsite" => new OfficialWebsiteFactory(
                    _serviceProvider.GetRequiredService<OfficialWebsiteExternalApiService>()),

                // 未來其他平台的 Factory
                // "Yahoo" => new EcommerceYahooFactory(
                //     _serviceProvider.GetRequiredService<YahooExternalApiService>()),

                // "Momo" => new EcommerceMomoFactory(
                //     _serviceProvider.GetRequiredService<MomoExternalApiService>()),

                // "Shopee" => new EcommerceShopeeFactory(
                //     _serviceProvider.GetRequiredService<ShopeeExternalApiService>()),

                _ => throw new ArgumentException($"不支援的平台: {platform}")
            };
        }
    }
}
