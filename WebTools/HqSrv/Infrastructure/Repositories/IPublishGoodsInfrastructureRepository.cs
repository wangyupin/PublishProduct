using Microsoft.AspNetCore.Http;
using POVWebDomain.Common;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HqSrv.Infrastructure.Repositories
{
    /// <summary>
    /// 商品發布基礎設施資料庫介面 - Infrastructure 層定義
    /// 包含所有具體的資料存取操作
    /// </summary>
    public interface IPublishGoodsInfrastructureRepository
    {
        // DTO 相關的查詢方法
        Task<Result<object>> GetSubmitModeAsync(GetSubmitModeRequest request);
        Task<Result<object>> GetSubmitDefValAsync(GetSubmitDefValRequest request);
        Task<Result<object>> GetEStoreCatOptionsAsync();

        // 不返回 Result 的查詢方法（保持原有 API 兼容性）
        Task<GetSubmitModeReponse> GetSubmitResByStoreAsync(string parentID, string platformID);
        Task<GetLookupAndCommonValueResponse> GetLookupAndCommonValueAsync(string parentID, string storeID);
        Task<GetEcIndexReturn> MergeEcAttributesAsync(List<string> platforms, string categoryCode = null);

        // 儲存方法
        Task<Result<object>> SaveSubmitGoodsReqAsync(SubmitMainRequestAll request);
        Task<Result<object>> SaveSubmitGoodsResAsync(SubmitMainRequestAll request, object requestDto, SubmitMainResponseAll response, StoreSetting store);
        Task<Result<object>> SavePictureAsync(IFormFile file, string baseName, string type, int? index = -1);

        // 處理方法
        Task HandleImageAsync(SubmitMainRequestAll request);
        Task<MoreInfoResult> ProcessMoreInfoAsync(string originalHtml, string baseName, string origin);
        Task<string> GetOriginalRequestParamsAsync(string parentID);
        Task<Result<object>> DeleteSubmitResByStoreAsync(string parentID, string platformID);
    }
}