using HqSrv.Application.Services.EcommerceServices;
using System;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using POVWebDomain.Models.ExternalApi.OfficialWebsite;
using Newtonsoft.Json;
using System.Collections.Generic;
using HqSrv.Infrastructure.ExternalServices;
using System.Linq;
using System.Threading.Tasks;
using HqSrv.Domain.Entities;
using POVWebDomain.Models.DB.POVWeb;

namespace HqSrv.Factories.Ecommerce
{
    public class OfficialWebsiteFactory : IEcommerceFactory
    {
        private readonly OfficialWebsiteExternalApiService _websiteApi;


        public OfficialWebsiteFactory(OfficialWebsiteExternalApiService websiteApi)
        {
            _websiteApi = websiteApi;
        }

        public IEcommerceService CreateEcommerceService() => new OfficialWebsiteService(_websiteApi);

        public async Task<object> CreateRequestDtoAdd(SubmitMainRequestAll request, StoreSetting storeSetting, GetLookupAndCommonValueResponse commonInfo)
        {
            POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.SubmitMainRequest basicInfo =
                JsonConvert.DeserializeObject<POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.SubmitMainRequest>(request.BasicInfo);

            bool hasColDetail1 = basicInfo.HasSku && basicInfo.SkuList?.Count > 0 && !string.IsNullOrEmpty(basicInfo.SkuList[0].ColDetail1?.Value);
            bool hasColDetail2 = basicInfo.HasSku && basicInfo.SkuList?.Count > 0 && !string.IsNullOrEmpty(basicInfo.SkuList[0].ColDetail2?.Value);

            // 決定選項順序：如果兩個都有，尺寸在前，顏色在後
            bool useOption1 = hasColDetail1 || hasColDetail2;
            bool useOption2 = hasColDetail1 && hasColDetail2;

            List<ProductOption> productOptions = new List<ProductOption>();


            if (basicInfo.HasSku && basicInfo.SkuList?.Count > 0)
            {
                // 有選項：使用 SkuList
                foreach (SkuItem sku in basicInfo.SkuList)
                {
                    productOptions.Add(new ProductOption
                    {
                        SkuName = sku.ConbineColDetail_Official(),
                        GoodID = sku.OuterId,
                        SuggestPrice = sku.SuggestPrice,
                        Price = sku.Price,
                        Cost = sku.Cost,
                        SafetyInventoryQty = sku.SafetyStockQty,
                        InventoryQty = sku.OnceQty,
                        LimitedQty = sku.SafetyStockQty
                    });
                }
            }
            else
            {
                // 無選項：用 basicInfo 組一筆
                productOptions.Add(new ProductOption
                {
                    SkuName = basicInfo.Title, // 或空字串 ""
                    GoodID = request.ParentID,
                    SuggestPrice = basicInfo.SuggestPrice,
                    Price = basicInfo.Price,
                    Cost = basicInfo.Cost,
                    SafetyInventoryQty = basicInfo.SafetyStockQty,
                    InventoryQty = basicInfo.Qty ?? 0,
                    LimitedQty = basicInfo.OnceQty ?? 1
                });
            }

            var firstOption = productOptions.FirstOrDefault();

            return new SubmitGoodsRequest
            {
                MainRequest = new AddProductRequest
                {
                    StoreNumber = int.Parse(storeSetting.PlatformID),
                    CategoryID = basicInfo.CategoryOfficialId ?? 0,
                    ProductName = basicInfo.Title,
                    IsClosed = false,
                    SellingStartDT = basicInfo.SellingStartDateTime,
                    SellingEndDT = basicInfo.SellingEndDateTime,
                    SellingEndSetting = 1, // 預設一年
                    IsProductOption = basicInfo.HasSku,
                    GoodID = firstOption?.GoodID,
                    SuggestPrice = firstOption?.SuggestPrice ?? 0,
                    Price = firstOption?.Price ?? 0,
                    Cost = firstOption?.Cost ?? 0,
                    SafetyInventoryQty = firstOption?.SafetyInventoryQty ?? 0,
                    InventoryQty = firstOption?.InventoryQty ?? 0,
                    LimitedQty = firstOption?.LimitedQty ?? 1,
                    DeliveryDateType = basicInfo.ApplyType switch
                    {
                        "一般" => 1, // 指定出貨日
                        "預購(指定出貨日)" => 4, // 預設等待天數
                        "預購(指定工作天)" => 5, // 預設備貨天數
                        "訂製" => 2, // 可配送區間
                        "客約" => 3, // 不提供配送
                        _ => 1
                    },
                    SpecifiedDeliveryDate = basicInfo.ExpectShippingDate,
                    WaitingDays = basicInfo.ShippingPrepareDay,
                    IsEnbaleDeliveryDate = basicInfo.IsEnableBookingPickupDate,
                    PrepareDays = basicInfo.PrepareDays,
                    AvailableDeliveryDays = basicInfo.AvailablePickupDays,
                    AvailableDeliveryStartDate = basicInfo.AvailablePickupStartDateTime,
                    AvailableDeliveryEndDate = basicInfo.AvailablePickupEndDateTime,
                    TemperatureTypeID = basicInfo.TemperatureTypeDef switch
                    {
                        "Normal" => 1, // 常溫
                        "Refrigerator" => 2, // 冷藏
                        "Freezer" => 5, // 冷凍
                        _ => 1
                    }, // 預設常溫
                    Length = basicInfo.Length,
                    Width = basicInfo.WIdth,
                    Height = basicInfo.Height,
                    Weight = basicInfo.Weight,
                    WebPageTitle = basicInfo.SEOTitle,
                    WebPageDesc = basicInfo.SEODescription,
                    WebPageKeywords = basicInfo.SEOKeywords,
                    IsShowProduct = true, //加前端欄位
                    IsShowSold = commonInfo.IsShowSoldQty_91 ?? false,
                    IsShowInventory = commonInfo.IsShowStockQty_91 ?? false,
                    IsRestricted = commonInfo.IsRestricted_91 ?? false,
                    StockoutShow = basicInfo.SoldOutActionType switch
                    {
                        "OutOfStock" => 1, // 已售完
                        "NoRestock" => 3, // 售完不補貨
                        "Restock" => 2, // 售完補貨中
                        "BackInStockAlert" => 4, //貨到通知
                        _ => 1
                    },
                    IsReturnable = basicInfo.IsReturnable,
                    ProductFeatures = basicInfo.ProductDescription ?? "",
                    ProductDetail = basicInfo.MoreInfo ?? "",
                    Brand = new int[] { int.Parse(commonInfo.BrandID ?? "0") },
                    PictureCount = request.MainImage?.Count ?? 0,
                    PayType = basicInfo.PayTypes.ToArray(),
                    ShippingType = basicInfo.ShipType_91app.Select(t => (int)t).ToArray(),
                    ProductSpecification = basicInfo.Specifications.ConvertToOfficial(new List<ProductSpecification>()),
                    IsProductOption1 = useOption1,
                    IsProductOption2 = useOption2,
                    ProductOption1 = useOption1 ? new ProductOptionType
                    {
                        Type = hasColDetail1 ? 1 : 2,
                        Name = hasColDetail1 ? "尺寸" : "顏色",
                        Data = hasColDetail1 ?
                            basicInfo.SkuList.Select(s => s.ColDetail1?.Label).Where(v => !string.IsNullOrEmpty(v)).Distinct().ToArray() :
                            basicInfo.SkuList.Select(s => s.ColDetail2?.Label).Where(v => !string.IsNullOrEmpty(v)).Distinct().ToArray()
                    } : null,
                    ProductOption2 = useOption2 ? new ProductOptionType
                    {
                        Type = 2,
                        Name = "顏色",
                        Data = basicInfo.SkuList.Select(s => s.ColDetail2?.Label).Where(v => !string.IsNullOrEmpty(v)).Distinct().ToArray()
                    } : null,
                    ProductOptionList = productOptions
                },
                MainImage = request.MainImage,
                SkuImage = request.SkuImage 
            };
        }

