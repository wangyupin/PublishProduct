// ============================================
// 第三步：重構 Application Layer
// ============================================

// 在 Application/Services/EcommerceMgmt/ 資料夾下創建或修改：

// ============================================
// Application/Services/EcommerceMgmt/PublishGoodsApplicationService.cs (重構)
// ============================================
using HqSrv.Domain.Entities;  // 新增 - 引用 Domain Entities
using HqSrv.Domain.Repositories;  // 新增 - 引用 Domain Repository 介面
using HqSrv.Domain.Services;  // 新增 - 引用 Domain Services
using HqSrv.Factories.Ecommerce;
using HqSrv.Infrastructure.ExternalServices;
using HqSrv.Infrastructure.Helpers;
using HqSrv.Infrastructure.Repositories;
using HqSrv.Repository.EcommerceMgmt;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using POVWebDomain.Common;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using POVWebDomain.Models.ExternalApi.Store91;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SubmitMainRequest = POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.SubmitMainRequest;
using POVWebDomain.Models.ExternalApi.OfficialFront;

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
        private readonly OfficialFrontExternalApiService _officialFrontApi;
        private readonly IEcommerceFactoryManager _ecommerceFactoryManager;

        // Domain 服務依賴 - 新增
        private readonly IProductValidationService _productValidationService;
        private readonly IPublishingService _publishingService;
        private readonly IPlatformMappingService _platformMappingService;
        private readonly IProductRepository _productRepository;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IOptionService _optionService;


        public PublishGoodsApplicationService(
            IPublishGoodsInfrastructureRepository repository,
            IPublishGoodsRepository domainRepository,
            Store91ExternalApiService api91,
            OfficialFrontExternalApiService officialFrontApi,
            IEcommerceFactoryManager ecommerceFactoryManager,
            IProductValidationService productValidationService,  // 新增
            IPublishingService publishingService,                // 新增
            IPlatformMappingService platformMappingService,      // 新增
            IProductRepository productRepository,
            IWebHostEnvironment hostEnvironment,
            IOptionService optionService)                // 新增
        {
            _repository = repository;
            _domainRepository = domainRepository;
            _91Api = api91;
            _officialFrontApi = officialFrontApi;
            _ecommerceFactoryManager = ecommerceFactoryManager;
            _productValidationService = productValidationService;
            _publishingService = publishingService;
            _platformMappingService = platformMappingService;
            _productRepository = productRepository;
            _hostEnvironment = hostEnvironment;
            _optionService = optionService;
        }

        // ============================================
        // 取得發布選項 - 保持原樣（主要是資料查詢）
        // ============================================
        public async Task<Result<GetOptionAllResponse>> GetPublishOptionsAsync(GetOptionAllRequest request)
        {
            try
            {
                // OfficialFront API 呼叫

                var officialFrontCategoryResult = await _officialFrontApi.GetCategory(new POVWebDomain.Models.ExternalApi.OfficialFront.GetCategoryRequest
                {
                    StoreNumber = request.StoreNumber
                });
                if (officialFrontCategoryResult.IsFailure)
                    return Result<GetOptionAllResponse>.Failure(officialFrontCategoryResult.Error);

                //var shopCategoryResult = await _91Api.GetShopCategory(new GetShopCategoryRequest { });
                //if (shopCategoryResult.IsFailure)
                //    return Result<GetOptionAllResponse>.Failure(shopCategoryResult.Error);

                var salesModeTypeResult = await _optionService.GetSalesModeType();
                if (salesModeTypeResult.IsFailure)
                    return Result<GetOptionAllResponse>.Failure(salesModeTypeResult.Error);

                var sellingDateTimeResult = await _optionService.GetSellingDateTime();
                if (sellingDateTimeResult.IsFailure)
                    return Result<GetOptionAllResponse>.Failure(sellingDateTimeResult.Error);

                var shippingResult = await _optionService.GetShipping();
                if (shippingResult.IsFailure)
                    return Result<GetOptionAllResponse>.Failure(shippingResult.Error);

                var paymentResult = await _optionService.GetPayment();
                if (paymentResult.IsFailure)
                    return Result<GetOptionAllResponse>.Failure(paymentResult.Error);

                // Repository 呼叫
                var ecIndex = await _repository.MergeEcAttributesAsync(
                    platforms: new List<string> { "0002", "0003", "0004" },
                    categoryCode: "your_category_code");

                var response = new GetOptionAllResponse
                {
                    Category_Official = (officialFrontCategoryResult.Data as dynamic)?.responseBody,
                    ShipType_91app = (shippingResult.Data as dynamic)?.responseBody,
                    Payment = (paymentResult.Data as dynamic)?.responseBody,
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
                // 1. 驗證輸入
                if (string.IsNullOrEmpty(request.BasicInfo))
                    return Result<object>.Failure(Error.Custom("INVALID_INPUT", "商品基本資料不可為空"));

                // 2. 轉換為 Product 實體
                var productResult = await ConvertToProductEntityAsync(request);
                if (productResult.IsFailure)
                    return Result<object>.Failure(productResult.Error);

                var product = productResult.Data;

                // 3. 取得目標發布平台
                var targetPlatforms = GetTargetPlatforms(request.StoreSettings);

                // 處理刪除邏輯 
                var storeSettings = JsonConvert.DeserializeObject<List<StoreSetting>>(request.StoreSettings);
                var deleteErrors = new List<string>();

                foreach (var store in storeSettings.Where(s => s.NeedDelete))
                {
                    var deleteResult = await DeleteFromPlatformAsync(request.ParentID, store);
                    if (deleteResult.IsFailure)
                    {
                        deleteErrors.Add($"平台 {store.EStoreID} 刪除失敗: {deleteResult.Error.Message}");
                    }
                }

                if (deleteErrors.Any())
                {
                    return Result<object>.Failure(Error.Custom("DELETE_ERRORS", string.Join("; ", deleteErrors)));
                }

                // 4. 使用 Domain Service 進行發布前驗證
                //var canPublishResult = await _publishingService.CanPublishAsync(product, targetPlatforms);
                //if (canPublishResult.IsFailure)
                //    return Result<object>.Failure(canPublishResult.Error);

                // 5. 使用 Domain Service 計算發布順序
                var publishOrderResult = _publishingService.CalculatePublishOrder(targetPlatforms);
                if (publishOrderResult.IsFailure)
                    return Result<object>.Failure(publishOrderResult.Error);


                // 6. 執行實際發布流程
                var publishResult = await ExecutePublishingWorkflowAsync(request, product, publishOrderResult.Data);
                if (publishResult.IsFailure)
                    return Result<object>.Failure(publishResult.Error);


                var saveReqResult = await _repository.SaveSubmitGoodsReqAsync(request);
                if (saveReqResult.IsFailure)
                {
                    // 記錄警告但不失敗
                }

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
                // 處理 SKU
                if (basicInfo.HasSku && basicInfo.SkuList?.Any() == true)
                {
                    // 有選項：使用 SkuList
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
                    // 無選項：用 basicInfo 組一筆 SKU
                    var sku = ProductSku.Create(
                        outerId: request.ParentID,
                        name: basicInfo.Title, // 或空字串 ""
                        qty: basicInfo.Qty ?? 0,
                        onceQty: basicInfo.OnceQty ?? 1,
                        price: basicInfo.Price,
                        cost: basicInfo.Cost);

                    sku.UpdatePricing(basicInfo.SuggestPrice, basicInfo.Price, basicInfo.Cost);
                    sku.UpdateInventory(basicInfo.Qty ?? 0, basicInfo.SafetyStockQty);
                    product.AddSku(sku);
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

                if (!string.IsNullOrEmpty(request.StoreSettings))
                {
                    JObject basicInfoObj = JObject.Parse(request.BasicInfo);
                    basicInfoObj["storeSettings"] = JToken.Parse(request.StoreSettings);
                    request.BasicInfo = basicInfoObj.ToString(Formatting.None);
                }


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
                await ProcessImagesBeforePublish(request);

                // 取得平台工廠
                var factory = _ecommerceFactoryManager.GetFactory(
                    store.EStoreID switch
                    {
                        "0001" => "91App",
                        "0002" => "Yahoo",
                        "0003" => "Momo",
                        "0004" => "Shopee",
                        "0005" => "OfficialWebsite",
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
                bool isEditMode = false;

                if (resData == null)
                {
                    // 新增模式
                    requestDto = await factory.CreateRequestDtoAdd(request, store, commonInfo);
                    submitResult = await service.SubmitGoodsAddAsync(requestDto, store.PlatformID);
                    isEditMode = false;
                }
                else
                {
                    // 編輯模式
                    var originalRequestParams = await _repository.GetOriginalRequestParamsAsync(request.ParentID);
                    requestDto = await factory.CreateRequestDtoEdit(request, originalRequestParams, resData.ResponseData, store, commonInfo);
                    submitResult = await service.SubmitGoodsEditAsync(requestDto, store.PlatformID);
                    isEditMode = true;
                }

                // 儲存回應
                if (submitResult.IsSuccess)
                {
                    bool shouldSaveResponse = !isEditMode || factory.ShouldSaveEditResponse();

                    if (shouldSaveResponse)
                    {
                        var saveResResult = await _repository.SaveSubmitGoodsResAsync(request, requestDto,
                            new SubmitMainResponseAll { Response = submitResult.Data }, store);
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
        // 私有方法 - 從平台刪除商品
        // ============================================
        private async Task<Result<object>> DeleteFromPlatformAsync(string parentID, StoreSetting store)
        {
            try
            {
                // 1. 取得該平台的商品回應資料(包含 ProductID)
                var resData = await _repository.GetSubmitResByStoreAsync(parentID, store.PlatformID);
                if (resData == null)
                {
                    // 如果沒有回應資料,代表該平台從未上架過,不需要刪除
                    return Result<object>.Success(new { Message = "該平台未曾上架,無需刪除" });
                }

                // 2. 解析回應資料取得 ProductID
                var responseData = JsonConvert.DeserializeObject<dynamic>(resData.ResponseData);
                int productID = 0;

                // 根據不同平台解析 ProductID
                if (store.EStoreID == "0005") // OfficialWebsite
                {
                    productID = (int)responseData.ProductID;
                }
                else if (store.EStoreID == "0001") // 91App
                {
                    productID = (int)responseData.Data.Id;
                }

                if (productID == 0)
                {
                    return Result<object>.Failure(Error.Custom("INVALID_PRODUCT_ID", "無法取得商品ID"));
                }

                // 3. 取得平台工廠
                var factory = _ecommerceFactoryManager.GetFactory(
                    store.EStoreID switch
                    {
                        "0001" => "91App",
                        "0002" => "Yahoo",
                        "0003" => "Momo",
                        "0004" => "Shopee",
                        "0005" => "OfficialWebsite",
                        _ => null
                    });

                if (factory == null)
                    return Result<object>.Failure(Error.Custom("UNSUPPORTED_PLATFORM", $"不支援的平台: {store.EStoreID}"));

                var service = factory.CreateEcommerceService();

                // 4. 執行刪除
                int storeNumber = int.Parse(store.PlatformID);
                var deleteResult = await service.DeleteGoodsAsync(storeNumber, productID, store.PlatformID);

                if (deleteResult.IsFailure)
                    return Result<object>.Failure(deleteResult.Error);

                // 5. 刪除成功後,清除資料庫中的回應記錄
                await _repository.DeleteSubmitResByStoreAsync(parentID, store.PlatformID);

                return Result<object>.Success(deleteResult.Data);
            }
            catch (Exception ex)
            {
                return Result<object>.Failure(Error.Custom("DELETE_FROM_PLATFORM_ERROR", ex.Message));
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

        private async Task ProcessImagesBeforePublish(SubmitMainRequestAll request)
        {
            // 處理主圖
            if (request.MainImage != null)
            {
                for (int idx = 0; idx < request.MainImage.Count; idx++)
                {
                    var file = request.MainImage[idx];
                    if (file != null && file.Length == 0 && file.FileName != "blob")
                    {
                        // 從檔案系統讀取
                        var actualFile = await ImageFileHelper.ReadImageFromPath(
                            file.FileName,
                            _hostEnvironment); // 需要注入到 PublishGoodsApplicationService

                        if (actualFile != null)
                        {
                            request.MainImage[idx] = actualFile;
                        }
                    }
                }
            }

            // 處理 SKU 圖片
            if (request.SkuImage != null)
            {
                for (int idx = 0; idx < request.SkuImage.Count; idx++)
                {
                    var file = request.SkuImage[idx];
                    if (file != null && file.Length == 0 && file.FileName != "blob")
                    {
                        var actualFile = await ImageFileHelper.ReadImageFromPath(
                            file.FileName,
                            _hostEnvironment);

                        if (actualFile != null)
                        {
                            request.SkuImage[idx] = actualFile;
                        }
                    }
                }
            }
        }
    }
}