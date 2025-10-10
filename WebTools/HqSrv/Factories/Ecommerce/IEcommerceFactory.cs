using HqSrv.Application.Services.EcommerceServices;
using System;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using POVWebDomain.Models.ExternalApi.Store91;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HqSrv.Factories.Ecommerce
{
    public interface IEcommerceFactory
    {
        IEcommerceService CreateEcommerceService();
        Task<object> CreateRequestDtoAdd(SubmitMainRequestAll request, StoreSetting storeSetting, GetLookupAndCommonValueResponse commonInfo);
        Task<object> CreateRequestDtoEdit(SubmitMainRequestAll request, string request1, string request2, StoreSetting storeSetting);
        Type GetResponseDtoType();
    }

    public interface IEcommerceFactoryManager
    {
        IEcommerceFactory GetFactory(string website);
    }

}
