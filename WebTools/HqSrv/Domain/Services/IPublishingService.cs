using HqSrv.Domain.Entities;
using POVWebDomain.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HqSrv.Domain.Services
{
    /// <summary>
    /// 商品發布服務 - 核心發布邏輯
    /// </summary>
    public interface IPublishingService
    {
        /// <summary>
        /// 檢查商品是否可以發布
        /// </summary>
        Task<Result<bool>> CanPublishAsync(Product product, List<string> targetPlatforms);

        /// <summary>
        /// 準備發布資料
        /// </summary>
        Task<Result<object>> PreparePublishDataAsync(Product product, string platformId, object platformConfig);

        /// <summary>
        /// 計算發布優先順序
        /// </summary>
        Result<List<string>> CalculatePublishOrder(List<string> platforms);

        /// <summary>
        /// 驗證發布前置條件
        /// </summary>
        Task<Result<bool>> ValidatePrerequisitesAsync(Product product, string platformId);
    }
}