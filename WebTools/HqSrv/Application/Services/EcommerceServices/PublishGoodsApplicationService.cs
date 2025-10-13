// ============================================
// 第三步：重構 Application Layer
// ============================================

// 在 Application/Services/EcommerceMgmt/ 資料夾下創建或修改：

// ============================================
// Application/Services/EcommerceMgmt/PublishGoodsApplicationService.cs (重構)
// ============================================
using HqSrv.Infrastructure.ExternalServices;
using HqSrv.Factories.Ecommerce;
using HqSrv.Repository.EcommerceMgmt;
using HqSrv.Domain.Services;  // 新增 - 引用 Domain Services
using HqSrv.Domain.Entities;  // 新增 - 引用 Domain Entities
using HqSrv.Domain.Repositories;  // 新增 - 引用 Domain Repository 介面
using Newtonsoft.Json;
using POVWebDomain.Common;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using POVWebDomain.Models.ExternalApi.Store91;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SubmitMainRequest = POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.SubmitMainRequest;
using HqSrv.Infrastructure.Repositories;

namespace HqSrv.Application.Services.EcommerceMgmt
{
    /// <summary>
    /// 商品發布應用服務 - 重構為用例協調器
    /// </summary>
    public class PublishGoodsApplicationService : IPublishGoodsApplicationService
    {
        // 基礎設施依賴
        private readonly IPublishGoodsInfrastructureRepository _repository;
        private readonly IPublishGoodsRepository _domainRepository;          // 新增 Domain 介面
        private readonly Store91ExternalApiService _91Api;
        private readonly IEcommerceFactoryManager _ecommerceFactoryManager;

        // Domain 服務依賴 - 新增
        private readonly IProductValidationService _productValidationService;
        private readonly IPublishingService _publishingService;
        private readonly IPlatformMappingService _platformMappingService;
        private readonly IProductRepository _productRepository;


        public PublishGoodsApplicationService(
            IPublishGoodsInfrastructureRepository repository,
            IPublishGoodsRepository domainRepository,
            Store91ExternalApiService api91,
            IEcommerceFactoryManager ecommerceFactoryManager,
            IProductValidationService productValidationService,  // 新增
            IPublishingService publishingService,                // 新增
            IPlatformMappingService platformMappingService,      // 新增
            IProductRepository productRepository)                // 新增
        {
            _repository = repository;
            _domainRepository = domainRepository;
            _91Api = api91;
            _ecommerceFactoryManager = ecommerceFactoryManager;
            _productValidationService = productValidationService;
            _publishingService = publishingService;
            _platformMappingService = platformMappingService;
            _productRepository = productRepository;
        }

        // ============================================
        // 取得發布選項 - 保持原樣（主要是資料查詢）
        // ============================================
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

                // Repository 呼叫
                var ecIndex = await _repository.MergeEcAttributesAsync(
                    platforms: new List<string> { "0002", "0003", "0004" },
                    categoryCode: "your_category_code");

