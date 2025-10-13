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
    /// 商品發布服務實作
    /// </summary>
    public class PublishingService : IPublishingService
    {
        private readonly IProductValidationService _validationService;

        public PublishingService(IProductValidationService validationService)
        {
            _validationService = validationService;
        }

        public async Task<Result<bool>> CanPublishAsync(Product product, List<string> targetPlatforms)
        {
            try
            {
                // 基本可發布性檢查
                if (!product.CanPublish())
                {
                    return Result<bool>.Failure(Error.Custom("CANNOT_PUBLISH",
                        "商品不符合發布基本要求"));
                }

                // 各平台驗證
                foreach (var platformId in targetPlatforms)
                {
                    var validationResult = await _validationService.ValidateForPlatformAsync(product, platformId);
                    if (validationResult.IsFailure)
                    {
                        return Result<bool>.Failure(Error.Custom("PLATFORM_VALIDATION_FAILED",
                            $"平台 {platformId} 驗證失敗: {validationResult.Error.Message}"));
                    }
                }

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(Error.Custom("PUBLISH_CHECK_ERROR", ex.Message));
            }
        }

        public async Task<Result<object>> PreparePublishDataAsync(Product product, string platformId, object platformConfig)
        {
            try
            {
                // 前置條件驗證
                var prerequisiteResult = await ValidatePrerequisitesAsync(product, platformId);
                if (prerequisiteResult.IsFailure)
                    return Result<object>.Failure(prerequisiteResult.Error);

                // 依據平台準備資料
                var publishData = platformId switch
                {
                    "0001" => Prepare91AppData(product, platformConfig),
                    "0002" => PrepareYahooData(product, platformConfig),
                    "0003" => PrepareMomoData(product, platformConfig),
                    "0004" => PrepareShopeeData(product, platformConfig),
                    _ => throw new ProductDomainException($"不支援的平台: {platformId}")
                };

                return Result<object>.Success(publishData);
            }
            catch (Exception ex)
            {
                return Result<object>.Failure(Error.Custom("PREPARE_DATA_ERROR", ex.Message));
            }
        }

        public Result<List<string>> CalculatePublishOrder(List<string> platforms)
        {
            // 定義平台發布優先順序 (可根據業務需求調整)
            var priorityOrder = new Dictionary<string, int>
            {
                { "0001", 1 }, // 91App - 主平台，優先發布
                { "0002", 2 }, // Yahoo
                { "0003", 3 }, // Momo
                { "0004", 4 }  // Shopee
            };

            var orderedPlatforms = platforms
                .Where(p => priorityOrder.ContainsKey(p))
                .OrderBy(p => priorityOrder[p])
                .ToList();

            return Result<List<string>>.Success(orderedPlatforms);
        }

        public async Task<Result<bool>> ValidatePrerequisitesAsync(Product product, string platformId)
        {
            var errors = new List<string>();

            // 通用前置條件
            if (string.IsNullOrWhiteSpace(product.ParentId))
                errors.Add("商品父編號不能為空");

            // 平台特定前置條件
            switch (platformId)
            {
                case "0001": // 91App
                    if (string.IsNullOrWhiteSpace(product.ApplyType))
                        errors.Add("91App 需要設定申請類型");
                    break;

                case "0003": // Momo
                    if (product.Weight <= 0)
                        errors.Add("Momo 需要設定商品重量");
                    break;
            }

            if (errors.Any())
            {
                return Result<bool>.Failure(Error.Custom("PREREQUISITES_NOT_MET",
                    string.Join("; ", errors)));
            }

            return Result<bool>.Success(true);
        }

        // ============================================
        // 平台特定資料準備方法
        // ============================================
        private object Prepare91AppData(Product product, object platformConfig)
        {
            // 91App 特定的資料準備邏輯
            return new
            {
                Title = product.Title,
                Price = product.Price,
                Cost = product.Cost,
                HasSku = product.HasSku,
                SkuList = product.SkuList.Select(s => new
                {
                    s.OuterId,
                    s.Price,
                    s.Cost,
                    s.Qty
                }).ToList(),
                ApplyType = product.ApplyType,
                TemperatureTypeDef = product.TemperatureTypeDef
            };
        }

        private object PrepareYahooData(Product product, object platformConfig)
        {
            // Yahoo 特定的資料準備邏輯
            return new
            {
                Title = product.Title?.Substring(0, Math.Min(100, product.Title.Length)), // Yahoo 限制 100 字元
                Price = product.Price,
                HasSku = product.HasSku
            };
        }

        private object PrepareMomoData(Product product, object platformConfig)
        {
            // Momo 特定的資料準備邏輯
            return new
            {
                Title = product.Title,
                Price = product.Price,
                Weight = product.Weight,
                HasSku = product.HasSku
            };
        }

        private object PrepareShopeeData(Product product, object platformConfig)
        {
            // Shopee 特定的資料準備邏輯
            return new
            {
                Title = product.Title,
                Price = product.Price,
                SkuList = product.SkuList.Take(20).ToList() // Shopee 限制 20 個 SKU
            };
        }
    }
}