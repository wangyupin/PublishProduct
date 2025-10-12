using POVWebDomain.Common;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using System.Threading.Tasks;

namespace HqSrv.Application.Services.EcommerceMgmt
{
    public interface IPublishGoodsApplicationService
    {
        Task<Result<GetOptionAllResponse>> GetPublishOptionsAsync(GetOptionAllRequest request);
        Task<Result<object>> SubmitProductAsync(SubmitMainRequestAll request);
        Task<Result<object>> GetSubmitModeAsync(GetSubmitModeRequest request);
        Task<Result<object>> GetSubmitDefaultValuesAsync(GetSubmitDefValRequest request);
        Task<Result<object>> GetEStoreCategoryOptionsAsync();
    }
}