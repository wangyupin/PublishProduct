using HqSrv.Domain.Entities;
using POVWebDomain.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HqSrv.Domain.Services
{
    /// <summary>
    /// 平台對應服務 - 處理不同平台間的資料對應
    /// </summary>
    public interface IPlatformMappingService
    {
        /// <summary>
        /// 將商品對應到指定平台格式
        /// </summary>
        Task<Result<object>> MapProductToPlatformAsync(Product product, string platformId, object platformSettings);

        /// <summary>
        /// 取得平台特定的類別對應
        /// </summary>
        Task<Result<object>> GetPlatformCategoryMappingAsync(string platformId, string categoryCode);

        /// <summary>
        /// 對應商品屬性到平台欄位
        /// </summary>
        Task<Result<Dictionary<string, object>>> MapProductAttributesAsync(
            Product product,
            string platformId,
            List<object> platformAttributes);
    }
}