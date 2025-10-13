using HqSrv.Application.Services.EcommerceServices;
using System;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using POVWebDomain.Models.ExternalApi.OfficialWebsite;
using Newtonsoft.Json;
using System.Collections.Generic;
using HqSrv.Infrastructure.ExternalServices;
using System.Linq;
using System.Threading.Tasks;

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

            return new SubmitGoodsRequest
            {
                MainRequest = new AddProductRequest
                {
                    StoreNumber = int.Parse(storeSetting.PlatformID),
                    CategoryID = 1,
                    ProductName = basicInfo.Title,
                    IsClosed = false,
                    SellingStartDT = basicInfo.SellingStartDateTime,
                    SellingEndDT = basicInfo.SellingEndDateTime,
                    SellingEndSetting = 1, // 預設一年
                    IsProductOption = basicInfo.HasSku,
                    GoodID = basicInfo.OuterId,
                    SuggestPrice = basicInfo.SuggestPrice,
                    Price = basicInfo.Price,
                    Cost = basicInfo.Cost,
                    SafetyInventoryQty = basicInfo.SafetyStockQty,
                    InventoryQty = basicInfo.Qty ?? 0,
                    LimitedQty = basicInfo.OnceQty ?? 1,
                    //DeliveryDateType = basicInfo.ApplyType switch
                    //{
                    //    "一般" => 1, // 指定出貨日
                    //    "預購(指定出貨日)" => 4, // 預設等待天數
                    //    "預購(指定工作天)" => 5, // 預設備貨天數
                    //    "訂製" => 2, // 可配送區間
                    //    "客約" => 3, // 不提供配送
                    //    _ => 1
                    //},
                    //SpecifiedDeliveryDate = basicInfo.ExpectShippingDate,
                    //WaitingDays = basicInfo.ShippingPrepareDay,
                    //IsEnbaleDeliveryDate = basicInfo.IsEnableBookingPickupDate,
                    //PrepareDays = basicInfo.PrepareDays,
                    //AvailableDeliveryDays = basicInfo.AvailablePickupDays,
                    //AvailableDeliveryStartDate = basicInfo.AvailablePickupStartDateTime,
                    //AvailableDeliveryEndDate = basicInfo.AvailablePickupEndDateTime,
                    //TemperatureTypeID = basicInfo.TemperatureTypeDef switch
                    //{
                    //    "Normal" => 1, // 常溫
                    //    "Refrigerator" => 2, // 冷藏
                    //    "Freezer" => 5, // 冷凍
                    //    _ => 1
                    //}, // 預設常溫
                    //Length = basicInfo.Length,
                    //Width = basicInfo.WIdth,
                    //Height = basicInfo.Height,
                    //Weight = basicInfo.Weight,
                    //WebPageTitle = basicInfo.SEOTitle,
                    //WebPageDesc = basicInfo.SEODescription,
                    //WebPageKeywords = basicInfo.SEOKeywords,
                    //IsShowProduct = 1, //加前端欄位
                    //IsShowSold = (commonInfo.IsShowSoldQty_91 ?? false) ? 1: 0,
                    //IsShowInventory = (commonInfo.IsShowStockQty_91 ?? false) ? 1 : 0,
                    //IsRestricted = (commonInfo.IsRestricted_91 ?? false) ? 1 : 0,
                    //StockoutShow = basicInfo.SoldOutActionType switch
                    //{
                    //    "OutOfStock" => 1, // 已售完
                    //    "NoRestock" => 3, // 售完不補貨
                    //    "Restock" => 2, // 售完補貨中
                    //    "BackInStockAlert" => 4, //貨到通知
                    //    _ => 1
                    //},
                    //IsReturnable = basicInfo.IsReturnable ? 1 : 0,
                    //ProductFeatures = basicInfo.ProductDescription ?? "",
                    //ProductDetail = basicInfo.MoreInfo ?? "",
                    //Brand = new int[] { int.Parse(commonInfo.BrandID) },
                    //PictureCount = request.MainImage.Count,
                    //PayType = basicInfo.PayTypes.ToArray(),
                    //ShippingType = basicInfo.ShipType_91app.Select(t=> (int) t).ToArray(),
                    //ProductSpecification = basicInfo.Specifications.ConvertToOfficial(new List<ProductSpecification>()),
                    //IsProductOption1 = useOption1,
                    //IsProductOption2 = useOption2,
                    //ProductOption1 = useOption1 ? new ProductOptionType
                    //{
                    //    Type = 1,
                    //    Name = hasColDetail1 ? "尺寸" : "顏色",
                    //    Data = hasColDetail1 ?
                    //        basicInfo.SkuList.Select(s => s.ColDetail1?.Label).Where(v => !string.IsNullOrEmpty(v)).Distinct().ToArray() :
                    //        basicInfo.SkuList.Select(s => s.ColDetail2?.Label).Where(v => !string.IsNullOrEmpty(v)).Distinct().ToArray()
                    //} : null,
                    //ProductOption2 = useOption2 ? new ProductOptionType
                    //{
                    //    Type = 2,
                    //    Name = "顏色",
                    //    Data = basicInfo.SkuList.Select(s => s.ColDetail2?.Label).Where(v => !string.IsNullOrEmpty(v)).Distinct().ToArray()
                    //} : null,
                    //ProductOptionList = productOptions
                }
            };
        }

        public async Task<object> CreateRequestDtoEdit(SubmitMainRequestAll request, string originalBasicInfo, string platformResponse, StoreSetting storeSetting)
        {
            // 當前要編輯的基本資料
            POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.SubmitMainRequest basicInfo =
                JsonConvert.DeserializeObject<POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.SubmitMainRequest>(request.BasicInfo);

            // 原始基本資料
            POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.SubmitMainRequest historyRequest =
                JsonConvert.DeserializeObject<POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.SubmitMainRequest>(originalBasicInfo);

            // 平台回應資料
            AddProductResponse historyResponse = JsonConvert.DeserializeObject<AddProductResponse>(platformResponse);

            bool hasColDetail1 = basicInfo.HasSku && basicInfo.SkuList?.Count > 0 && !string.IsNullOrEmpty(basicInfo.SkuList[0].ColDetail1?.Value);
            bool hasColDetail2 = basicInfo.HasSku && basicInfo.SkuList?.Count > 0 && !string.IsNullOrEmpty(basicInfo.SkuList[0].ColDetail2?.Value);

            // 決定選項順序：如果兩個都有，尺寸在前，顏色在後
            bool useOption1 = hasColDetail1 || hasColDetail2;
            bool useOption2 = hasColDetail1 && hasColDetail2;

            SubmitGoodsEditRequest editRequest = new SubmitGoodsEditRequest
            {
                MainRequest = new UpdateProductRequest
                {
                    StoreNumber = int.Parse(storeSetting.PlatformID),
                    ProductID = historyResponse.ProductID,
                    CategoryID = 1,
                    ProductName = basicInfo.Title,
                    IsClosed = false,
                    SellingStartDT = basicInfo.SellingStartDateTime,
                    SellingEndDT = basicInfo.SellingEndDateTime,
                    SellingEndSetting = 1,
                    IsProductOption = basicInfo.HasSku,
                    GoodID = basicInfo.OuterId,
                    SuggestPrice = basicInfo.SuggestPrice,
                    Price = basicInfo.Price,
                    Cost = basicInfo.Cost,
                    SafetyInventoryQty = basicInfo.SafetyStockQty,
                    InventoryQty = basicInfo.Qty ?? 0,
                    LimitedQty = basicInfo.OnceQty ?? 1,
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
                    IsShowProduct = 1,
                    StockoutShow = basicInfo.SoldOutActionType switch
                    {
                        "OutOfStock" => 1,
                        "NoRestock" => 3,
                        "Restock" => 2,
                        "BackInStockAlert" => 4,
                        _ => 1
                    },
                    IsReturnable = basicInfo.IsReturnable ? 1 : 0,
                    ProductFeatures = basicInfo.ProductDescription ?? "",
                    ProductDetail = basicInfo.MoreInfo ?? "",
                    PictureCount = request.MainImage.Count,
                    PayType = basicInfo.PayTypes.ToArray(),
                    ShippingType = basicInfo.ShipType_91app.Select(t => (int)t).ToArray(),
                    ProductSpecification = basicInfo.Specifications.ConvertToOfficial(new List<ProductSpecification>()),
                    IsProductOption1 = useOption1,
                    IsProductOption2 = useOption2,
                    ProductOption1 = useOption1 ? new ProductOptionType
                    {
                        Type = 1,
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
                    ProductDescription = basicInfo.ProductDescription
                }
            };

            // 處理商品選項更新
            if (basicInfo.SkuList?.Count > 0)
            {
                List<ProductOptionUpdate> updateOptions = new List<ProductOptionUpdate>();

                foreach (SkuItem sku in basicInfo.SkuList)
                {
                    SkuResult historySkuResult = historyResponse.SkuList?.FirstOrDefault(h => h.GoodID == sku.OuterId);

                    updateOptions.Add(new ProductOptionUpdate
                    {
                        SkuID = historySkuResult?.SkuID,
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

                editRequest.UpdateProductOptionRequest = new UpdateProductOptionRequest
                {
                    StoreNumber = int.Parse(storeSetting.PlatformID),
                    ProductID = historyResponse.ProductID,
                    ProductOptionList = updateOptions
                };
            }

            return editRequest;
        }

        public Type GetResponseDtoType() => typeof(AddProductResponse);
    }
}