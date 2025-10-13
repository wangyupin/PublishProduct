using HqSrv.Application.Services;
using HqSrv.Application.Services.ApiKey;
using Microsoft.Extensions.Configuration;
using POVWebDomain.Common;
using POVWebDomain.Models.DB.POVWeb;
using POVWebDomain.Models.ExternalApi.OfficialWebsite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

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

        // 上傳主圖 API
        public async Task<Result<ApiResponse_OfficialPlatform<string>>> UpdateProductImage(UpdateProductImageRequest request)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(request.StoreNumber.ToString()), "StoreNumber");
            content.Add(new StringContent(request.ProductID.ToString()), "ProductID");

            for (int idx = 0; idx < request.Data?.Count; idx++)
            {
                var imageData = request.Data[idx];

                // 加入 DisplayOrder
                content.Add(new StringContent(imageData.DisplayOrder.ToString()), $"Data[{idx}].DisplayOrder");

                // 加入圖片檔案 - 關鍵修改在這裡!
                if (imageData.ImageFile != null && imageData.ImageFile.Length > 0)
                {
                    // 方式1: 先複製到 MemoryStream (推薦)
                    var memoryStream = new MemoryStream();
                    await imageData.ImageFile.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    var streamContent = new StreamContent(memoryStream);
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                    // 注意: Postman 用完整路徑當 filename,但實際上用檔名就可以
                    content.Add(streamContent, $"Data[{idx}].ImageFile", imageData.ImageFile.FileName);
                }
            }

            return await CallMultipartAsync<ApiResponse_OfficialPlatform<string>>(
                "api/product/Update_ProductOption_Image", content);
        }

        // 上傳選項圖片 API
        public async Task<Result<ApiResponse_OfficialPlatform<string>>> UpdateProductOptionImage(UpdateProductOptionImageRequest request)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(request.StoreNumber.ToString()), "StoreNumber");

            for (int idx = 0; idx < request.Data?.Count; idx++)
            {
                var imageData = request.Data[idx];

                // 加入 SkuID
                content.Add(new StringContent(imageData.SkuID.ToString()), $"Data[{idx}].SkuID");

                // 加入 DisplayOrder
                content.Add(new StringContent(imageData.DisplayOrder.ToString()), $"Data[{idx}].DisplayOrder");

                // 加入圖片檔案
                if (imageData.ImageFile != null && imageData.ImageFile.Length > 0)
                {
                    var memoryStream = new MemoryStream();
                    await imageData.ImageFile.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    var streamContent = new StreamContent(memoryStream);
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                    content.Add(streamContent, $"Data[{idx}].ImageFile", imageData.ImageFile.FileName);
                }
            }

            return await CallMultipartAsync<ApiResponse_OfficialPlatform<string>>(
                "api/product/Update_ProductOption_Image", content);
        }

        public async Task<Result<RemoveProductResponse>> RemoveProduct(RemoveProductRequest request)
        {
            return await CallPostAsync<RemoveProductRequest, RemoveProductRequest, RemoveProductResponse>(
                "api/Product/remove_product", request);
        }
    }
}