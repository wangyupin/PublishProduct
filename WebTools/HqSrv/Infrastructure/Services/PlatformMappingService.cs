using HqSrv.Domain.Entities;
using HqSrv.Domain.Services;
using POVWebDomain.Common;
using POVWebDomain.Models.DB.POVWeb;
using Microsoft.Data.SqlClient;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace HqSrv.Infrastructure.Services
{
    /// <summary>
    /// 平台對應服務實作 - Infrastructure 層
    /// </summary>
    public class PlatformMappingService : IPlatformMappingService
    {
        private readonly POVWebDbContextDapper _context;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public PlatformMappingService(
            POVWebDbContextDapper context,
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<Result<object>> MapProductToPlatformAsync(Product product, string platformId, object platformSettings)
        {
            try
            {
                var mappedData = platformId switch
                {
                    "0001" => MapTo91App(product, platformSettings),
                    "0002" => MapToYahoo(product, platformSettings),
                    "0003" => MapToMomo(product, platformSettings),
                    "0004" => MapToShopee(product, platformSettings),
                    _ => throw new NotSupportedException($"不支援的平台: {platformId}")
                };

                return Result<object>.Success(mappedData);
            }
            catch (Exception ex)
            {
                return Result<object>.Failure(Error.Custom("MAPPING_ERROR", ex.Message));
            }
        }

        public async Task<Result<object>> GetPlatformCategoryMappingAsync(string platformId, string categoryCode)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("POVWebDb"));
                await connection.OpenAsync();

                var sql = @"
                    SELECT 
                        PlatformCategoryId,
                        PlatformCategoryName,
                        SystemCategoryCode
                    FROM EC_CategoryMapping 
                    WHERE PlatformType = @PlatformId 
                      AND SystemCategoryCode = @CategoryCode";

                var mapping = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    sql, new { PlatformId = platformId, CategoryCode = categoryCode });

                if (mapping == null)
                    return Result<object>.Failure(Error.Custom("CATEGORY_NOT_MAPPED",
                        $"找不到平台 {platformId} 的類別對應: {categoryCode}"));

                return Result<object>.Success(mapping);
            }
            catch (Exception ex)
            {
                return Result<object>.Failure(Error.Custom("GET_CATEGORY_MAPPING_ERROR", ex.Message));
            }
        }

        public async Task<Result<Dictionary<string, object>>> MapProductAttributesAsync(
            Product product,
            string platformId,
            List<object> platformAttributes)
        {
            try
            {
                var attributeMapping = new Dictionary<string, object>();

                // 基本屬性對應
                attributeMapping["title"] = product.Title;
                attributeMapping["price"] = product.Price;
                attributeMapping["cost"] = product.Cost;

                // 平台特定屬性對應
                switch (platformId)
                {
                    case "0001": // 91App
                        attributeMapping["applyType"] = product.ApplyType;
                        attributeMapping["temperatureTypeDef"] = product.TemperatureTypeDef;
                        break;

                    case "0003": // Momo
                        attributeMapping["weight"] = product.Weight;
                        attributeMapping["dimensions"] = $"{product.Length}x{product.Width}x{product.Height}";
                        break;
                }

                // 動態屬性對應（根據平台屬性設定）
                foreach (var attr in platformAttributes)
                {
                    // 實作動態屬性對應邏輯
                    // 這裡可以根據 attr 的結構來對應商品屬性
                }

                return Result<Dictionary<string, object>>.Success(attributeMapping);
            }
            catch (Exception ex)
            {
                return Result<Dictionary<string, object>>.Failure(Error.Custom("ATTRIBUTE_MAPPING_ERROR", ex.Message));
            }
        }

        // ============================================
        // 私有方法 - 平台特定對應
        // ============================================
        private object MapTo91App(Product product, object platformSettings)
        {
            return new
            {
                Title = product.Title,
                Price = product.Price,
                Cost = product.Cost,
                SuggestPrice = product.SuggestPrice,
                ApplyType = product.ApplyType,
                TemperatureTypeDef = product.TemperatureTypeDef,
                HasSku = product.HasSku,
                Qty = product.Qty,
                OnceQty = product.OnceQty,
                OuterId = product.OuterId,
                SkuList = product.SkuList.Select(s => new
                {
                    s.OuterId,
                    s.Name,
                    s.Price,
                    s.Cost,
                    s.Qty,
                    s.OnceQty
                }).ToList(),
                SellingStartDateTime = product.SellingStartDateTime,
                SellingEndDateTime = product.SellingEndDateTime
            };
        }

        private object MapToYahoo(Product product, object platformSettings)
        {
            return new
            {
                Title = product.Title?.Length > 100 ? product.Title.Substring(0, 100) : product.Title,
                Price = product.Price,
                HasSku = product.HasSku,
                Qty = product.Qty
            };
        }

        private object MapToMomo(Product product, object platformSettings)
        {
            return new
            {
                Title = product.Title,
                Price = product.Price,
                Weight = product.Weight,
                Dimensions = new
                {
                    Length = product.Length,
                    Width = product.Width,
                    Height = product.Height
                },
                HasSku = product.HasSku
            };
        }

        private object MapToShopee(Product product, object platformSettings)
        {
            return new
            {
                Title = product.Title,
                Price = product.Price,
                SkuList = product.SkuList.Take(20).ToList() // Shopee 限制
            };
        }
    }
}