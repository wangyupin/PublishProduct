using Microsoft.Extensions.Configuration;
using POVWebDomain.Models.DB.POVWeb;
using POVWebDomain.Common;  // 新增
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using HqSrv.Application.Services.ApiKey;
using HqSrv.Application.Services;

namespace HqSrv.Infrastructure.ExternalServices
{
    public abstract class BaseExternalApiService
    {
        protected readonly POVWebDbContextDapper _context;
        protected readonly IConfiguration _configuration;
        protected readonly HttpClientService _httpClient;
        protected readonly ApiKeyProvider _apiKeyProvider;

        public abstract string EndPoint { get; }

        protected BaseExternalApiService(
            POVWebDbContextDapper context,
            IConfiguration configuration,
            HttpClientService httpClient,
            ApiKeyProvider apiKeyProvider)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = httpClient;
            _apiKeyProvider = apiKeyProvider;
        }

        // 抽象方法:子類別實作錯誤處理邏輯
        protected abstract Result<T> ProcessResponse<T>(T response) where T : class;
        protected abstract Task<Dictionary<string, string>> GetHeadersAsync(string endPoint = "");

        // 核心方法:統一的 API 呼叫邏輯
        protected virtual JsonSerializerOptions GetJsonSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        protected virtual Task<TFinalRequest> ProcessRequestAsync<TOriginalRequest, TFinalRequest>(TOriginalRequest request)
        {
            return Task.FromResult((TFinalRequest)(object)request);
        }

        // GET 方法
        protected async Task<Result<T>> CallGetAsync<T>(string endpoint) where T : class
        {
            try
            {
                var headers = await GetHeadersAsync(endpoint);
                var response = await _httpClient.GetAsync<T>(EndPoint, endpoint, headers);

                if (response == null)
                {
                    return Result<T>.Failure(
                        Error.Custom("API_NULL_RESPONSE", "API 回應為空")
                    );
                }

                return ProcessResponse(response);
            }
            catch (Exception ex)
            {
                return Result<T>.Failure(
                    Error.Custom("API_EXCEPTION", $"API 呼叫異常: {ex.Message}")
                );
            }
        }

        // POST 方法 (JSON Body)
        protected async Task<Result<TResponse>> CallPostAsync<TOriginalRequest, TFinalRequest, TResponse>(
            string endpoint,
            TOriginalRequest request)
            where TResponse : class
        {
            try
            {
                var headers = await GetHeadersAsync(endpoint);
                var processedRequest = await ProcessRequestAsync<TOriginalRequest, TFinalRequest>(request);
                var jsonOptions = GetJsonSerializerOptions();

                var response = await _httpClient.PostAsyncWithOptions<TFinalRequest, TResponse>(
                    EndPoint, endpoint, processedRequest, jsonOptions, headers);

                if (response == null)
                {
                    return Result<TResponse>.Failure(
                        Error.Custom("API_NULL_RESPONSE", "API 回應為空")
                    );
                }

                return ProcessResponse(response);
            }
            catch (Exception ex)
            {
                return Result<TResponse>.Failure(
                    Error.Custom("API_EXCEPTION", $"API 呼叫異常: {ex.Message}")
                );
            }
        }

        // POST 方法 (Multipart Form Data) - 用於上傳檔案
        protected async Task<Result<T>> CallMultipartAsync<T>(
            string endpoint,
            MultipartFormDataContent content)
            where T : class
        {
            try
            {
                var headers = await GetHeadersAsync(endpoint);
                var response = await _httpClient.PostMultipartAsync<T>(EndPoint, endpoint, content, headers);

                if (response == null)
                {
                    return Result<T>.Failure(
                        Error.Custom("API_NULL_RESPONSE", "API 回應為空")
                    );
                }

                return ProcessResponse(response);
            }
            catch (Exception ex)
            {
                return Result<T>.Failure(
                    Error.Custom("API_EXCEPTION", $"API 呼叫異常: {ex.Message}")
                );
            }
        }
    }
}