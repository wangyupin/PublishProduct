using POVWebDomain.Common;
using System.Threading.Tasks;

namespace HqSrv.Application.Services.EcommerceServices
{
    public interface IEcommerceService
    {
        Task<Result<object>> SubmitGoodsAddAsync(object requestDto, string platformID);
        Task<Result<object>> SubmitGoodsEditAsync(object requestDto, string platformID);
        Task<Result<object>> DeleteGoodsAsync(int storeNumber, int productID, string platformID);
    }
}