using HqSrv.Domain.Entities;
using HqSrv.Domain.Exceptions;
using POVWebDomain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HqSrv.Domain.Services
{
    /// <summary>
    /// 商品驗證服務實作
    /// </summary>
    public class ProductValidationService : IProductValidationService
    {
        public async Task<Result<bool>> ValidateForPlatformAsync(Product product, string platformId)
        {
            try
            {
                var errors = new List<string>();

                // 基本驗證
                var basicResult = ValidateBasicInfo(product);
                if (basicResult.IsFailure)
                    errors.Add(basicResult.Error.Message);

                // SKU 驗證
                var skuResult = ValidateSkuConfiguration(product);
                if (skuResult.IsFailure)
                    errors.Add(skuResult.Error.Message);

                // 價格驗證
                var pricingResult = ValidatePricing(product);
                if (pricingResult.IsFailure)
                    errors.Add(pricingResult.Error.Message);

                // 庫存驗證
                var inventoryResult = ValidateInventory(product);
                if (inventoryResult.IsFailure)
                    errors.Add(inventoryResult.Error.Message);

                // 平台特定驗證
                var platformErrors = await ValidatePlatformSpecificRulesAsync(product, platformId);
                errors.AddRange(platformErrors);

                if (errors.Any())
                {
                    return Result<bool>.Failure(Error.Custom("VALIDATION_FAILED",
                        string.Join("; ", errors)));
                }

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(Error.Custom("VALIDATION_ERROR", ex.Message));
            }
        }

        public Result<bool> ValidateBasicInfo(Product product)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(product.Title))
                errors.Add("商品標題不能為空");

            if (product.Title?.Length > 255)
                errors.Add("商品標題不能超過 255 個字元");

            if (string.IsNullOrWhiteSpace(product.ParentId))
                errors.Add("商品父編號不能為空");

            if (errors.Any())
                return Result<bool>.Failure(Error.Custom("BASIC_INFO_INVALID",
                    string.Join("; ", errors)));

            return Result<bool>.Success(true);
        }

        public Result<bool> ValidateSkuConfiguration(Product product)
        {
            var errors = new List<string>();

            if (product.HasSku)
            {
                if (!product.SkuList.Any())
                    errors.Add("啟用 SKU 模式時必須至少有一個 SKU");

                // 檢查 SKU 外部編號重複
                var duplicateOuterIds = product.SkuList
                    .GroupBy(s => s.OuterId)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key);

                if (duplicateOuterIds.Any())
                    errors.Add($"SKU 外部編號重複: {string.Join(", ", duplicateOuterIds)}");

                // 檢查每個 SKU 的有效性
                foreach (var sku in product.SkuList)
                {
                    if (!sku.IsValid())
                        errors.Add($"SKU {sku.OuterId} 設定無效");
                }
            }
            else
            {
                if (!product.Qty.HasValue || product.Qty <= 0)
                    errors.Add("單一 SKU 模式時庫存數量必須大於 0");

                if (!product.OnceQty.HasValue || product.OnceQty <= 0)
                    errors.Add("單次購買數量必須大於 0");

                if (string.IsNullOrWhiteSpace(product.OuterId))
                    errors.Add("外部編號不能為空");
            }

            if (errors.Any())
                return Result<bool>.Failure(Error.Custom("SKU_CONFIG_INVALID",
                    string.Join("; ", errors)));

            return Result<bool>.Success(true);
        }

        public Result<bool> ValidatePricing(Product product)
        {
            var errors = new List<string>();

            if (!product.HasSku)
            {
                if (product.Price <= 0)
                    errors.Add("商品價格必須大於 0");

                if (product.Cost < 0)
                    errors.Add("商品成本不能為負數");

                if (product.SuggestPrice < 0)
                    errors.Add("建議售價不能為負數");

                // 成本不應高於售價 (警告，但不阻止發布)
                if (product.Cost > product.Price)
                    errors.Add("警告：成本高於售價，請確認定價策略");
            }
           

            if (product.HasSku)
            {
                foreach (var sku in product.SkuList)
                {
                    if (sku.Price <= 0)
                        errors.Add($"SKU {sku.OuterId} 價格必須大於 0");

                    if (sku.Cost < 0)
                        errors.Add($"SKU {sku.OuterId} 成本不能為負數");
                }
            }

            if (errors.Any())
                return Result<bool>.Failure(Error.Custom("PRICING_INVALID",
                    string.Join("; ", errors)));

            return Result<bool>.Success(true);
        }

        public Result<bool> ValidateInventory(Product product)
        {
            var errors = new List<string>();

            if (product.HasSku)
            {
                foreach (var sku in product.SkuList)
                {
                    if (sku.Qty < 0)
                        errors.Add($"SKU {sku.OuterId} 庫存數量不能為負數");

                    if (sku.SafetyStockQty < 0)
                        errors.Add($"SKU {sku.OuterId} 安全庫存不能為負數");
                }
            }
            else
            {
                if (product.Qty.HasValue && product.Qty < 0)
                    errors.Add("庫存數量不能為負數");
            }

            if (errors.Any())
                return Result<bool>.Failure(Error.Custom("INVENTORY_INVALID",
                    string.Join("; ", errors)));

            return Result<bool>.Success(true);
        }

        public async Task<Result<List<string>>> GetAllValidationErrorsAsync(Product product, List<string> targetPlatforms)
        {
            var allErrors = new List<string>();

            foreach (var platformId in targetPlatforms)
            {
                var result = await ValidateForPlatformAsync(product, platformId);
                if (result.IsFailure)
                {
                    allErrors.Add($"平台 {platformId}: {result.Error.Message}");
                }
            }

            return Result<List<string>>.Success(allErrors);
        }

        // ============================================
        // 平台特定驗證規則
        // ============================================
        private async Task<List<string>> ValidatePlatformSpecificRulesAsync(Product product, string platformId)
        {
            var errors = new List<string>();

            switch (platformId)
            {
                case "0001": // 91App
                    errors.AddRange(Validate91AppRules(product));
                    break;
                case "0002": // Yahoo
                    errors.AddRange(ValidateYahooRules(product));
                    break;
                case "0003": // Momo
                    errors.AddRange(ValidateMomoRules(product));
                    break;
                case "0004": // Shopee
                    errors.AddRange(ValidateShopeeRules(product));
                    break;
            }

            return errors;
        }

        private List<string> Validate91AppRules(Product product)
        {
            var errors = new List<string>();

            // 91App 特定規則
            if (string.IsNullOrWhiteSpace(product.ApplyType))
                errors.Add("91App 需要設定申請類型");

            // 檢查溫層設定
            var validTemperatureTypes = new[] { "Normal", "Frozen", "Refrigerated" };
            if (!validTemperatureTypes.Contains(product.TemperatureTypeDef))
                errors.Add("91App 溫層設定無效");

            return errors;
        }

        private List<string> ValidateYahooRules(Product product)
        {
            var errors = new List<string>();

            // Yahoo 特定規則
            if (product.Title?.Length > 100)
                errors.Add("Yahoo 商品標題不能超過 100 個字元");

            return errors;
        }

        private List<string> ValidateMomoRules(Product product)
        {
            var errors = new List<string>();

            // Momo 特定規則
            if (product.Weight <= 0)
                errors.Add("Momo 需要設定商品重量");

            return errors;
        }

        private List<string> ValidateShopeeRules(Product product)
        {
            var errors = new List<string>();

            // Shopee 特定規則
            if (product.HasSku && product.SkuList.Count > 20)
                errors.Add("Shopee SKU 數量不能超過 20 個");

            return errors;
        }
    }
}
