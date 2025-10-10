using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using static System.Net.WebRequestMethods;
using System.Collections;

namespace CityHubCore.Infrastructure.ServiceClient
{
    public class SrvClientBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _name;

        public SrvClientBase(HttpClient client, IConfiguration config, string name)
        {
            _httpClient = client;
            _name = name;

            _httpClient.BaseAddress = new Uri(config[$"{_name}:Uri"]);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            string userAgent = config[$"{_name}:UserAgent"];
            if (string.IsNullOrEmpty(userAgent))
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "Undefine");
            else
                _httpClient.DefaultRequestHeaders.Add("User-Agent", config[$"{_name}:UserAgent"]);

            string timeout = config[$"{_name}:Timeout"];
            if (string.IsNullOrEmpty(timeout))
            {
                _httpClient.Timeout = TimeSpan.FromSeconds(180);
            }
            else
            {
                _httpClient.Timeout = TimeSpan.FromSeconds(short.Parse(timeout));
            }
        }

        public string Uri { get { return _httpClient.BaseAddress.AbsoluteUri; } }

        public async Task<T> HttpGetAsync<T>(string uri) where T : class
        {
            var result = await _httpClient.GetAsync(uri);
            if (!result.IsSuccessStatusCode)
            {
                return null;
            }

            return await FromHttpResponseMessageAsync<T>(result);
        }

        public async Task<T> HttpPostAsync<T>(string uri, object dataToSend) where T : class
        {
            var content = ToJson(dataToSend);

            var result = await _httpClient.PostAsync(uri, content);
            if (!result.IsSuccessStatusCode)
            {

                return null;
            }

            return await FromHttpResponseMessageAsync<T>(result);
        }

        public async Task<T> HttpPostAsyncFormData<T>(string uri, object dataToSend) where T : class
        {
            var content = ToFormData(dataToSend);

            var result = await _httpClient.PostAsync(uri, content);
            if (!result.IsSuccessStatusCode)
            {

                return null;
            }

            return await FromHttpResponseMessageAsync<T>(result);
        }

        public T HttpGet<T>(string uri) where T : class
        {
            var result = _httpClient.GetAsync(uri).Result;
            if (!result.IsSuccessStatusCode)
            {
                return null;
            }

            return FromHttpResponseMessage<T>(result);
        }

        public T HttpPost<T>(string uri, object dataToSend) where T : class
        {
            var content = ToJson(dataToSend);

            var result = _httpClient.PostAsync(uri, content).Result;
            if (!result.IsSuccessStatusCode)
            {
                return null;
            }

            return FromHttpResponseMessage<T>(result);
        }
        private T FromHttpResponseMessage<T>(HttpResponseMessage result)
        {
            var resultStr = result.Content.ReadAsStringAsync().Result;
            T RET = default;
            if (resultStr is not null)
            {
                RET = JsonSerializer.Deserialize<T>(resultStr, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            return RET;
        }

        private StringContent ToJson(object obj)
        {
            return new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");
        }

        private async Task<T> FromHttpResponseMessageAsync<T>(HttpResponseMessage result)
        {
            var resultStr = await result.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(resultStr)) return default(T);

            return JsonSerializer.Deserialize<T>(resultStr, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private MultipartFormDataContent ToFormData(object obj)
        {
            MultipartFormDataContent content = new MultipartFormDataContent();
            var tmp = obj.GetType().GetProperties();
            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                var type = propertyInfo.PropertyType;
                var value = propertyInfo.GetValue(obj, null);
                var name = propertyInfo.Name;

                if (value is not null)
                {
                    if (type == typeof(IFormFile))
                    {
                        IFormFile file = (IFormFile)value;
                        content.Add(new StreamContent(file.OpenReadStream()), name, file.FileName);
                    }else if (type == typeof(List<IFormFile>))
                    {
                        List<IFormFile> file = (List<IFormFile>)value;
                        foreach(IFormFile singleFile in file)
                        {
                            content.Add(new StreamContent(singleFile.OpenReadStream()), name, singleFile.FileName);
                        }
                    }else if (type.IsGenericType && type.FullName.StartsWith("System."))
                    {
                        var items = (IEnumerable)value;
                        int index = 0;
                        foreach (var item in items)
                        {
                            var childValue = item == null ? "" : item.ToString();
                            content.Add(new StringContent(childValue), $"{name}[{index}]");
                            index++;
                        }
                    }
                    else if(type.IsClass&& !type.FullName.StartsWith("System.") ){

                        foreach (var prop in value.GetType().GetProperties()){
                            var childValue = prop.GetValue(value, null)== null? "": prop.GetValue(value, null);
                            var childName = prop.Name;
                            content.Add(new StringContent(childValue.ToString()), $"{name}[{childName}]");
                        }
                    }
                    else
                    {
                        String text = value.ToString();
                        content.Add(new StringContent(text), name);
                    }
                }
            }
            return content;
        }
    }
}
