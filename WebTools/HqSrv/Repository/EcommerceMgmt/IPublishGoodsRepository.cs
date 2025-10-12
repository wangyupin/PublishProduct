using Microsoft.AspNetCore.Http;
using POVWebDomain.Common;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HqSrv.Repository.EcommerceMgmt
{
    public interface IPublishGoodsRepository
    {
        // 查詢方法 (改用 Result Pattern)
        Task<Result<object>> GetSubmitModeAsync(GetSubmitModeRequest request);
        Task<Result<object>> GetSubmitDefValAsync(GetSubmitDefValRequest request);
        Task<Result<object>> GetEStoreCatOptionsAsync();

        // 查詢方法 (保持原樣,因為不回傳 Tuple)
        Task<GetSubmitModeReponse> GetSubmitResByStoreAsync(string parentID, string platformID);
        Task<GetLookupAndCommonValueResponse> GetLookupAndCommonValueAsync(string parentID, string storeID);
        Task<GetEcIndexReturn> MergeEcAttributesAsync(List<string> platforms, string categoryCode = null);

        // 儲存方法 (改用 Result Pattern)
        Task<Result<object>> SaveSubmitGoodsReqAsync(SubmitMainRequestAll request);
        Task<Result<object>> SaveSubmitGoodsResAsync(SubmitMainRequestAll request, object requestDto, SubmitMainResponseAll response, StoreSetting store);
        Task<Result<object>> SavePictureAsync(IFormFile file, string baseName, string type, int? index = -1);

        // 處理方法 (保持原樣,因為不回傳 Tuple)
        Task HandleImageAsync(SubmitMainRequestAll request);
        Task<MoreInfoResult> ProcessMoreInfoAsync(string originalHtml, string baseName, string origin);
    }
}