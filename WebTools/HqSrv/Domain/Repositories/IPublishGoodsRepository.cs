using HqSrv.Domain.Entities;
using POVWebDomain.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HqSrv.Domain.Repositories
{
    /// <summary>
    /// 商品發布資料庫抽象介面 - Domain 層定義
    /// 只包含核心業務相關的資料操作
    /// </summary>
    public interface IPublishGoodsRepository
    {
        // 商品相關查詢（Domain 層關心的）
        Task<Result<Product>> GetProductForEditAsync(string parentId, string platformId);
        Task<Result<List<Product>>> GetProductsByStatusAsync(string status);
        Task<Result<bool>> SaveProductPublishHistoryAsync(Product product, string platformId, object publishData);

        // 平台設定相關（業務邏輯相關）
        Task<Result<object>> GetPlatformConfigurationAsync(string platformId);
        Task<Result<List<object>>> GetSupportedPlatformsAsync();
    }
}
