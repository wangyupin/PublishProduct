using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using System;
using POVWebDomain.Models.DB.POVWeb;
using Dapper;
using POVWebDomain.Models.ExternalApi.Momo;
using Microsoft.Extensions.Configuration;
using POVWebDomain.Models.ExternalApi;
using POVWebDomain.Models.ExternalApi.ShopeeSCM;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Buffers.Text;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Azure.Core;
using Microsoft.Data.SqlClient;

namespace HqSrv.Application.Services.ApiKey
{
    public class ApiKeyProvider
    {
        private readonly IMemoryCache _cache;
        private readonly POVWebDbContextDapper _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClientService _httpClientService;

        public ApiKeyProvider(IMemoryCache cache, POVWebDbContextDapper context, IConfiguration configuration, HttpClientService httpClientService)
        {
            _cache = cache;
            _context = context;
            _configuration = configuration;
            _httpClientService = httpClientService;
        }

        public async Task<string> GetApiKeyStoreIDAsync(string platformID)
        {
            string sql = @"select StoreID
                            from EcommerceStore
                            where OldID = @PlatformID";

            return await _cache.GetOrCreateAsync($"91ApiKey{platformID}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30); // 快取 30 分鐘
                string storeID = await _context.Connection.QueryFirstOrDefaultAsync<string>(sql, new { PlatformID = platformID });

                return storeID ?? throw new Exception("API key not found.");
            });
        }

        public async Task<string> GetApiKeyAsync(string storeID, string platform)
        {
            string sql = "SELECT ApiKey FROM EcommerceStore WHERE StoreID = @StoreID";
            
            return await _cache.GetOrCreateAsync($"{platform}ApiKey{storeID}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30); // 快取 30 分鐘
                string key = await _context.Connection.QueryFirstOrDefaultAsync<string>(sql, new { StoreID = storeID });

                return key ?? throw new Exception("API key not found.");
            });
        }
    }
}
