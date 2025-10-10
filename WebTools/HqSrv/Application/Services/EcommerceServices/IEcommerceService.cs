using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HqSrv.Application.Services.EcommerceServices
{
    public interface IEcommerceService
    {
        Task<(object, string)> SubmitGoodsAdd(object requestDto, string platformID);
        Task<(object, string)> SubmitGoodsEdit(object requestDto, string platformID);
    }
}
