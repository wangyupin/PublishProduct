using HqSrv.Domain.Entities;
using HqSrv.Domain.Repositories;
using HqSrv.Domain.ValueObjects;
using POVWebDomain.Common;
using POVWebDomain.Models.DB.POVWeb;
using Microsoft.Data.SqlClient;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace HqSrv.Infrastructure.Repositories
{
    /// <summary>
    /// 商品資料庫存取實作 - Infrastructure 層
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly POVWebDbContextDapper _context;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public ProductRepository(
            POVWebDbContextDapper context,
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<Result<Product>> GetByParentIdAsync(string parentId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(parentId))
                    return Result<Product>.Failure(Error.Custom("INVALID_PARENT_ID", "商品父編號不能為空"));


                using var connection = new SqlConnection(_configuration.GetConnectionString("POVWebDb"));
                await connection.OpenAsync();

                // 查詢商品基本資訊
                var productSql = @"
                    SELECT TOP 1 
                        ParentID,
                        RequestParams
                    FROM ESubmitGoodsReq 
                    WHERE ParentID = @ParentId
                    ORDER BY ChangeTime DESC";

                var productData = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    productSql, new { ParentId = parentId });

                if (productData == null)
                    return Result<Product>.Failure(Error.Custom("PRODUCT_NOT_FOUND", $"找不到商品: {parentId}"));

                // 解析 JSON 資料
                var requestParams = JsonConvert.DeserializeObject<ProductJsonData>(productData.RequestParams.ToString());

                // 轉換為 Domain Entity
                var product = ConvertToProduct(parentId, requestParams);

                return Result<Product>.Success(product);
            }
            catch (Exception ex)
            {
                return Result<Product>.Failure(Error.Custom("GET_PRODUCT_ERROR", ex.Message));
            }
        }

        public async Task<Result<List<Product>>> GetByIdsAsync(List<string> parentIds)
        {
            try
            {
                if (!parentIds?.Any() == true)
                    return Result<List<Product>>.Success(new List<Product>());

                using var connection = new SqlConnection(_configuration.GetConnectionString("POVWebDb"));
                await connection.OpenAsync();

                var sql = @"
                    SELECT 
                        ParentID,
                        RequestParams,
                        ROW_NUMBER() OVER (PARTITION BY ParentID ORDER BY ChangeTime DESC) as rn
                    FROM ESubmitGoodsReq 
                    WHERE ParentID IN @ParentIds";

                var results = await connection.QueryAsync<dynamic>(sql, new { ParentIds = parentIds });

                var products = new List<Product>();

                foreach (var data in results.Where(r => r.rn == 1))
                {
                    try
                    {
                        var requestParams = JsonConvert.DeserializeObject<ProductJsonData>(data.RequestParams.ToString());
                        var product = ConvertToProduct(data.ParentID, requestParams);
                        products.Add(product);
                    }
                    catch (Exception ex)
                    {
                        // 記錄個別商品轉換錯誤，但繼續處理其他商品
                        // Logger.LogWarning($"轉換商品 {data.ParentID} 失敗: {ex.Message}");
                    }
                }

                return Result<List<Product>>.Success(products);
            }
            catch (Exception ex)
            {
                return Result<List<Product>>.Failure(Error.Custom("GET_PRODUCTS_ERROR", ex.Message));
            }
        }

        public async Task<Result<Product>> SaveAsync(Product product)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("POVWebDb"));
                await connection.OpenAsync();
                using var transaction = connection.BeginTransaction();

                try
                {
                    // 轉換為 JSON 格式
                    var jsonData = ConvertToJson(product);

                    var sql = @"
                        MERGE INTO ESubmitGoodsReq AS target
                        USING (VALUES (@ParentID, @RequestParams, GETDATE(), @ChangePerson)) AS source (ParentID, RequestParams, ChangeTime, ChangePerson)
                        ON target.ParentID = source.ParentID
                        WHEN MATCHED THEN
                            UPDATE SET RequestParams = source.RequestParams, ChangeTime = source.ChangeTime, ChangePerson = source.ChangePerson
                        WHEN NOT MATCHED THEN
                            INSERT (ParentID, RequestParams, ChangeTime, ChangePerson)
                            VALUES (source.ParentID, source.RequestParams, source.ChangeTime, source.ChangePerson);";

                    await connection.ExecuteAsync(sql, new
                    {
                        ParentID = product.ParentId,
                        RequestParams = jsonData,
                        ChangePerson = "SYSTEM" // 或從上下文取得使用者
                    }, transaction);

                    transaction.Commit();
                    return Result<Product>.Success(product);
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                return Result<Product>.Failure(Error.Custom("SAVE_PRODUCT_ERROR", ex.Message));
            }
        }

        public async Task<Result<bool>> ExistsAsync(string parentId)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("POVWebDb"));
                await connection.OpenAsync();

                var sql = "SELECT COUNT(1) FROM ESubmitGoodsReq WHERE ParentID = @ParentId";
                var count = await connection.ExecuteScalarAsync<int>(sql, new { ParentId = parentId });

                return Result<bool>.Success(count > 0);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(Error.Custom("CHECK_EXISTS_ERROR", ex.Message));
            }
        }

        public async Task<Result<List<Product>>> GetPublishedProductsAsync(string platformId)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("POVWebDb"));
                await connection.OpenAsync();

                var sql = @"
                    SELECT DISTINCT r.ParentID, r.RequestParams
                    FROM ESubmitGoodsReq r
                    INNER JOIN ESubmitGoodsRes res ON r.ParentID = res.ParentID
                    WHERE res.StoreID = @PlatformId";

                var results = await connection.QueryAsync<dynamic>(sql, new { PlatformId = platformId });

                var products = new List<Product>();

                foreach (var data in results)
                {
                    try
                    {
                        var requestParams = JsonConvert.DeserializeObject<ProductJsonData>(data.RequestParams.ToString());
                        var product = ConvertToProduct(data.ParentID, requestParams);
                        products.Add(product);
                    }
                    catch (Exception ex)
                    {
                        // 記錄錯誤但繼續處理
                        // Logger.LogWarning($"轉換已發布商品 {data.ParentID} 失敗: {ex.Message}");
                    }
                }

                return Result<List<Product>>.Success(products);
            }
            catch (Exception ex)
            {
                return Result<List<Product>>.Failure(Error.Custom("GET_PUBLISHED_PRODUCTS_ERROR", ex.Message));
            }
        }

        // ============================================
        // 私有方法 - 資料轉換
        // ============================================
        private Product ConvertToProduct(string parentId, ProductJsonData jsonData)
        {
            var product = Product.Create(
                parentId: parentId,
                title: jsonData.Title ?? "",
                price: jsonData.Price,
                cost: jsonData.Cost,
                applyType: jsonData.ApplyType ?? "一般");

            // 設定詳細資訊
            product.UpdateBasicInfo(
                title: jsonData.Title ?? "",
                description: jsonData.ProductDescription ?? "",
                moreInfo: jsonData.MoreInfo ?? "");

            product.UpdatePricing(
                suggestPrice: jsonData.SuggestPrice,
                price: jsonData.Price,
                cost: jsonData.Cost);

            if (jsonData.SellingStartDateTime.HasValue || jsonData.SellingEndDateTime.HasValue)
            {
                product.SetSellingPeriod(
                    startTime: jsonData.SellingStartDateTime,
                    endTime: jsonData.SellingEndDateTime);
            }

            product.SetDimensions(
                height: jsonData.Height,
                width: jsonData.Width,
                length: jsonData.Length,
                weight: jsonData.Weight);

            // 處理 SKU
            if (jsonData.HasSku && jsonData.SkuList?.Any() == true)
            {
                product.EnableSkuMode();

                foreach (var skuData in jsonData.SkuList)
                {
                    var sku = ProductSku.Create(
                        outerId: skuData.OuterId ?? "",
                        name: skuData.Name ?? "",
                        qty: skuData.Qty,
                        onceQty: skuData.OnceQty,
                        price: skuData.Price,
                        cost: skuData.Cost);

                    sku.UpdatePricing(skuData.SuggestPrice, skuData.Price, skuData.Cost);
                    sku.UpdateInventory(skuData.Qty, skuData.SafetyStockQty);

                    product.AddSku(sku);
                }
            }
            else if (jsonData.Qty.HasValue)
            {
                product.DisableSkuMode(
                    qty: jsonData.Qty.Value,
                    onceQty: jsonData.OnceQty ?? 1,
                    outerId: jsonData.OuterId ?? "");
            }

            return product;
        }

        private string ConvertToJson(Product product)
        {
            var jsonData = new ProductJsonData
            {
                Title = product.Title,
                ProductDescription = product.Description,
                MoreInfo = product.MoreInfo,
                Price = product.Price,
                Cost = product.Cost,
                SuggestPrice = product.SuggestPrice,
                ApplyType = product.ApplyType,
                SellingStartDateTime = product.SellingStartDateTime,
                SellingEndDateTime = product.SellingEndDateTime,
                Height = product.Height,
                Width = product.Width,
                Length = product.Length,
                Weight = product.Weight,
                TemperatureTypeDef = product.TemperatureTypeDef,
                HasSku = product.HasSku,
                Qty = product.Qty,
                OnceQty = product.OnceQty,
                OuterId = product.OuterId,
                SkuList = product.SkuList.Select(s => new SkuJsonData
                {
                    OuterId = s.OuterId,
                    Name = s.Name,
                    Qty = s.Qty,
                    OnceQty = s.OnceQty,
                    Price = s.Price,
                    Cost = s.Cost,
                    SuggestPrice = s.SuggestPrice,
                    SafetyStockQty = s.SafetyStockQty
                }).ToList()
            };

            return JsonConvert.SerializeObject(jsonData);
        }

        // ============================================
        // 資料模型 - JSON 序列化用
        // ============================================
        private class ProductJsonData
        {
            public string Title { get; set; }
            public string ProductDescription { get; set; }
            public string MoreInfo { get; set; }
            public decimal Price { get; set; }
            public decimal Cost { get; set; }
            public decimal SuggestPrice { get; set; }
            public string ApplyType { get; set; }
            public DateTime? SellingStartDateTime { get; set; }
            public DateTime? SellingEndDateTime { get; set; }
            public int Height { get; set; }
            public int Width { get; set; }
            public int Length { get; set; }
            public int Weight { get; set; }
            public string TemperatureTypeDef { get; set; }
            public bool HasSku { get; set; }
            public int? Qty { get; set; }
            public int? OnceQty { get; set; }
            public string OuterId { get; set; }
            public List<SkuJsonData> SkuList { get; set; } = new List<SkuJsonData>();
        }

        private class SkuJsonData
        {
            public string OuterId { get; set; }
            public string Name { get; set; }
            public int Qty { get; set; }
            public int OnceQty { get; set; }
            public decimal Price { get; set; }
            public decimal Cost { get; set; }
            public decimal SuggestPrice { get; set; }
            public int SafetyStockQty { get; set; }
        }
    }
}
