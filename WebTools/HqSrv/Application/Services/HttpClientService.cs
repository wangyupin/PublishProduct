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

        public async Task<T?> GetAsyncQueryString<T>(
                                            string baseUrl,
                                            string endpoint,
                                            Dictionary<string, string>? queryParams = null)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var httpClient = _httpClientFactory.CreateClient();

            // 組合 base URL 和 endpoint
            var baseUri = new Uri(baseUrl);             // e.g. https://example.com/api/
            var uriBuilder = new UriBuilder(new Uri(baseUri, endpoint)); // e.g. products/123

            // 加上 Query String（如果有）
            if (queryParams != null && queryParams.Count > 0)
            {
                var query = string.Join("&", queryParams.Select(kvp =>
                    $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
                uriBuilder.Query = query;
            }

            var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(content, options);
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(
            string baseUrl,
            string endpoint,
            TRequest data,
            Dictionary<string, string>? headers = null)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var httpClient = _httpClientFactory.CreateClient();
                var json = JsonSerializer.Serialize(data, options);
             
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var baseUri = new Uri(baseUrl); // e.g. "https://example.com/api/"
                var fullUri = new Uri(baseUri, endpoint); // e.g. "products/123"
                var request = new HttpRequestMessage(HttpMethod.Post, fullUri)
                {
                    Content = content
                };

                AddCustomHeaders(request, headers);

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();

                
                return JsonSerializer.Deserialize<TResponse>(result, options);
            }
            catch (HttpRequestException ex)
            {
                return default;
            }
        
        }

        public async Task<string?> PostAsyncYahoo<TRequest, TResponse>(
         string baseUrl,
         string endpoint,
         string data,
         Dictionary<string, string>? headers = null)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var content = new StringContent(data, Encoding.UTF8, "application/json");

                var baseUri = new Uri(baseUrl); // e.g. "https://example.com/api/"
                var fullUri = new Uri(baseUri, endpoint); // e.g. "products/123"
                var request = new HttpRequestMessage(HttpMethod.Post, fullUri)
                {
                    Content = content
                };

                AddCustomHeaders(request, headers);

                var response = await httpClient.SendAsync(request);
                //response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();

                return result;
            }
            catch (HttpRequestException ex)
            {
                return default;
            }

        }

        public async Task<TResponse?> PostAsyncTCatTimeOut<TRequest, TResponse>(
           string baseUrl,
           string endpoint,
           TRequest data,
           Dictionary<string, string>? headers = null)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var httpClient = _httpClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromSeconds(130);
                var json = JsonSerializer.Serialize(data, options);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var baseUri = new Uri(baseUrl); // e.g. "https://example.com/api/"
                var fullUri = new Uri(baseUri, endpoint); // e.g. "products/123"
                var request = new HttpRequestMessage(HttpMethod.Post, fullUri)
                {
                    Content = content
                };

                AddCustomHeaders(request, headers);

                var response = await httpClient.SendAsync(request);

                var result = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<TResponse>(result, options);
            }
            catch (HttpRequestException ex)
            {
                return default;
            }

        }
        public async Task<TResponse?> PostAsyncHCTXMLAddress<TRequest, TResponse>(
           string baseUrl,
           TRequest data,
           Dictionary<string, string>? headers = null)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var httpClient = _httpClientFactory.CreateClient();
                var json = JsonSerializer.Serialize(data, options);
                var jsonEscape = SecurityElement.Escape(json);
                string soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
                                <soap12:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                                                 xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                                                 xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"">
                                  <soap12:Body>
                                    <addrCompare_Json xmlns=""http://tempuri.org/"">
                                      <Json>{jsonEscape}</Json>
                                    </addrCompare_Json>
                                  </soap12:Body>
                                </soap12:Envelope>";

                var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");

                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, baseUrl);
                req.Content = content;

                var response = await httpClient.SendAsync(req);

                var xmlResponse = await response.Content.ReadAsStringAsync();

                // 1. 先 parse XML
                var xdoc = XDocument.Parse(xmlResponse);

                // 2. 使用 namespace 定位節點
                XNamespace ns = "http://tempuri.org/";
                var jsonResultText = xdoc
                    .Descendants(ns + "addrCompare_JsonResult")
                    .FirstOrDefault()
                    ?.Value;

                return JsonSerializer.Deserialize<TResponse>(jsonResultText, options);
            }
            catch (HttpRequestException ex)
            {
                return default;
            }

        }

        public async Task<TResponse?> PostAsyncHCTXML<TRequest, TResponse>(
           string baseUrl,
           TRequest data,
           HCTKey hCTKey,
           Dictionary<string, string>? headers = null)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var httpClient = _httpClientFactory.CreateClient();
                var json = JsonSerializer.Serialize(data, options);
                var jsonEscape = SecurityElement.Escape(json);
                string soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
                                <soap12:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                                                 xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                                                 xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope"">
                                  <soap12:Body>
                                    <TransData_Json xmlns=""http://tempuri.org/"">
                                      <company>{hCTKey.Company}</company>
                                      <password>{hCTKey.Password}</password>
                                      <json>{jsonEscape}</json>
                                    </TransData_Json>
                                  </soap12:Body>
                                </soap12:Envelope>";

                var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");

                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, baseUrl);
                req.Content = content;

                var response = await httpClient.SendAsync(req);

                var xmlResponse = await response.Content.ReadAsStringAsync();

                // 1. 先 parse XML
                var xdoc = XDocument.Parse(xmlResponse);

                // 2. 使用 namespace 定位節點
                XNamespace ns = "http://tempuri.org/";
                var jsonResultText = xdoc
                    .Descendants(ns + "TransData_JsonResult")
                    .FirstOrDefault()
                    ?.Value;

                return JsonSerializer.Deserialize<TResponse>(jsonResultText, options);
            }
            catch (HttpRequestException ex)
            {
                return default;
            }

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

        public async Task<TResponse?> PostMultipartAsyncQueryString<TResponse>(
            string baseUrl,
            string endpoint,
            MultipartFormDataContent content,
            Dictionary<string, string>? queryParams = null)
        {
            try
            {
               
                var httpClient = _httpClientFactory.CreateClient();

                var baseUri = new Uri(baseUrl); // e.g. "https://example.com/api/"
                var uriBuilder = new UriBuilder(new Uri(baseUri, endpoint));

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


        public async Task<TResponse?> GetAsyncWithOptions<TRequest, TResponse>(string baseUrl, string endpoint, TRequest data, System.Text.Json.JsonSerializerOptions jsonOptions, Dictionary<string, string>? headers = null)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                var baseUri = new Uri(baseUrl);
                var fullUri = new Uri(baseUri, endpoint);


                if (data != null)
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(data, jsonOptions);
                    var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                    if (dict?.Any() == true)
                    {
                        var queryParams = dict
                            .Where(kvp => kvp.Value != null)
                            .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value.ToString())}");

                        var queryString = string.Join("&", queryParams);
                        var uriBuilder = new UriBuilder(fullUri);
                        uriBuilder.Query = queryString;
                        fullUri = uriBuilder.Uri;
                    }
                }

                var request = new HttpRequestMessage(HttpMethod.Get, fullUri);
                AddCustomHeaders(request, headers);


                var response = await httpClient.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();

                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return System.Text.Json.JsonSerializer.Deserialize<TResponse>(result, options);
            }
            catch (HttpRequestException)
            {
                return default;
            }
        }

        // 新增：使用自訂 HttpClient 的 POST 方法
        public async Task<TResponse?> PostAsyncWithOptions<TRequest, TResponse>(
            HttpClient httpClient,
            string endpoint,
            TRequest data,
            System.Text.Json.JsonSerializerOptions jsonOptions,
            Dictionary<string, string>? headers = null)
        {
            try
            {
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
                var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
                {
                    Content = content
                };
                AddCustomHeaders(request, headers);
                var response = await httpClient.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(result) || typeof(TResponse) == typeof(object))
                {
                    return default(TResponse);
                }

                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return System.Text.Json.JsonSerializer.Deserialize<TResponse>(result, options);
            }
            catch (HttpRequestException)
            {
                return default;
            }
        }

        // 新增：使用自訂 HttpClient 的 GET 方法
        public async Task<TResponse?> GetAsyncWithOptions<TRequest, TResponse>(
            HttpClient httpClient,
            string endpoint,
            TRequest data,
            System.Text.Json.JsonSerializerOptions jsonOptions,
            Dictionary<string, string>? headers = null)
        {
            try
            {
                var fullUri = new Uri(httpClient.BaseAddress, endpoint);
                if (data != null)
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(data, jsonOptions);
                    var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                    if (dict?.Any() == true)
                    {
                        var queryParams = dict
                            .Where(kvp => kvp.Value != null)
                            .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value.ToString())}");
                        var queryString = string.Join("&", queryParams);
                        var uriBuilder = new UriBuilder(fullUri);
                        uriBuilder.Query = queryString;
                        fullUri = uriBuilder.Uri;
                    }
                }
                var request = new HttpRequestMessage(HttpMethod.Get, fullUri);
                AddCustomHeaders(request, headers);

                var response = await httpClient.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return System.Text.Json.JsonSerializer.Deserialize<TResponse>(result, options);
            }
            catch (HttpRequestException)
            {
                return default;
            }
        }

        // 建立有 Cookie 支援的 HttpClient
        public HttpClient CreateHttpClientWithCookies(CookieContainer cookieContainer = null)
        {
            var container = cookieContainer ?? new CookieContainer();
            var handler = new HttpClientHandler()
            {
                CookieContainer = container,
                UseCookies = true
            };

            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.UserAgent.ParseAdd("YourAppName/1.0");
            client.Timeout = new TimeSpan(0, 2, 0);

            return client;
        }


    }
}
