using Microsoft.Extensions.Configuration;
using POVWebDomain.Models.DB.POVWeb;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Net.Http.Json;
using System.Text.Json;
using HqSrv.Application.Services.ApiKey;
using Azure.Core;
using System.Reflection;
using System.Collections;
using System.Linq;

namespace HqSrv.Application.Services.ExternalApiServices
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

        protected abstract (T result, string errorMessage) ProcessResponse<T>(T response);
        protected abstract Task<Dictionary<string, string>> GetHeadersAsync(string endPoint = "");
        protected async Task<(T result, string errorMessage)> CallGetAsync<T>(string endpoint)
        {
            try
            {
                var headers = await GetHeadersAsync();
                var response = await _httpClient.GetAsync<T>(EndPoint, endpoint, headers);
                return ProcessResponse(response);
            }
            catch (Exception ex)
            {
                return (default(T), ex.Message);
            }
        }

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


        protected async Task<(TResponse result, string errorMessage)> CallPostAsync<TOriginalRequest, TFinalRequest, TResponse>(
            string endpoint, TOriginalRequest request)
        {
            try
            {
                var headers = await GetHeadersAsync(endpoint);
                var processedRequest = await ProcessRequestAsync<TOriginalRequest, TFinalRequest>(request);
                var jsonOptions = GetJsonSerializerOptions();
                var response = await _httpClient.PostAsyncWithOptions<TFinalRequest, TResponse>(EndPoint, endpoint, processedRequest, jsonOptions, headers);
                return ProcessResponse(response);
            }
            catch (Exception ex)
            {
                return (default(TResponse), ex.Message);
            }
        }

        protected async Task<(TResponse result, string errorMessage)> CallPostAsyncWithOptions<TOriginalRequest, TFinalRequest, TResponse>(
            string endpoint, TOriginalRequest request, JsonSerializerOptions customJsonOptions)
        {
            try
            {
                var headers = await GetHeadersAsync(endpoint);
                var processedRequest = await ProcessRequestAsync<TOriginalRequest, TFinalRequest>(request);
                var response = await _httpClient.PostAsyncWithOptions<TFinalRequest, TResponse>(
                    EndPoint, endpoint, processedRequest, customJsonOptions, headers);
                return ProcessResponse(response);
            }
            catch (Exception ex)
            {
                return (default(TResponse), ex.Message);
            }
        }

        protected async Task<(T result, string errorMessage)> CallMultipartAsync<T>(
            string endpoint, MultipartFormDataContent content)
        {
            try
            {
                var headers = await GetHeadersAsync();
                var response = await _httpClient.PostMultipartAsync<T>(EndPoint, endpoint, content, headers);
                return ProcessResponse(response);
            }
            catch (Exception ex)
            {
                return (default(T), ex.Message);
            }
        }

        protected async Task<(TResponse result, string errorMessage)> CallGetAsync<TOriginalRequest, TFinalRequest, TResponse>(
            string endpoint, TOriginalRequest request)
        {
            try
            {
                var headers = await GetHeadersAsync(endpoint);
                var processedRequest = await ProcessRequestAsync<TOriginalRequest, TFinalRequest>(request);
                var jsonOptions = GetJsonSerializerOptions();
                var response = await _httpClient.GetAsyncWithOptions<TFinalRequest, TResponse>(
                    EndPoint, endpoint, processedRequest, jsonOptions, headers);
                return ProcessResponse(response);
            }
            catch (Exception ex)
            {
                return (default(TResponse), ex.Message);
            }
        }

        protected async Task<(TResponse result, string errorMessage)> CallGetAsyncWithQueryParams<TOriginalRequest, TFinalRequest, TResponse>(
        string endpoint, TOriginalRequest request)
        {
            try
            {
                var headers = await GetHeadersAsync(endpoint);
                var processedRequest = await ProcessRequestAsync<TOriginalRequest, TFinalRequest>(request);

                var requestParams = ConvertObjectToDictionary(processedRequest);
                foreach (var param in requestParams)
                {
                    headers[param.Key] = param.Value;
                }

                var response = await _httpClient.GetAsyncQueryString<TResponse>(
                    EndPoint, endpoint, headers);
                return ProcessResponse(response);
            }
            catch (Exception ex)
            {
                return (default(TResponse), ex.Message);
            }
        }

        protected async Task<(TResponse result, string errorMessage)> CallPostAsyncWithQueryParams<TOriginalRequest, TResponse>(string endpoint, TOriginalRequest request)
        {
            try
            {
                var headers = await GetHeadersAsync(endpoint);
              

                var response = await _httpClient.PostAsyncQueryString<TOriginalRequest, TResponse>(
                    EndPoint, endpoint, request, headers);
                return ProcessResponse(response);
            }
            catch (Exception ex)
            {
                return (default(TResponse), ex.Message);
            }
        }

        protected async Task<(T result, string errorMessage)> CallMultipartAsyncWithQueryParams<T>(string endpoint, MultipartFormDataContent content)
        {
            try
            {
                var headers = await GetHeadersAsync(endpoint);
                var response = await _httpClient.PostMultipartAsyncQueryString<T>(EndPoint, endpoint, content, headers);
                return ProcessResponse(response);
            }
            catch (Exception ex)
            {
                return (default(T), ex.Message);
            }
        }

        private Dictionary<string, string> ConvertObjectToDictionary<T>(T obj)
        {
            var result = new Dictionary<string, string>();

            if (obj == null) return result;

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var value = property.GetValue(obj);
                if (value != null)
                {
                    string stringValue;

                    // 檢查是否為集合型態（但排除字串）
                    if (value is IEnumerable enumerable && !(value is string))
                    {
                        stringValue = string.Join(",", enumerable.Cast<object>().Select(x => x?.ToString()));
                    }
                    else
                    {
                        stringValue = value.ToString();
                    }

                    result[property.Name.ToLower()] = stringValue;
                }
            }

            return result;
        }

    }
}
