using HqSrv.Domain.Entities;
using POVWebDomain.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HqSrv.Domain.Services
{
    /// <summary>
    /// 商品驗證服務 - 核心業務規則
    /// </summary>
    public interface IProductValidationService
    {
        /// <summary>
        /// 驗證商品是否可以發布到指定平台
        /// </summary>
        Task<Result<bool>> ValidateForPlatformAsync(Product product, string platformId);

        /// <summary>
        /// 驗證商品基本資訊
        /// </summary>
        Result<bool> ValidateBasicInfo(Product product);

        /// <summary>
        /// 驗證 SKU 設定
        /// </summary>
        Result<bool> ValidateSkuConfiguration(Product product);

        /// <summary>
        /// 驗證價格設定
        /// </summary>
        Result<bool> ValidatePricing(Product product);

        /// <summary>
        /// 驗證庫存設定
        /// </summary>
        Result<bool> ValidateInventory(Product product);

        /// <summary>
        /// 取得所有驗證錯誤
        /// </summary>
        Task<Result<List<string>>> GetAllValidationErrorsAsync(Product product, List<string> targetPlatforms);
    }
}