        public async Task<object> CreateRequestDtoEdit(SubmitMainRequestAll request, string originalBasicInfo, string platformResponse, StoreSetting storeSetting, GetLookupAndCommonValueResponse commonInfo)
        {
            // 當前要編輯的基本資料
            POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.SubmitMainRequest basicInfo =
                JsonConvert.DeserializeObject<POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.SubmitMainRequest>(request.BasicInfo);

            // 原始基本資料
            POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.SubmitMainRequest historyRequest =
                JsonConvert.DeserializeObject<POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.SubmitMainRequest>(originalBasicInfo);

            // 平台回應資料
            AddProductResponse historyResponse = JsonConvert.DeserializeObject<AddProductResponse>(platformResponse);


            var productId = historyResponse.ProductID;
            var storeNumber = int.Parse(storeSetting.PlatformID);


            UpdateProductOptionRequest updateOptionsRequest = null;
            List<ProductOption> productOptions = new List<ProductOption>();

            // 判斷是否有 SKU 選項
            if (basicInfo.HasSku && basicInfo.SkuList?.Any() == true)
            {
                // 有選項：使用 SkuList
                foreach (var sku in basicInfo.SkuList)
                {
                    var existingOption = historyResponse.ProductOpton?.FirstOrDefault(t => t.GoodID == sku.OriginalOuterId);

                    productOptions.Add(new ProductOption
                    {
                        SkuID = existingOption?.SkuID,
                        GoodID = sku.OuterId,
                        SuggestPrice = sku.SuggestPrice,
                        Price = sku.Price,
                        Cost = sku.Cost,
                        SafetyInventoryQty = sku.SafetyStockQty,
                        InventoryQty = sku.Qty,
                        LimitedQty = sku.OnceQty
                    });
                }

                updateOptionsRequest = new UpdateProductOptionRequest
                {
                    StoreNumber = storeNumber,
                    ProductID = productId,
                    ProductOptionList = productOptions
                };
            }
            else
            {
                // 無選項：用 basicInfo 組一筆
                var existingOption = historyResponse.ProductOpton?.FirstOrDefault();

                productOptions.Add(new ProductOption
                {
                    SkuID = existingOption?.SkuID,
                    GoodID = basicInfo.OuterId,
                    SuggestPrice = basicInfo.SuggestPrice,
                    Price = basicInfo.Price,
                    Cost = basicInfo.Cost,
                    SafetyInventoryQty = basicInfo.SafetyStockQty,
                    InventoryQty = basicInfo.Qty ?? 0,
                    LimitedQty = basicInfo.OnceQty ?? 1
                });

                updateOptionsRequest = new UpdateProductOptionRequest
                {
                    StoreNumber = storeNumber,
                    ProductID = productId,
                    ProductOptionList = productOptions
                };
            }

            var firstOption = productOptions.FirstOrDefault();

            SubmitGoodsEditRequest editRequest = new SubmitGoodsEditRequest
            {
                MainRequest = new UpdateProductRequest
                {
                    StoreNumber = storeNumber,
                    ProductID = historyResponse.ProductID,
                    CategoryID = basicInfo.CategoryOfficialId ?? 0,
                    ProductName = basicInfo.Title,
                    IsClosed = false,
                    SellingStartDT = basicInfo.SellingStartDateTime,
                    SellingEndDT = basicInfo.SellingEndDateTime,
                    SellingEndSetting = 1,
                    IsProductOption = basicInfo.HasSku,
                    GoodID = firstOption?.GoodID,
                    SuggestPrice = firstOption?.SuggestPrice ?? 0,
                    Price = firstOption?.Price ?? 0,
                    Cost = firstOption?.Cost ?? 0,
                    SafetyInventoryQty = firstOption?.SafetyInventoryQty ?? 0,
                    InventoryQty = firstOption?.InventoryQty ?? 0,
                    LimitedQty = firstOption?.LimitedQty ?? 1,
                    DeliveryDateType = basicInfo.ApplyType switch
                    {
                        "一般" => 1,
                        "預購(指定出貨日)" => 4,
                        "預購(指定工作天)" => 5,
                        "訂製" => 2,
                        "客約" => 3,
                        _ => 1
                    },
                    SpecifiedDeliveryDate = basicInfo.ExpectShippingDate,
                    WaitingDays = basicInfo.ShippingPrepareDay,
                    IsEnbaleDeliveryDate = basicInfo.IsEnableBookingPickupDate,
                    PrepareDays = basicInfo.PrepareDays,
                    AvailableDeliveryDays = basicInfo.AvailablePickupDays,
                    AvailableDeliveryStartDate = basicInfo.AvailablePickupStartDateTime,
                    AvailableDeliveryEndDate = basicInfo.AvailablePickupEndDateTime,
                    TemperatureTypeID = basicInfo.TemperatureTypeDef switch
                    {
                        "Normal" => 1,
                        "Refrigerator" => 2,
                        "Freezer" => 5,
                        _ => 1
                    },
                    Length = basicInfo.Length,
                    Width = basicInfo.WIdth,
                    Height = basicInfo.Height,
                    Weight = basicInfo.Weight,
                    WebPageTitle = basicInfo.SEOTitle,
                    WebPageDesc = basicInfo.SEODescription,
                    WebPageKeywords = basicInfo.SEOKeywords,
                    IsShowProduct = true,
                    IsShowSold = commonInfo.IsShowSoldQty_91 ?? false,
                    IsShowInventory = commonInfo.IsShowStockQty_91 ?? false,
                    IsRestricted = commonInfo.IsRestricted_91 ?? false,
                    StockoutShow = basicInfo.SoldOutActionType switch
                    {
                        "OutOfStock" => 1,
                        "NoRestock" => 3,
                        "Restock" => 2,
                        "BackInStockAlert" => 4,
                        _ => 1
                    },
                    IsReturnable = basicInfo.IsReturnable,
                    ProductFeatures = basicInfo.ProductDescription ?? "",
                    ProductDetail = basicInfo.MoreInfo ?? "",
                    Brand = new int[] { int.Parse(commonInfo.BrandID ?? "0") },
                    PictureCount = request.MainImage?.Count ?? 0,
                    PayType = basicInfo.PayTypes.ToArray(),
                    ShippingType = basicInfo.ShipType_91app.Select(t => (int)t).ToArray(),
                    ProductSpecification = basicInfo.Specifications.ConvertToOfficial(new List<ProductSpecification>())

                },
                MainImage = request.MainImage,
                SkuImage = request.SkuImage,
                UpdateOptionsRequest = updateOptionsRequest
            };


            return editRequest;
        }

        public Type GetResponseDtoType() => typeof(AddProductResponse);
        public bool ShouldSaveEditResponse() => false;
    }
}