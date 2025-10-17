using HqSrv.Domain.Entities;
using HqSrv.Domain.ValueObjects;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace HqSrv.Application.DTOs
{
    /// <summary>
    /// DTO 與 Domain Entity 間的對應擴展方法
    /// </summary>
    public static class ProductMappingExtensions
    {
        /// <summary>
        /// 將 SubmitMainRequestAll 轉換為 Product 實體
        /// </summary>
        public static Product ToProductEntity(this SubmitMainRequestAll request)
        {
            var basicInfo = JsonConvert.DeserializeObject<SubmitMainRequest>(request.BasicInfo);

            var product = Product.Create(
                parentId: request.ParentID,
                title: basicInfo.Title,
                price: basicInfo.Price,
                cost: basicInfo.Cost,
                applyType: basicInfo.ApplyType);

            // 設定詳細資訊
            product.UpdateBasicInfo(
                title: basicInfo.Title,
                description: basicInfo.ProductDescription,
                moreInfo: basicInfo.MoreInfo);

            product.UpdatePricing(
                suggestPrice: basicInfo.SuggestPrice,
                price: basicInfo.Price,
                cost: basicInfo.Cost);

            product.SetSellingPeriod(
                startTime: basicInfo.SellingStartDateTime,
                endTime: basicInfo.SellingEndDateTime);

            product.SetDimensions(
                height: basicInfo.Height,
                width: basicInfo.WIdth,
                length: basicInfo.Length,
                weight: basicInfo.Weight);

            // 處理 SKU
            if (basicInfo.HasSku && basicInfo.SkuList?.Any() == true)
            {

                foreach (var skuDto in basicInfo.SkuList)
                {
                    var sku = ProductSku.Create(
                        outerId: skuDto.OuterId,
                        name: skuDto.ConbineColDetail(),
                        qty: skuDto.Qty,
                        onceQty: skuDto.OnceQty,
                        price: skuDto.Price,
                        cost: skuDto.Cost);

                    sku.UpdatePricing(skuDto.SuggestPrice, skuDto.Price, skuDto.Cost);
                    sku.UpdateInventory(skuDto.Qty, skuDto.SafetyStockQty);

                    product.AddSku(sku);
                }
            }

            return product;
        }

        /// <summary>
        /// 將 Product 實體轉換為 SubmitMainRequest DTO
        /// </summary>
        public static SubmitMainRequest ToSubmitMainRequest(this Product product)
        {
            return new SubmitMainRequest
            {
                Title = product.Title,
                Price = product.Price,
                Cost = product.Cost,
                SuggestPrice = product.SuggestPrice,
                ProductDescription = product.Description,
                MoreInfo = product.MoreInfo,
                ApplyType = product.ApplyType,
                SellingStartDateTime = product.SellingStartDateTime,
                SellingEndDateTime = product.SellingEndDateTime,
                HasSku = product.HasSku,
                Qty = product.Qty,
                OnceQty = product.OnceQty,
                OuterId = product.OuterId,
                Height = product.Height,
                WIdth = product.Width,
                Length = product.Length,
                Weight = product.Weight,
                TemperatureTypeDef = product.TemperatureTypeDef,
                SkuList = product.SkuList.Select(s => new SkuItem
                {
                    OuterId = s.OuterId,
                    Price = s.Price,
                    Cost = s.Cost,
                    Qty = s.Qty,
                    OnceQty = s.OnceQty,
                    SuggestPrice = s.SuggestPrice,
                    SafetyStockQty = s.SafetyStockQty
                }).ToList()
            };
        }
    }
}