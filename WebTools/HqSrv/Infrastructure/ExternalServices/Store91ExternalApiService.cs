using HqSrv.Application.Services.ApiKey;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using POVWebDomain.Common;
using POVWebDomain.Models.API.StoreSrv.Common;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using POVWebDomain.Models.DB.POVWeb;
using POVWebDomain.Models.ExternalApi.Store91;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using StringContentWithoutContentType = POVWebDomain.Models.ExternalApi.Store91.StringContentWithoutContentType;
using SubmitMainRequest = POVWebDomain.Models.ExternalApi.Store91.SubmitMainRequest;
using HqSrv.Application.Services;

namespace HqSrv.Infrastructure.ExternalServices
{
    public class ApiResponse_91App<T>
    {
        public string ErrorId { get; set; }
        public string Status { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class JsonHelper_91App
    {
        public static ApiResponse_91App<T> DeserializeApiResponse<T>(string jsonResponse)
        {
            return JsonConvert.DeserializeObject<ApiResponse_91App<T>>(jsonResponse);
        }
    }

    public class Store91ExternalApiService : BaseExternalApiService
    {
        private readonly string _endPoint;
        public override string EndPoint => _endPoint;
        private string _platformId;
        private string _storeId;

        public Store91ExternalApiService(
            POVWebDbContextDapper context,
            IConfiguration configuration,
            HttpClientService httpClient,
            ApiKeyProvider apiKeyProvider)
            : base(context, configuration, httpClient, apiKeyProvider)
        {
            _endPoint = "https://api.91app.com/ec/";
            _platformId = "2";
        }

        protected override async Task<Dictionary<string, string>> GetHeadersAsync(string endPoint)
        {
            _storeId = await _apiKeyProvider.GetApiKeyStoreIDAsync(_platformId);
            string apiKey = await _apiKeyProvider.GetApiKeyAsync(_storeId);

            return new Dictionary<string, string>
            {
                { "x-api-key", apiKey }
            };
        }

        protected override Result<T> ProcessResponse<T>(T response)
        {
            if (response is string jsonString)
            {
                if (jsonString.StartsWith("\"") && jsonString.EndsWith("\""))
                {
                    var innerJsonString = jsonString.Trim('\"').Replace("\\\"", "\"");
                    try
                    {
                        var parsedResponse = JsonHelper_91App.DeserializeApiResponse<object>(innerJsonString);
                        if (!string.IsNullOrEmpty(parsedResponse.ErrorMessage))
                        {
                            return Result<T>.Failure(Error.Custom("91APP_ERROR", parsedResponse.ErrorMessage));
                        }
                    }
                    catch
                    {
                        // 解析失敗就繼續往下
                    }
                }
            }

            if (response is ApiResponse_91App<object> apiResponse)
            {
                if (!string.IsNullOrEmpty(apiResponse.ErrorMessage))
                {
                    return Result<T>.Failure(Error.Custom("91APP_ERROR", apiResponse.ErrorMessage));
                }
            }

            return Result<T>.Success(response);
        }

        public void Configure(string platformIdInput)
        {
            if (string.IsNullOrEmpty(platformIdInput))
            {
                throw new ArgumentException("platformId 不能為空", nameof(platformIdInput));
            }
            _platformId = platformIdInput;
        }

        private async Task EnsureStoreIdAsync()
        {
            if (string.IsNullOrEmpty(_platformId))
            {
                throw new InvalidOperationException("PlatformId 尚未設定，請先呼叫 Configure() 方法");
            }

            if (string.IsNullOrEmpty(_storeId))
            {
                _storeId = await _apiKeyProvider.GetApiKeyStoreIDAsync(_platformId);
            }
        }

        public MultipartFormDataContent GetMultipartFormDataContent()
        {
            var content = new MultipartFormDataContent();
            var boundary = content.Headers.ContentType.Parameters.First(o => o.Name == "boundary");
            boundary.Value = boundary.Value.Replace("\"", String.Empty);
            return content;
        }

        // ============ API 方法 ============

        public async Task<Result<ApiResponse_91App<List<GetCategoryResponse>>>> GetCategory(GetCategoryRequest request)
        {
            return await CallPostAsync<GetCategoryRequest, GetCategoryRequest, ApiResponse_91App<List<GetCategoryResponse>>>(
                "V1/Category/GetCategory", request);
        }

        public async Task<Result<dynamic>> GetShopCategory(GetShopCategoryRequest request)
        {
            await EnsureStoreIdAsync();
            request.ShopId = long.Parse(_storeId);

            var result = await CallPostAsync<GetShopCategoryRequest, GetShopCategoryRequest, ApiResponse_91App<List<GetShopCategoryResponse>>>(
                "V1/Category/GetShopCategory", request);

            if (result.IsFailure)
                return Result<dynamic>.Failure(result.Error);

            var responseBody = ProcessShopCategoryResponse(result.Data.Data);
            return Result<dynamic>.Success(new { responseBody });
        }

        public async Task<Result<dynamic>> GetPayment(GetPaymentRequest request)
        {
            var result = await CallPostAsync<GetPaymentRequest, GetPaymentRequest, ApiResponse_91App<List<GetPaymentResponse>>>(
                "V1/Shop/GetPayment", request);

            if (result.IsFailure)
                return Result<dynamic>.Failure(result.Error);

            GetPaymentReturn responseBody = new GetPaymentReturn
            {
                Options = result.Data.Data.Select(t => new CheckboxOption<string> { ID = t.PayType, Name = t.PayTypeDesc }).ToList(),
            };
            responseBody.PayTypes = responseBody.Options.Select(t => new PayTypes { ID = t.ID }).ToList();

            return Result<dynamic>.Success(new { responseBody });
        }

        public async Task<Result<dynamic>> GetShipping(GetShippingRequest request)
        {
            await EnsureStoreIdAsync();
            request.Id = long.Parse(_storeId);

            var result = await CallPostAsync<GetShippingRequest, GetShippingRequest, ApiResponse_91App<List<GetShippingResponse>>>(
                "V1/Shop/GetShipping", request);

            if (result.IsFailure)
                return Result<dynamic>.Failure(result.Error);

            GetShippingReturn responseBody = new GetShippingReturn
            {
                Options = result.Data.Data.Select(t => new CheckboxOption<long> { ID = t.Id, Name = t.TypeName }).ToList()
            };
            responseBody.ShippingTypes = responseBody.Options.Select(t => new ShippingTypes { ID = t.ID }).ToList();

            return Result<dynamic>.Success(new { responseBody });
        }

        public async Task<Result<ApiResponse_91App<SalePageSpecChartGetListResponse>>> SalePageSpecChartGetList(
            SalePageSpecChartGetListRequest request)
        {
            await EnsureStoreIdAsync();
            request.SearchItem.ShopId = long.Parse(_storeId);

            return await CallPostAsync<SalePageSpecChartGetListRequest, SalePageSpecChartGetListRequest,
                ApiResponse_91App<SalePageSpecChartGetListResponse>>(
                "V2/SalePageSpecChart/GetList", request);
        }

        public async Task<Result<ApiResponse_91App<SubmitMainResponse>>> SubmitMain(SubmitMainRequest request)
        {
            await EnsureStoreIdAsync();
            request.ShopId = long.Parse(_storeId);

            return await CallPostAsync<SubmitMainRequest, SubmitMainRequest, ApiResponse_91App<SubmitMainResponse>>(
                "V1/SalePage/SubmitMain", request);
        }

        public async Task<Result<ApiResponse_91App<CreateSaleProductSkuResponse>>> CreateSaleProductSku(
            CreateSaleProductSkuRequest request)
        {
            return await CallPostAsync<CreateSaleProductSkuRequest, CreateSaleProductSkuRequest,
                ApiResponse_91App<CreateSaleProductSkuResponse>>(
                "V2/SalePage/CreateSaleProductSku", request);
        }

        public async Task<Result<ApiResponse_91App<string>>> UpdateMainImage(UpdateMainImageRequest request)
        {
            var content = GetMultipartFormDataContent();

            content.Add(new StringContentWithoutContentType(request.Id.ToString()), "\"Id\"");
            content.Add(new StringContentWithoutContentType(request.Index.ToString()), "\"Index\"");
            content.Add(new StringContentWithoutContentType($"{{\"Id\":{request.Id},\"Index\":{request.Index}}}"), "\"data\"");

            if (request.Image != null)
            {
                var imageContent = new StreamContent(request.Image.OpenReadStream());
                imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                content.Add(imageContent, "\"Image\"", $"\"{request.Image.FileName}\"");
            }

            return await CallMultipartAsync<ApiResponse_91App<string>>(
                "V1/SalePage/UpdateMainImageJObject", content);
        }

        public async Task<Result<ApiResponse_91App<string>>> UpdateSKUImage(UpdateSKUImageRequest request)
        {
            var content = GetMultipartFormDataContent();

            content.Add(new StringContentWithoutContentType(request.Id.ToString()), "\"Id\"");
            content.Add(new StringContentWithoutContentType(request.Index.ToString()), "\"Index\"");
            content.Add(new StringContentWithoutContentType($"{{\"Id\":{request.Id},\"Index\":{request.Index}}}"), "\"data\"");

            if (request.Image != null)
            {
                var imageContent = new StreamContent(request.Image.OpenReadStream());
                imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                content.Add(imageContent, "\"Image\"", $"\"{request.Image.FileName}\"");
            }

            return await CallMultipartAsync<ApiResponse_91App<string>>(
                "V1/SalePage/UpdateSKUImageJObject", content);
        }

        public async Task<Result<ApiResponse_91App<string>>> UpdateSpecChartId(UpdateSpecChartIdRequest request)
        {
            await EnsureStoreIdAsync();
            request.ShopId = long.Parse(_storeId);

            return await CallPostAsync<UpdateSpecChartIdRequest, UpdateSpecChartIdRequest, ApiResponse_91App<string>>(
                "V2/SalePageExtensionInfo/UpdateSpecChartId", request);
        }

        public async Task<Result<ApiResponse_91App<string>>> UpdateMainDetail(UpdateMainDetailRequest request)
        {
            await EnsureStoreIdAsync();
            request.ShopId = long.Parse(_storeId);

            return await CallPostAsync<UpdateMainDetailRequest, UpdateMainDetailRequest, ApiResponse_91App<string>>(
                "V1/SalePage/UpdateMainDetail", request);
        }

        public async Task<Result<ApiResponse_91App<string>>> UpdateTitle(UpdateTitleRequest request)
        {
            return await CallPostAsync<UpdateTitleRequest, UpdateTitleRequest, ApiResponse_91App<string>>(
                "V1/SalePage/UpdateTitle", request);
        }

        public async Task<Result<ApiResponse_91App<string>>> UpdatePrice(UpdatePriceRequest request)
        {
            return await CallPostAsync<UpdatePriceRequest, UpdatePriceRequest, ApiResponse_91App<string>>(
                "V1/SalePage/UpdatePrice", request);
        }

        public async Task<Result<ApiResponse_91App<string>>> UpdateSkuDetail(UpdateSkuDetailRequest request)
        {
            return await CallPostAsync<UpdateSkuDetailRequest, UpdateSkuDetailRequest, ApiResponse_91App<string>>(
                "V1/SalePage/UpdateSkuDetail", request);
        }

        public async Task<Result<ApiResponse_91App<string>>> UpdateSellingQty(UpdateSellingQtyRequest request)
        {
            return await CallPostAsync<UpdateSellingQtyRequest, UpdateSellingQtyRequest, ApiResponse_91App<string>>(
                "V2/SalePage/UpdateSellingQty", request);
        }

        public async Task<Result<ApiResponse_91App<UpdateSaleProductSkuResponse>>> UpdateSaleProductSku(
            UpdateSaleProductSkuRequest request)
        {
            return await CallPostAsync<UpdateSaleProductSkuRequest, UpdateSaleProductSkuRequest,
                ApiResponse_91App<UpdateSaleProductSkuResponse>>(
                "V2/SalePage/UpdateSaleProductSku", request);
        }

        public async Task<Result<ApiResponse_91App<dynamic>>> OperateBrand(OperateBrandRequest request)
        {
            await EnsureStoreIdAsync();
            request.ShopId = long.Parse(_storeId);

            return await CallPostAsync<OperateBrandRequest, OperateBrandRequest, ApiResponse_91App<dynamic>>(
                "V2/Brand/OperateBrand", request);
        }

        public async Task<Result<dynamic>> GetSalesModeType()
        {
            SaleModeTypeReturn responseBody = new SaleModeTypeReturn
            {
                Options = new List<CheckboxOption<int>> {
                    new CheckboxOption<int>(1, "現金"),
                    new CheckboxOption<int>(2, "點數加價購")
                },
            };
            responseBody.SalesModeTypeDef = responseBody.Options.Select(t => new SaleModeTypes
            {
                ID = t.ID,
                Checked = t.ID == 1
            }).ToList();

            return Result<dynamic>.Success(new { responseBody });
        }

        public async Task<Result<dynamic>> GetSellingDateTime()
        {
            SellingDateTimeReturn responseBody = new SellingDateTimeReturn
            {
                Options = new List<Option<int>> {
                    new Option<int>(0, "一年"),
                    new Option<int>(1, "五年"),
                    new Option<int>(2, "無期限"),
                    new Option<int>(3, "自定結束時間")
                }
            };
            responseBody.SellingDateTime = responseBody.Options[0];

            return Result<dynamic>.Success(new { responseBody });
        }

        private List<MultipleLayerOption<int>> ProcessShopCategoryResponse(List<GetShopCategoryResponse> data)
        {
            List<MultipleLayerOption<int>> responseBody = new List<MultipleLayerOption<int>>();
            int tempIdCounter = -1;

            foreach (var category in data)
            {
                string[] levels = category.CategoryLevel.Split(new string[] { " > " }, StringSplitOptions.None);
                int categoryId = category.CategoryId;
                List<MultipleLayerOption<int>> currentLevelChildren = responseBody;

                for (int i = 0; i < levels.Length; i++)
                {
                    string levelName = levels[i];
                    bool isLeafNode = (i == levels.Length - 1);

                    MultipleLayerOption<int> existingCategory = currentLevelChildren
                        .FirstOrDefault(c => c.Label == levelName);

                    if (existingCategory != null)
                    {
                        currentLevelChildren = existingCategory.Children;
                    }
                    else
                    {
                        MultipleLayerOption<int> newCategory = new MultipleLayerOption<int>
                        {
                            Value = isLeafNode ? categoryId : tempIdCounter--,
                            Label = levelName
                        };

                        currentLevelChildren.Add(newCategory);
                        currentLevelChildren = newCategory.Children;
                    }
                }
            }

            return responseBody;
        }
    }
}