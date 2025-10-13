using HqSrv.Application.Services.EcommerceServices;
using System;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using POVWebDomain.Models.ExternalApi.Store91;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using HqSrv.Infrastructure.ExternalServices;
using System.Linq;
using CityAdminDomain.Models.Common;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace HqSrv.Factories.Ecommerce
{
    public class Ecommerce91Factory:IEcommerceFactory
    {
        private readonly Store91ExternalApiService __91Api;
        public Ecommerce91Factory(Store91ExternalApiService _91api)
        {
            __91Api = _91api;
        }
        public IEcommerceService CreateEcommerceService() => new Ecommerce91Service(__91Api);

        public async Task<object> CreateRequestDtoAdd(SubmitMainRequestAll request, StoreSetting storeSetting, GetLookupAndCommonValueResponse commonInfo)
        {
            JObject obj = JObject.Parse(request.JsonData);

            POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.SubmitMainRequest basicInfo = JsonConvert.DeserializeObject<POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.SubmitMainRequest>(request.BasicInfo);

            List<SkuListReq> skuLists = new List<SkuListReq>();

            foreach (SkuItem sku in basicInfo.SkuList)
            {
                skuLists.Add(new SkuListReq
                {
                    Name = sku.ConbineColDetail(),
                    Qty = sku.Qty,
                    OnceQty = sku.OnceQty,
                    OuterId = sku.OuterId,
                    SafetyStockQty = sku.SafetyStockQty
                });
            }

            return new SubmitGoodsRequest
            {
                MainRequest = new POVWebDomain.Models.ExternalApi.Store91.SubmitMainRequest
                {
                    CategoryId = (int)storeSetting.CategoryId,
                    MirrorCategoryIdList = new List<int>(),
                    ShopCategoryId = (int)basicInfo.ShopCategoryId,
                    MirrorShopCategoryIdList = new List<long>(),
                    Title = basicInfo.Title,
                    SellingStartDateTime = basicInfo.SellingStartDateTime,
                    SellingEndDateTime = basicInfo.SellingEndDateTime,
                    ApplyType = basicInfo.ApplyType,
                    ExpectShippingDate = basicInfo.ExpectShippingDate,
                    ShippingPrepareDay = basicInfo.ShippingPrepareDay,
                    ShippingTypes = basicInfo.ShipType_91app,
                    PayTypes = basicInfo.PayTypes,
                    SuggestPrice = basicInfo.SuggestPrice,
                    Price = basicInfo.Price,
                    Cost = basicInfo.Cost,
                    ProductHighlight = basicInfo.ProductHighlight,
                    ProductDescription = basicInfo.ProductDescription,
                    MoreInfo = (string)obj["moreInfo"],
                    Brand = null,
                    Type = basicInfo.Type,
                    Specifications = basicInfo.Specifications.ConvertToDictionary(basicInfo.IndexList.ConvertToDictionary()),
                    HasSku = basicInfo.HasSku,
                    OnceQty = basicInfo.OnceQty,
                    Qty = basicInfo.Qty,
                    OuterId = basicInfo.OuterId,
                    SkuList = skuLists,
                    TemperatureTypeDef = basicInfo.TemperatureTypeDef,
                    Length = basicInfo.Length,
                    WIdth = basicInfo.WIdth,
                    Height = basicInfo.Height,
                    Weight = basicInfo.Weight,
                    Status = commonInfo.Status_91,
                    IsShowStockQty = commonInfo.IsShowStockQty_91,
                    TaxTypeDef = basicInfo.TaxTypeDef,
                    IsReturnable = basicInfo.IsReturnable,
                    IsEnableBookingPickupDate = basicInfo.IsEnableBookingPickupDate,
                    PrepareDays = basicInfo.PrepareDays,
                    AvailablePickupDays = basicInfo.AvailablePickupDays,
                    AvailablePickupStartDateTime = basicInfo.AvailablePickupStartDateTime,
                    AvailablePickupEndDateTime = basicInfo.AvailablePickupEndDateTime,
                    SEOTitle = basicInfo.SEOTitle,
                    SEOKeywords = basicInfo.SEOKeywords,
                    SEODescription = basicInfo.SEODescription,
                    SafetyStockQty = basicInfo.SafetyStockQty,
                    IsShowPurchaseList = commonInfo.IsShowPurchaseList_91,
                    IsShowSoldQty = commonInfo.IsShowSoldQty_91,
                    IsDesignatedReturnGoodsType = basicInfo.IsDesignatedReturnGoodsType,
                    ReturnGoodsType = basicInfo.ReturnGoodsType,
                    SoldOutActionType = commonInfo.SoldOutActionType_91,
                    IsRestricted = commonInfo.IsRestricted_91,
                    SalesModeTypeDef = basicInfo.SalesModeTypeDef,
                    PointsPayPairs = basicInfo.PointsPayPairs
                },
                MainImage = request.MainImage,
                SkuImage = request.SkuImage,
                SpecChartRequest = new UpdateSpecChartIdRequest
                {
                    SalePageSpecChartId = basicInfo.SalePageSpecChartId
                },
                UpdateSaleProductSkuRequest = basicInfo.HasSku? new UpdateSaleProductSkuRequest
                {
                    IsSkuDifferentPrice = true,
                    SkuList = basicInfo.SkuList.Select((sku, idx) =>
                        new UpdSku
                        {
                            ChangeQty =0,
                            Sort =idx+1,
                            OnceQty = sku.OnceQty,
                            OuterId = sku.OuterId,
                            IsShow=true,
                            SuggestPrice = sku.SuggestPrice,
                            Price = sku.Price,
                            Cost = sku.Cost,
                            SafetyStockQty=sku.SafetyStockQty
                        }
                    ).ToList()
                }:null,
                OperateBrandRequest = new OperateBrandRequest
                {
                    BrandId = commonInfo.BrandID,
                    ModifyType = "Add"
                }
            };
        }

        public async Task<object> CreateRequestDtoEdit(SubmitMainRequestAll request, string request1, string request2, StoreSetting storeSetting)
        {
            POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.SubmitMainRequest basicInfo = JsonConvert.DeserializeObject<POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.SubmitMainRequest>(request.BasicInfo);

            POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.SubmitMainRequest historyRequest = JsonConvert.DeserializeObject<POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.SubmitMainRequest>(request1);


            SubmitMainResponse historyResponse = JsonConvert.DeserializeObject<SubmitMainResponse>(request2);

            SubmitGoodsEditRequest editRequest = new SubmitGoodsEditRequest
            {
                MainRequest = new UpdateMainDetailRequest
                {
                    Id = historyResponse.Id,
                    CategoryId = (int)storeSetting.CategoryId,
                    ShopCategoryId = (int)basicInfo.ShopCategoryId,
                    SellingStartDateTime = basicInfo.SellingStartDateTime,
                    SellingEndDateTime = basicInfo.SellingEndDateTime,
                    ShippingTypes = basicInfo.ShipType_91app,
                    PayTypes = basicInfo.PayTypes,
                    Type = basicInfo.Type,
                    Specifications = basicInfo.Specifications.ConvertToDictionary(basicInfo.IndexList.ConvertToDictionary()),
                    ProductHighlight = basicInfo.ProductHighlight,
                    ProductDescription = basicInfo.ProductDescription,
                    MoreInfo = basicInfo.MoreInfo,
                    SEOTitle = basicInfo.SEOTitle,
                    SEOKeywords = basicInfo.SEOKeywords,
                    SEODescription = basicInfo.SEODescription,
                    TemperatureTypeDef = basicInfo.TemperatureTypeDef,
                    Length = basicInfo.Length,
                    WIdth = basicInfo.WIdth,
                    Height = basicInfo.Height,
                    Weight = basicInfo.Weight,
                    Status = basicInfo.Status,
                    IsShowStockQty = basicInfo.IsShowStockQty,
                    TaxTypeDef = basicInfo.TaxTypeDef,
                    IsReturnable = basicInfo.IsReturnable,
                    IsEnableBookingPickupDate = basicInfo.IsEnableBookingPickupDate,
                    PrepareDays = basicInfo.PrepareDays,
                    AvailablePickupDays = basicInfo.AvailablePickupDays,
                    AvailablePickupStartDateTime = basicInfo.AvailablePickupStartDateTime,
                    AvailablePickupEndDateTime = basicInfo.AvailablePickupEndDateTime,
                    ApplyType = basicInfo.ApplyType,
                    ExpectShippingDate = basicInfo.ExpectShippingDate,
                    ShippingPrepareDay = basicInfo.ShippingPrepareDay.ToString(),
                    IsShowPurchaseList = basicInfo.IsShowPurchaseList,
                    IsShowSoldQty = basicInfo.IsShowSoldQty,
                    IsDesignatedReturnGoodsType = basicInfo.IsDesignatedReturnGoodsType,
                    ReturnGoodsType = basicInfo.ReturnGoodsType,
                    SoldOutActionType = basicInfo.SoldOutActionType,
                    IsRestricted = basicInfo.IsRestricted,
                    SalesModeTypeDef = basicInfo.SalesModeTypeDef,
                    PointsPayPairs = basicInfo.PointsPayPairs
                },
                SkuList = historyResponse
            };

            if (request.MainImage?.Count > 0)
            {
                List<UpdateMainImageRequest> tmp = new List<UpdateMainImageRequest>();
                for(int idx=0; idx<request.MainImage.Count; idx++)
                {
                    IFormFile file = request.MainImage[idx];
                    if (file == null || file.Length == 0) continue;
                    tmp.Add(new UpdateMainImageRequest
                    {
                        Id = historyResponse.Id,
                        Image = file,
                        Index = idx
                    });
                }
                if (tmp.Count > 0) editRequest.MainImage = tmp;
            }

            if (request.SkuImage?.Count > 0)
            {
                List<UpdateSKUImageRequest> tmp = new List<UpdateSKUImageRequest>();
                for (int idx = 0; idx < request.SkuImage.Count; idx++)
                {
                    IFormFile file = request.SkuImage[idx];
                    if (file == null || file.Length == 0) continue;

                    SkuListRes res = historyResponse.SkuList.Find(t => t.OuterId == basicInfo.SkuList[idx].OriginalOuterId);

                    tmp.Add(new UpdateSKUImageRequest
                    {
                        Id = res?.SkuId,
                        Image = file,
                        Index = 0
                    });
                }
                if (tmp.Count > 0) editRequest.SkuImage = tmp;
            }


            if (basicInfo.SalePageSpecChartId != historyRequest.SalePageSpecChartId)
            {
                editRequest.SpecChartRequest = new UpdateSpecChartIdRequest
                {
                    SalePageId = historyResponse.Id,
                    SalePageSpecChartId = basicInfo.SalePageSpecChartId
                };
            }

            if(storeSetting.Title != historyRequest.Title)
            {
                editRequest.UpdateTitleRequest = new UpdateTitleRequest
                {
                    Id = historyResponse.Id,
                    Title = storeSetting.Title
                };
            }

            List<UpdSku> updSku = new List<UpdSku>();
            List<SkuList_All> addSku = new List<SkuList_All>();
            if (!basicInfo.HasSku && !historyRequest.HasSku)
            {
                updSku.Add(new UpdSku
                {
                    Id = historyResponse.SkuList[0].SkuId,
                    ChangeQty = basicInfo.Qty - historyRequest.Qty,
                    Sort = 1,
                    OnceQty = (int)basicInfo.OnceQty,
                    OuterId = basicInfo.OuterId,
                    IsShow = true,
                    SuggestPrice = basicInfo.SuggestPrice,
                    Price = basicInfo.Price,
                    Cost = basicInfo.Cost,
                    SafetyStockQty = basicInfo.SafetyStockQty
                });
            }
            else
            {
                
                for (int idx =0; idx<basicInfo.SkuList.Count; idx++)
                {
                    SkuItem sku = basicInfo.SkuList[idx];
                    SkuItem historySku = historyRequest.SkuList.Find(t => t.OuterId == sku.OuterId);
                    SkuListRes res = historyResponse.SkuList.Find(t => t.OuterId == sku.OriginalOuterId);
                    if (res != null)
                    {
                        updSku.Add(new UpdSku
                        {
                            Id = res.SkuId,
                            ChangeQty = sku.Qty - historySku.Qty,
                            Sort = idx + 1,
                            OnceQty = sku.OnceQty,
                            OuterId = sku.OuterId,
                            IsShow = true,
                            SuggestPrice = sku.SuggestPrice,
                            Price = sku.Price,
                            Cost = sku.Cost,
                            SafetyStockQty = sku.SafetyStockQty
                        });
                    }
                    else
                    {
                        addSku.Add(new SkuList_All
                        {
                            Name = sku.ConbineColDetail(),
                            Sort = idx + 1,
                            Qty = sku.Qty,
                            OnceQty = sku.OnceQty,
                            OuterId = sku.OuterId,
                            IsShow = true,
                            SuggestPrice = sku.SuggestPrice,
                            Price = sku.Price,
                            Cost = sku.Cost,
                            SafetyStockQty = sku.SafetyStockQty
                        });
                    }
                }

               
            }

            if (updSku.Count > 0)
            {
                editRequest.UpdateSaleProductSkuRequest = new UpdateSaleProductSkuRequest
                {
                    Id = historyResponse.Id,
                    IsSkuDifferentPrice = true,
                    SkuList = updSku
                };
            }

            if (addSku.Count > 0)
            {
                editRequest.CreateSaleProductSkuRequest = new CreateSaleProductSkuRequest
                {
                    Id = historyResponse.Id,
                    IsSkuDifferentPrice = true,
                    SkuList = addSku
                };
            }

            return editRequest;
        }

        public Type GetResponseDtoType() => typeof(SubmitMainResponse);
    }
}
