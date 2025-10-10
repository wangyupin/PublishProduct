using HqSrv.Application.Services.ExternalApiServices.Store91;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Hosting;
using HqSrv.Repository.EcommerceMgmt;

namespace HqSrv.Factories.Ecommerce
{
    public class EcommerceFactoryManager: IEcommerceFactoryManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly PublishGoodsRepository _PublishGoodsRepository;

        public EcommerceFactoryManager(IServiceProvider serviceProvider, IWebHostEnvironment hostEnvironment, PublishGoodsRepository publishGoodsRepository)
        {
            _serviceProvider = serviceProvider;
            _hostEnvironment = hostEnvironment;
            _PublishGoodsRepository = publishGoodsRepository;
        }
        public IEcommerceFactory GetFactory(string platform)
        {
            return platform switch
            {
                "91App" => new Ecommerce91Factory(_serviceProvider.GetRequiredService<Store91ExternalApiService>()),

                _ => throw new ArgumentException("沒有支援該店商!")
            };
        }
    }
}
