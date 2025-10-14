using HqSrv.Application.Services;
using HqSrv.Application.Services.ApiKey;
using Microsoft.Extensions.Configuration;
using POVWebDomain.Common;
using POVWebDomain.Models.API.StoreSrv.Common;
using POVWebDomain.Models.DB.POVWeb;
using POVWebDomain.Models.ExternalApi.OfficialFront;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace HqSrv.Infrastructure.ExternalServices
{
    public class ApiResponse_OfficialFront<T>
    {
        public string Error { get; set; }
        public T Data { get; set; }
    }

    public class OfficialFrontExternalApiService : BaseExternalApiService
    {
        private readonly string _endPoint;
        public override string EndPoint => _endPoint;
        private string _storeNumber;

        public OfficialFrontExternalApiService(
            POVWebDbContextDapper context,
            IConfiguration configuration,
            HttpClientService httpClient,
            ApiKeyProvider apiKeyProvider)
            : base(context, configuration, httpClient, apiKeyProvider)
        {
            _endPoint = "https://onlineshop.gbtech.com.tw/";
        }

        protected override async Task<Dictionary<string, string>> GetHeadersAsync(string endPoint)
        {
            string apiKey = await _apiKeyProvider.GetApiKeyAsync(_storeNumber, "OfficialFront");

            return new Dictionary<string, string>
            {
                { "x-api-key", apiKey }
            };
        }

        protected override Result<T> ProcessResponse<T>(T response)
        {
            if (response is ApiResponse_OfficialFront<object> apiResponse)
            {
                if (!string.IsNullOrEmpty(apiResponse.Error))
                {
                    return Result<T>.Failure(Error.Custom("OfficialFront_ERROR", apiResponse.Error));
                }
            }

            return Result<T>.Success(response);
        }

        public void Configure(int? storeNumber)
        {
            if (!storeNumber.HasValue)
            {
                throw new ArgumentException("storeNumber 不能為空", nameof(storeNumber));
            }
            _storeNumber = storeNumber.ToString();
        }

        // ============ API 方法 ============

        public async Task<Result<dynamic>> GetCategory(GetCategoryRequest request)
        {
            Configure(request.StoreNumber);
            var result = await CallGetAsync<ApiResponse_OfficialFront<List<GetCategoryResponse>>>(
                "api/Front/v1/product/get_Category", request);

            if (result.IsFailure)
                return Result<dynamic>.Failure(result.Error);

            var responseBody = result.Data.Data
                .Where(c => c.Active)
                .Select(c => c.ToMultipleLayerOption())
                .ToList();

            return Result<dynamic>.Success(new { responseBody });

        }
    }
}