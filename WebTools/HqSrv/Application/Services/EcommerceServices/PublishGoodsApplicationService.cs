using HqSrv.Application.Services.ExternalApiServices.Store91;
using HqSrv.Factories.Ecommerce;
using HqSrv.Repository.EcommerceMgmt;
using Newtonsoft.Json;
using POVWebDomain.Common;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using POVWebDomain.Models.ExternalApi.Store91;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HqSrv.Application.Services.EcommerceMgmt
{
    public class PublishGoodsApplicationService : IPublishGoodsApplicationService
    {
        private readonly IPublishGoodsRepository _repository;
        private readonly Store91ExternalApiService _91Api;
        private readonly IEcommerceFactoryManager _ecommerceFactoryManager;

        public PublishGoodsApplicationService(
            IPublishGoodsRepository repository,
            Store91ExternalApiService api91,
            IEcommerceFactoryManager ecommerceFactoryManager)
        {
            _repository = repository;
            _91Api = api91;
            _ecommerceFactoryManager = ecommerceFactoryManager;
        }

        public async Task<Result<GetOptionAllResponse>> GetPublishOptionsAsync(GetOptionAllRequest request)
        {
            try
            {
                // 91App API 呼叫
                var shippingResult = await _91Api.GetShipping(new GetShippingRequest { });
                if (shippingResult.IsFailure)
                    return Result<GetOptionAllResponse>.Failure(shippingResult.Error);

                var paymentResult = await _91Api.GetPayment(new GetPaymentRequest { });
                if (paymentResult.IsFailure)
                    return Result<GetOptionAllResponse>.Failure(paymentResult.Error);

                var specChartResult = await _91Api.SalePageSpecChartGetList(new SalePageSpecChartGetListRequest
                {
                    SearchItem = new SearchItem(),
                    Skip = 0,
                    Take = 50
                });
                if (specChartResult.IsFailure)
                    return Result<GetOptionAllResponse>.Failure(specChartResult.Error);

                var shopCategoryResult = await _91Api.GetShopCategory(new GetShopCategoryRequest { });
                if (shopCategoryResult.IsFailure)
                    return Result<GetOptionAllResponse>.Failure(shopCategoryResult.Error);

                var salesModeTypeResult = await _91Api.GetSalesModeType();
                if (salesModeTypeResult.IsFailure)
                    return Result<GetOptionAllResponse>.Failure(salesModeTypeResult.Error);

                var sellingDateTimeResult = await _91Api.GetSellingDateTime();
                if (sellingDateTimeResult.IsFailure)
                    return Result<GetOptionAllResponse>.Failure(sellingDateTimeResult.Error);

                // Repository 呼叫 - MergeEcAttributesAsync 沒有改成 Result,所以直接呼叫
                var ecIndex = await _repository.MergeEcAttributesAsync(
                    platforms: new List<string> { "0002", "0003", "0004" },
                    categoryCode: "your_category_code");

                var response = new GetOptionAllResponse
                {
                    ShipType_91app = (shippingResult.Data as dynamic)?.responseBody,
                    Payment = (paymentResult.Data as dynamic)?.responseBody,
                    SpecChart = (specChartResult.Data as dynamic)?.Data,
                    ShopCategory = (shopCategoryResult.Data as dynamic)?.responseBody,
                    SalesModeType = (salesModeTypeResult.Data as dynamic)?.responseBody,
                    SellingDateTime = (sellingDateTimeResult.Data as dynamic)?.responseBody,
                    EcIndex = ecIndex
                };

                return Result<GetOptionAllResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<GetOptionAllResponse>.Failure(Error.Custom("GET_OPTIONS_ERROR", ex.Message));
            }
        }

        public async Task<Result<object>> SubmitProductAsync(SubmitMainRequestAll request)
        {
            try
            {
                var apiResponse = new SubmitMainResponseAll();
                var errorList = new List<string>();

                // 處理圖片
                await _repository.HandleImageAsync(request);

                // 儲存請求 - 改用 Result Pattern
                var saveReqResult = await _repository.SaveSubmitGoodsReqAsync(request);
                if (saveReqResult.IsFailure)
                {
                    errorList.Add(saveReqResult.Error.Message);
                }

                // 解析商店設定
                var storeSettings = JsonConvert.DeserializeObject<List<StoreSetting>>(request.StoreSettings);

                foreach (var store in storeSettings)
                {
                    if (!store.Publish) continue;

                    var factory = _ecommerceFactoryManager.GetFactory(
                        store.EStoreID switch
                        {
                            "0001" => "91App",
                            "0002" => "Yahoo",
                            "0003" => "Momo",
                            "0004" => "Shopee",
                            _ => null
                        });

                    var service = factory.CreateEcommerceService();

                    // GetSubmitResByStoreAsync 沒有改成 Result,直接呼叫
                    var resData = await _repository.GetSubmitResByStoreAsync(request.ParentID, store.PlatformID);

                    // GetLookupAndCommonValueAsync 沒有改成 Result,直接呼叫
                    var commonInfo = await _repository.GetLookupAndCommonValueAsync(request.ParentID, store.EStoreID);

                    object requestDto;
                    if (resData == null)
                    {
                        requestDto = await factory.CreateRequestDtoAdd(request, store, commonInfo);
                        var submitResult = await service.SubmitGoodsAddAsync(requestDto, store.PlatformID);

                        if (submitResult.IsFailure)
                        {
                            errorList.Add(submitResult.Error.Message);
                        }
                        else
                        {
                            apiResponse.Response = submitResult.Data;
                        }
                    }
                    else
                    {
                        requestDto = factory.CreateRequestDtoEdit(request, resData.RequestParams, resData.ResponseData, store);
                        var submitResult = await service.SubmitGoodsEditAsync(requestDto, store.PlatformID);

                        if (submitResult.IsFailure)
                        {
                            errorList.Add(submitResult.Error.Message);
                        }
                        else
                        {
                            apiResponse.Response = submitResult.Data;
                        }
                    }

                    // 儲存回應 - 改用 Result Pattern
                    var saveResResult = await _repository.SaveSubmitGoodsResAsync(request, requestDto, apiResponse, store);
                    if (saveResResult.IsFailure)
                    {
                        errorList.Add(saveResResult.Error.Message);
                    }
                }

                if (errorList.Count == 0)
                {
                    return Result<object>.Success(null);
                }
                else
                {
                    return Result<object>.Failure(Error.Custom("SUBMIT_ERROR", string.Join("; ", errorList)));
                }
            }
            catch (Exception ex)
            {
                return Result<object>.Failure(Error.Custom("SUBMIT_PRODUCT_ERROR", ex.Message));
            }
        }

        public async Task<Result<object>> GetSubmitModeAsync(GetSubmitModeRequest request)
        {
            // 改用 Result Pattern
            return await _repository.GetSubmitModeAsync(request);
        }

        public async Task<Result<object>> GetSubmitDefaultValuesAsync(GetSubmitDefValRequest request)
        {
            // 改用 Result Pattern
            return await _repository.GetSubmitDefValAsync(request);
        }

        public async Task<Result<object>> GetEStoreCategoryOptionsAsync()
        {
            // 改用 Result Pattern
            return await _repository.GetEStoreCatOptionsAsync();
        }
    }
}