                var response = new GetOptionAllResponse
                {
                    ShipType_91app = (shippingResult.Data as dynamic)?.responseBody,
                    Payment = (paymentResult.Data as dynamic)?.responseBody,
                    SpecChart = (specChartResult.Data as dynamic)?.options,
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

        // ============================================
        // 商品發布 - 重構為用例協調器
        // ============================================
        public async Task<Result<object>> SubmitProductAsync(SubmitMainRequestAll request)
        {
            try
            {
                // 1. 將 DTO 轉換為 Domain Entity
                var productResult = await ConvertToProductEntityAsync(request);
                if (productResult.IsFailure)
                    return Result<object>.Failure(productResult.Error);

                var product = productResult.Data;

                // 2. 取得目標發布平台
                var targetPlatforms = GetTargetPlatforms(request.StoreSettings);

                // 3. 使用 Domain Service 進行發布前驗證
                var canPublishResult = await _publishingService.CanPublishAsync(product, targetPlatforms);
                if (canPublishResult.IsFailure)
                    return Result<object>.Failure(canPublishResult.Error);

                // 4. 使用 Domain Service 計算發布順序
                var publishOrderResult = _publishingService.CalculatePublishOrder(targetPlatforms);
                if (publishOrderResult.IsFailure)
                    return Result<object>.Failure(publishOrderResult.Error);


                // 5. 執行實際發布流程
                var publishResult = await ExecutePublishingWorkflowAsync(request, product, publishOrderResult.Data);
                if (publishResult.IsFailure)
                    return Result<object>.Failure(publishResult.Error);

                return Result<object>.Success(publishResult.Data);
            }
            catch (Exception ex)
            {
                return Result<object>.Failure(Error.Custom("SUBMIT_PRODUCT_ERROR", ex.Message));
            }
        }

        // ============================================
        // 私有方法 - 商品實體轉換
        // ============================================
        private async Task<Result<Product>> ConvertToProductEntityAsync(SubmitMainRequestAll request)
        {
            try
            {
                // 解析基本資訊
                var basicInfo = JsonConvert.DeserializeObject<SubmitMainRequest>(request.BasicInfo);

                // 創建商品實體
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
                    width: basicInfo.WIdth,  // 注意原本的拼寫
                    length: basicInfo.Length,
                    weight: basicInfo.Weight);

                // 處理 SKU
                if (basicInfo.HasSku && basicInfo.SkuList?.Any() == true)
                {
                    product.EnableSkuMode();

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
                else
                {
                    product.DisableSkuMode(
                        qty: basicInfo.Qty ?? 0,
                        onceQty: basicInfo.OnceQty ?? 1,
                        outerId: basicInfo.OuterId ?? "");
                }

                return Result<Product>.Success(product);
            }
            catch (Exception ex)
            {
                return Result<Product>.Failure(Error.Custom("CONVERT_TO_ENTITY_ERROR", ex.Message));
            }
        }

        // ============================================
        // 私有方法 - 取得目標發布平台
        // ============================================
        private List<string> GetTargetPlatforms(string storeSettingsJson)
        {
            try
            {
                var storeSettings = JsonConvert.DeserializeObject<List<StoreSetting>>(storeSettingsJson);
                return storeSettings
                    .Where(s => s.Publish)
                    .Select(s => s.EStoreID)
                    .ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        // ============================================
        // 私有方法 - 執行發布工作流程
        // ============================================
        private async Task<Result<object>> ExecutePublishingWorkflowAsync(
            SubmitMainRequestAll request,
            Product product,
            List<string> orderedPlatforms)
        {
            try
            {
                var apiResponse = new SubmitMainResponseAll();
                var errorList = new List<string>();

                // 處理圖片
                await _repository.HandleImageAsync(request);


                // 解析商店設定
                var storeSettings = JsonConvert.DeserializeObject<List<StoreSetting>>(request.StoreSettings);

                // 按照優先順序發布到各平台
                foreach (var platformId in orderedPlatforms)
                {
                    var store = storeSettings.FirstOrDefault(s => s.EStoreID == platformId && s.Publish);
                    if (store == null) continue;

                    // 使用 Domain Service 準備發布資料
                    var prepareResult = await _publishingService.PreparePublishDataAsync(product, platformId, store);
                    if (prepareResult.IsFailure)
                    {
                        errorList.Add($"平台 {platformId} 資料準備失敗: {prepareResult.Error.Message}");
                        continue;
                    }

                    // 執行平台發布
                    var publishResult = await PublishToPlatformAsync(request, store, product);
                    if (publishResult.IsFailure)
                    {
                        errorList.Add($"平台 {platformId} 發布失敗: {publishResult.Error.Message}");
                    }
                    else
                    {
                        apiResponse.Response = publishResult.Data;
                    }
                }

                if (errorList.Count == 0)
                {
                    return Result<object>.Success(apiResponse);
                }
                else
                {
                    return Result<object>.Failure(Error.Custom("PUBLISH_ERRORS", string.Join("; ", errorList)));
                }
            }
            catch (Exception ex)
            {
                return Result<object>.Failure(Error.Custom("PUBLISH_WORKFLOW_ERROR", ex.Message));
            }
        }

        // ============================================
        // 私有方法 - 發布到指定平台
        // ============================================
        private async Task<Result<object>> PublishToPlatformAsync(
            SubmitMainRequestAll request,
            StoreSetting store,
            Product product)
        {
            try
            {
                // 取得平台工廠
                var factory = _ecommerceFactoryManager.GetFactory(
                    store.EStoreID switch
                    {
                        "0001" => "91App",
                        "0002" => "Yahoo",
                        "0003" => "Momo",
                        "0004" => "Shopee",
                        _ => null
                    });

                if (factory == null)
                    return Result<object>.Failure(Error.Custom("UNSUPPORTED_PLATFORM", $"不支援的平台: {store.EStoreID}"));

                var service = factory.CreateEcommerceService();

                // 檢查是否為編輯模式
                var resData = await _repository.GetSubmitResByStoreAsync(request.ParentID, store.PlatformID);
                var commonInfo = await _repository.GetLookupAndCommonValueAsync(request.ParentID, store.EStoreID);

                object requestDto;
                Result<object> submitResult;

                if (resData == null)
                {
                    // 新增模式
                    requestDto = await factory.CreateRequestDtoAdd(request, store, commonInfo);
                    submitResult = await service.SubmitGoodsAddAsync(requestDto, store.PlatformID);
                }
                else
                {
                    // 編輯模式
                    var originalRequestParams = await _repository.GetOriginalRequestParamsAsync(request.ParentID);
                    requestDto = await factory.CreateRequestDtoEdit(request, originalRequestParams, resData.ResponseData, store);
                    submitResult = await service.SubmitGoodsEditAsync(requestDto, store.PlatformID);
                }

                // 儲存回應
                if (submitResult.IsSuccess)
                {
                    var saveResResult = await _repository.SaveSubmitGoodsResAsync(request, requestDto,
                        new SubmitMainResponseAll { Response = submitResult.Data }, store);

                    var saveReqResult = await _repository.SaveSubmitGoodsReqAsync(request);

                    if (saveResResult.IsFailure)
                    {
                        // 記錄警告但不失敗
                        // Logger.LogWarning($"儲存回應失敗: {saveResResult.Error.Message}");
                    }
                }

                return submitResult;
            }
            catch (Exception ex)
            {
                return Result<object>.Failure(Error.Custom("PLATFORM_PUBLISH_ERROR", ex.Message));
            }
        }

        // ============================================
        // 其他方法保持不變
        // ============================================
        public async Task<Result<object>> GetSubmitModeAsync(GetSubmitModeRequest request)
        {
            return await _repository.GetSubmitModeAsync(request);
        }

        public async Task<Result<object>> GetSubmitDefaultValuesAsync(GetSubmitDefValRequest request)
        {
            return await _repository.GetSubmitDefValAsync(request);
        }

        public async Task<Result<object>> GetEStoreCategoryOptionsAsync()
        {
            return await _repository.GetEStoreCatOptionsAsync();
        }
    }
}