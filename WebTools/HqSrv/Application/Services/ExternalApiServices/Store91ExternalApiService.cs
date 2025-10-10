using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using POVWebDomain.Models.ExternalApi.OMO91;
using System;
using System.Linq;
using POVWebDomain.Models.ExternalApi.Store91;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using POVWebDomain.Models.DB.POVWeb;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Net;
using POVWebDomain.Models.API.StoreSrv.Common;
using System.Text;
using HqSrv.Application.Services.ApiKey;
using Azure.Core;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using SubmitMainRequest = POVWebDomain.Models.ExternalApi.Store91.SubmitMainRequest;
using StringContentWithoutContentType = POVWebDomain.Models.ExternalApi.Store91.StringContentWithoutContentType;


namespace HqSrv.Application.Services.ExternalApiServices.Store91
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



        protected override (T result, string errorMessage) ProcessResponse<T>(T response)
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
                            return (default(T), parsedResponse.ErrorMessage);
                        }
                    }
                    catch
                    {

                    }
                }
            }

            // 處理標準的91App回應格式
            if (response is ApiResponse_91App<object> apiResponse)
            {
                if (!string.IsNullOrEmpty(apiResponse.ErrorMessage))
                {
                    return (default(T), apiResponse.ErrorMessage);
                }
            }

            return (response, string.Empty);
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


        // 商品品類查詢（系統定義）
        public async Task<(object, string)> GetCategory(GetCategoryRequest request)
        {
            return await CallPostAsync<GetCategoryRequest, GetCategoryRequest, ApiResponse_91App<List<GetCategoryResponse>>>(
                "V1/Category/GetCategory", request);
        }
        


        // 商品分類查詢（商店定義）
        public async Task<(object, string)> GetShopCategory(GetShopCategoryRequest request)
        {
            await EnsureStoreIdAsync();
            request.ShopId = long.Parse(_storeId);
            var (data, error) = await CallPostAsync<GetShopCategoryRequest, GetShopCategoryRequest, ApiResponse_91App<List<GetShopCategoryResponse>>>(
                "V1/Category/GetShopCategory", request);

            if (!string.IsNullOrEmpty(error))
                return (null, error);

            // 處理特殊的回應格式轉換
            var responseBody = ProcessShopCategoryResponse(data.Data);
            return (new { responseBody }, string.Empty);
        
        }

        // 查詢商店支援的付款方式
        public async Task<(object, string)> GetPayment(GetPaymentRequest request)
        {
            var (data, error) = await CallPostAsync<GetPaymentRequest, GetPaymentRequest, ApiResponse_91App<List<GetPaymentResponse>>>(
                "V1/Shop/GetPayment", request);

            if (!string.IsNullOrEmpty(error))
                return (null, error);

            GetPaymentReturn responseBody = new GetPaymentReturn
            {
                Options = data.Data.Select(t => new CheckboxOption<string> { ID = t.PayType, Name = t.PayTypeDesc }).ToList(),
            };
            responseBody.PayTypes = responseBody.Options.Select(t => new POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.PayTypes
            {
                ID = t.ID
            }).ToList();

            return (new { responseBody }, string.Empty);
        }

        // 查詢商店支援的物流方式
        public async Task<(object, string)> GetShipping(GetShippingRequest request)
        {
            await EnsureStoreIdAsync();
            request.Id = long.Parse(_storeId);
            var (data, error) = await CallPostAsync<GetShippingRequest, GetShippingRequest, ApiResponse_91App<List<GetShippingResponse>>>(
                "V1/Shop/GetShipping", request);

            if (!string.IsNullOrEmpty(error))
                return (null, error);

            GetShippingReturn responseBody = new GetShippingReturn
            {
                Options = data.Data.Select(t => new CheckboxOption<long> { ID = t.Id, Name = t.TypeName }).ToList()
            };
            responseBody.ShippingTypes = responseBody.Options.Select(t => new POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.ShippingTypes
            {
                ID = t.ID
            }).ToList();

            return (new { responseBody }, string.Empty);
        }


        // 新增單一商品頁
        public async Task<(object, string)> SubmitMain(SubmitMainRequest request)
        {
            await EnsureStoreIdAsync();
            request.ShopId = long.Parse(_storeId);
            return await CallPostAsync<SubmitMainRequest, SubmitMainRequest, ApiResponse_91App<SubmitMainResponse>>(
                "V1/SalePage/SubmitMain", request);
        }

        // 批次新增商品選項
        public async Task<(object, string)> CreateSaleProductSku(CreateSaleProductSkuRequest request)
        {
            return await CallPostAsync<CreateSaleProductSkuRequest, CreateSaleProductSkuRequest, ApiResponse_91App<CreateSaleProductSkuResponse>>(
                "V2/SalePage/CreateSaleProductSku", request);
        }

        // 商品頁圖片維護
        public async Task<(object, string)> UpdateMainImage(UpdateMainImageRequest request)
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

        // 商品選項圖片維護
        public async Task<(object, string)> UpdateSKUImage(UpdateSKUImageRequest request)
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

        // 查詢商品頁規格表清單
        public async Task<(object, string)> SalePageSpecChartGetList(SalePageSpecChartGetListRequest request)
        {
            await EnsureStoreIdAsync();
            request.SearchItem.ShopId = long.Parse(_storeId);
            var (data, error) = await CallPostAsync<SalePageSpecChartGetListRequest, SalePageSpecChartGetListRequest, ApiResponse_91App<SalePageSpecChartGetListResponse>>(
                "V2/SalePageSpecChart/GetList", request);

            if (!string.IsNullOrEmpty(error))
                return (null, error);

            List<Option<long>> responseBody = data.Data.List.Select(t => new Option<long> { Value = t.SalePageSpecChartId ?? -1, Label = t.Name }).ToList();
            return (new { responseBody }, string.Empty);
        }

        // 更新商品頁與商品頁規格表的關聯
        public async Task<(object, string)> UpdateSpecChartId(UpdateSpecChartIdRequest request)
        {
            await EnsureStoreIdAsync();
            request.ShopId = long.Parse(_storeId);
            return await CallPostAsync<UpdateSpecChartIdRequest, UpdateSpecChartIdRequest, ApiResponse_91App<string>>(
                "V2/SalePageExtensionInfo/UpdateSpecChartId", request);
        }

        // 商品頁明細變更
        public async Task<(object, string)> UpdateMainDetail(UpdateMainDetailRequest request)
        {
            await EnsureStoreIdAsync();
            request.ShopId = long.Parse(_storeId);
            return await CallPostAsync<UpdateMainDetailRequest, UpdateMainDetailRequest, ApiResponse_91App<string>>(
                "V1/SalePage/UpdateMainDetail", request);
        }

        // 變更商品頁名稱
        public async Task<(object, string)> UpdateTitle(UpdateTitleRequest request)
        {
            return await CallPostAsync<UpdateTitleRequest, UpdateTitleRequest, ApiResponse_91App<string>>(
                "V1/SalePage/UpdateTitle", request);
        }

        // 商品頁銷售價格 - 變更
        public async Task<(object, string)> UpdatePrice(UpdatePriceRequest request)
        {
            return await CallPostAsync<UpdatePriceRequest, UpdatePriceRequest, ApiResponse_91App<string>>(
                "V1/SalePage/UpdatePrice", request);
        }

        // 商品選項明細變更
        public async Task<(object, string)> UpdateSkuDetail(UpdateSkuDetailRequest request)
        {
            return await CallPostAsync<UpdateSkuDetailRequest, UpdateSkuDetailRequest, ApiResponse_91App<string>>(
                "V1/SalePage/UpdateSkuDetail", request);
        }

        // 商品可售數量 - 變更
        public async Task<(object, string)> UpdateSellingQty(UpdateSellingQtyRequest request)
        {
            return await CallPostAsync<UpdateSellingQtyRequest, UpdateSellingQtyRequest, ApiResponse_91App<string>>(
                "V2/SalePage/UpdateSellingQty", request);
        }

        // 變更商品選項
        public async Task<(object, string)> UpdateSaleProductSku(UpdateSaleProductSkuRequest request)
        {
            return await CallPostAsync<UpdateSaleProductSkuRequest, UpdateSaleProductSkuRequest, ApiResponse_91App<UpdateSaleProductSkuResponse>>(
                "V2/SalePage/UpdateSaleProductSku", request);
        }

        // 商品品牌新增移除與商品頁的關聯
        public async Task<(object, string)> OperateBrand(OperateBrandRequest request)
        {
            await EnsureStoreIdAsync();
            request.ShopId = long.Parse(_storeId);
            return await CallPostAsync<OperateBrandRequest, OperateBrandRequest, ApiResponse_91App<dynamic>>(
                "V2/Brand/OperateBrand", request);
        }

        // 查詢商品銷售方式
        public async Task<(object, string)> GetSalesModeType()
        {
            SaleModeTypeReturn responseBody = new SaleModeTypeReturn
            {
                Options = new List<CheckboxOption<int>> { new CheckboxOption<int>(1, "現金"), new CheckboxOption<int>(2, "點數加價購") },
            };
            responseBody.SalesModeTypeDef = responseBody.Options.Select(t => new POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.SaleModeTypes
            {
                ID = t.ID,
                Checked = t.ID == 1 ? true : false
            }).ToList();

            return (new { responseBody }, "");
        }

        // 查詢商品銷售時間
        public async Task<(object, string)> GetSellingDateTime()
        {
            SellingDateTimeReturn responseBody = new SellingDateTimeReturn
            {
                Options = new List<Option<int>> { new Option<int>(0, "一年"), new Option<int>(1, "五年"), new Option<int>(2, "無期限"), new Option<int>(3, "自定結束時間") }
            };
            responseBody.SellingDateTime = responseBody.Options[0];

            return (new { responseBody }, "");
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
