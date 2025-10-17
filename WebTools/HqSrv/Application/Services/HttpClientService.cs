using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using POVWebDomain.Models.DB.POVWeb;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Linq;
using System.Threading;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using POVWebDomain.Models.ExternalApi;
using System.Security;
using System.Xml.Linq;
using System.Net;
using System.Reflection.PortableExecutable;

namespace HqSrv.Application.Services
{
    public class HttpClientService
    {
        private readonly System.Net.Http.IHttpClientFactory _httpClientFactory;

        public HttpClientService(System.Net.Http.IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<T?> GetAsync<T>(string baseUrl, string endpoint, Dictionary<string, string>? headers = null)
        {

            var httpClient = _httpClientFactory.CreateClient();
            var baseUri = new Uri(baseUrl); // e.g. "https://example.com/api/"
            var fullUri = new Uri(baseUri, endpoint); // e.g. "products/123"
            var request = new HttpRequestMessage(HttpMethod.Get, fullUri);
            AddCustomHeaders(request, headers);

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            return JsonSerializer.Deserialize<T>(content, options);
        }


        public async Task<TResponse?> PostAsyncQueryString<TRequest, TResponse>(
            string baseUrl,
            string endpoint,
            TRequest data,
            Dictionary<string, string>? queryParams = null)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                // Serialize body data
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // 組 base URL + endpoint
                var baseUri = new Uri(baseUrl); // e.g. "https://example.com/api/"
                var uriBuilder = new UriBuilder(new Uri(baseUri, endpoint));

                // 加入 query string（如果有）
                if (queryParams != null && queryParams.Any())
                {
                    var query = string.Join("&", queryParams.Select(kvp =>
                        $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
                    uriBuilder.Query = query;
                }

                var request = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
                {
                    Content = content
                };

                var response = await httpClient.SendAsync(request);

                var result = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<TResponse>(result, options);
            }
            catch (HttpRequestException)
            {
                return default;
            }
        }

        

        private void AddCustomHeaders(HttpRequestMessage request, Dictionary<string, string>? headers)
        {
            if (headers == null) return;

            foreach (var kv in headers)
            {
                request.Headers.Add(kv.Key, kv.Value);
            }
        }

        public async Task<TResponse?> PostMultipartAsync<TResponse>(string baseUrl, string endpoint, MultipartFormDataContent content, Dictionary<string, string>? headers = null)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var baseUri = new Uri(baseUrl);
                var fullUri = new Uri(baseUri, endpoint);
                var request = new HttpRequestMessage(HttpMethod.Post, fullUri)
                {
                    Content = content
                };

                AddCustomHeaders(request, headers);
                var response = await httpClient.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<TResponse>(result, options);
            }
            catch (HttpRequestException)
            {
                throw;
            }
        }

        public async Task<TResponse?> PostAsyncWithOptions<TRequest, TResponse>(string baseUrl, string endpoint, TRequest data, System.Text.Json.JsonSerializerOptions jsonOptions, Dictionary<string, string>? headers = null)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                string json;
                if (data is string str)
                {
                    json = str;
                }
                else
                {
                    json = System.Text.Json.JsonSerializer.Serialize(data, jsonOptions);
                }
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var baseUri = new Uri(baseUrl);
                var fullUri = new Uri(baseUri, endpoint);
                var request = new HttpRequestMessage(HttpMethod.Post, fullUri)
                {
                    Content = content
                };

                AddCustomHeaders(request, headers);
                var response = await httpClient.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {

                    throw new HttpRequestException($"API 請求失敗: {response.StatusCode} - {response.ReasonPhrase}\n內容: {result}");
                }

                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return System.Text.Json.JsonSerializer.Deserialize<TResponse>(result, options);
            }
            catch (HttpRequestException)
            {
                throw;
            }
        }

    }
}
