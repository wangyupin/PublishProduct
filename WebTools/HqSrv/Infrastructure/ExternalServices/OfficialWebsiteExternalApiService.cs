using HqSrv.Application.Services.ApiKey;
using Microsoft.Extensions.Configuration;
using POVWebDomain.Common;
using POVWebDomain.Models.DB.POVWeb;
using POVWebDomain.Models.ExternalApi.OfficialWebsite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HqSrv.Application.Services;

namespace HqSrv.Infrastructure.ExternalServices
{
    public class ApiResponse_OfficialPlatform<T>
    {
        public string Error { get; set; }
        public T Data { get; set; }
    }

    public class OfficialWebsiteExternalApiService : BaseExternalApiService
    {
        private readonly string _endPoint;
        public override string EndPoint => _endPoint;
        private string _platformId;

        public OfficialWebsiteExternalApiService(
            POVWebDbContextDapper context,
            IConfiguration configuration,
            HttpClientService httpClient,
            ApiKeyProvider apiKeyProvider)
            : base(context, configuration, httpClient, apiKeyProvider)
        {
            _endPoint = "https://onlineshop-admin.gbtech.com.tw/";
        }

        protected override async Task<Dictionary<string, string>> GetHeadersAsync(string endPoint)
        {
            //string apiKey = await _apiKeyProvider.GetApiKeyAsync(_platformId);

            return new Dictionary<string, string>
            {
 
            };
        }

        protected override Result<T> ProcessResponse<T>(T response)
        {
            if (response is ApiResponse_OfficialPlatform<object> apiResponse)
            {
                if (!string.IsNullOrEmpty(apiResponse.Error))
                {
                    return Result<T>.Failure(Error.Custom("OfficialPlatform_ERROR", apiResponse.Error));
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

        // ============ API 方法 ============

        public async Task<Result<ApiResponse_OfficialPlatform<AddProductResponse>>> AddProduct(AddProductRequest request)
        {
            return await CallPostAsync<AddProductRequest, AddProductRequest, ApiResponse_OfficialPlatform<AddProductResponse>>(
                "api/product/add_Product", request);
        }

        public async Task<Result<UpdateProductResponse>> UpdateProduct(UpdateProductRequest request)
        {
            return await CallPostAsync<UpdateProductRequest, UpdateProductRequest, UpdateProductResponse>(
                "api/product/update_Product", request);
        }

        public async Task<Result<AddProductOptionResponse>> AddProductOption(AddProductOptionRequest request)
        {
            return await CallGetAsync<AddProductOptionResponse>(
                $"api/product/add_ProductOption?ProductID={request.ProductID}");
        }

        public async Task<Result<UpdateProductOptionResponse>> UpdateProductOption(UpdateProductOptionRequest request)
        {
            return await CallGetAsync<UpdateProductOptionResponse>(
                $"api/product/update_ProductOption?StoreNumber={request.StoreNumber}&ProductID={request.ProductID}");
        }
    }
}