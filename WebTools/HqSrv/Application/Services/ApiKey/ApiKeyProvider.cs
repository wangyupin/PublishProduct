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

        public async Task<string> GetApiKeyAsync(string storeID)
        {
            string sql = "SELECT ApiKey FROM EcommerceStore WHERE StoreID = @StoreID";
            
            return await _cache.GetOrCreateAsync($"91ApiKey{storeID}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30); // 快取 30 分鐘
                string key = await _context.Connection.QueryFirstOrDefaultAsync<string>(sql, new { StoreID = storeID });

                return key ?? throw new Exception("API key not found.");
            });
        }

        public async Task<MomoLoginInfo> GetMomoApiKeyAsync(string platformID)
        {
            string sql = @"SELECT StoreID, ApiKey, AccessToken, KeyValue 
                           FROM EcommerceStore WHERE OldID = @PlatformID";
            return await _cache.GetOrCreateAsync($"MomoApiKey{platformID}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30); // 快取 30 分鐘
                var config = await _context.Connection.QueryFirstOrDefaultAsync(sql, new { PlatformID = platformID });
                if (config == null)
                {
                    throw new Exception("API key not found.");
                }
                var loginInfo = new MomoLoginInfo
                {
                    EntpCode = config.StoreID,
                    EntpID = config.ApiKey,
                    EntpPwd = config.KeyValue,
                    OtpBackNo = config.AccessToken
                };

                return loginInfo;
            });
        }

        public async Task<YahooApiKey> GetYahooApiKeyAsync(string platformID)
        {
            string sql = @"SELECT StoreID, ApiKey, AccessToken, KeyValue, Scope 
                           FROM EcommerceStore WHERE OldID = @PlatformID";

            var config = await _context.Connection.QueryFirstOrDefaultAsync(sql, new { PlatformID = platformID });

            if (config == null)
                throw new Exception($"API key not found for platformID: {platformID}");

            if (!_cache.TryGetValue($"YahooApiKey{platformID}", out YahooApiKey key))
            {
                key = new YahooApiKey()
                {
                    Token = config.AccessToken,
                    KeyValue = config.ApiKey,
                    KeyIV = config.StoreID,
                    KeyVersion = config.Scope,
                    SaltKey = config.KeyValue
                };

                // 設定快取
                _cache.Set($"YahooApiKey{platformID}", key, TimeSpan.FromMinutes(30));
            }

            return key;
        }

        public async Task<ShopeeApiKey> GetShopeeApiKeyAsync(string platformID)
        {

            string sql = @"SELECT StoreID, ApiKey, Accesstoken, Refreshtoken, Scope, KeyValue
                           FROM EcommerceStore WHERE OldID = @PlatformID";


            return await _cache.GetOrCreateAsync($"ShopeeApiKey{platformID}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10); // 快取 10 分鐘

                var config = await _context.Connection.QueryFirstOrDefaultAsync<ShopeeApiKeyEntity>(sql, new { PlatformID = platformID });
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                long tenMinutes = 60 * 10;
                var key = new ShopeeApiKey();
                if (config?.KeyValue - tenMinutes < timestamp || config?.KeyValue == null)
                {
                    string baseUri = @"https://openplatform.shopee.cn/";
                    string endpoint = @"/api/v2/auth/access_token/get";
                    string partnerkey = config.Scope;
                    //以 Public APIs：Partner_id、API 路徑、Timestamp
                    string signStr = $@"{config.ApiKey}{endpoint}{timestamp}";
                    string sign = signStr.HmacSha256(partnerkey);

                    var reqbody = new ShopeeRefreshToken()
                    {
                        Partner_id = config.ApiKey,
                        Refresh_token = config.Refreshtoken,
                        Shop_id = config.StoreID
                    };
                    var paramDic = new Dictionary<string, string>()
                    {
                        {"partner_id", config.ApiKey.ToString()},
                        {"timestamp", timestamp.ToString()},
                        {"sign", sign}
                    };

                    var result = await _httpClientService.PostAsyncQueryString<ShopeeRefreshToken, ShopeeRefreshTokenResponse>(baseUri, endpoint.TrimStart('/'), reqbody, paramDic);

                    string shopeeTokenUpdateSql = @"
                              Update EcommerceStore
                              set Refreshtoken = @Refreshtoken, AccessToken = @AccessToken, KeyValue = @Expire
                              where OldID = @PlatformID 
                        ";
                    if(result != null && string.IsNullOrEmpty(result.Error) )
                    {
                        long expire = result.Expire_in + timestamp;
                        var param = new DynamicParameters();
                        param.Add("@PlatformID", platformID);
                        param.Add("@Refreshtoken", result.Refresh_token);
                        param.Add("@AccessToken", result.Access_token);
                        param.Add("@Expire", expire);

                        using (var connection = _context.Connection)
                        {
                            connection.Open();
                            using var transaction = connection.BeginTransaction();
                            try
                            {
                                var updateresult = await connection.ExecuteAsync(
                                    shopeeTokenUpdateSql,
                                    param,
                                    transaction: transaction,
                                    commandTimeout: 180);

                                transaction.Commit();
                            }
                            catch (SqlException e)
                            {
                                throw new Exception();
                            }
                        }

                        key.Partner_id = config.ApiKey;
                        key.Access_token = result.Access_token;
                        key.Shop_id = config.StoreID;
                        key.Partner_key = config.Scope;
                    }
                }
                else
                {
                    key.Partner_id = config.ApiKey;
                    key.Access_token = config.Accesstoken;
                    key.Shop_id = config.StoreID;
                    key.Partner_key = config.Scope;
                }

                return key ?? throw new Exception("API key not found.");
            });
        }

        public async Task<ShopeeApiKey> GetShopeeApiKeyAsync_V2(string platformID)
        {
            string sql = @"SELECT StoreID, ApiKey, Accesstoken, Refreshtoken, Scope, KeyValue, AccessCode
                   FROM EcommerceStore WHERE OldID = @PlatformID";

            return await _cache.GetOrCreateAsync($"ShopeeApiKey{platformID}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

                var config = await _context.Connection.QueryFirstOrDefaultAsync<ShopeeApiKeyEntity>(sql, new { PlatformID = platformID });
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                long tenMinutes = 60 * 10;
                var key = new ShopeeApiKey();

                // 檢查 token 是否過期或不存在
                if (config?.KeyValue - tenMinutes < timestamp || config?.KeyValue == null)
                {
                    string baseUri = @"https://openplatform.sandbox.test-stable.shopee.cn/";
                    string endpoint;
                    object reqbody;

                    // 決定使用 refresh_token 還是 access_code
                    bool useRefreshToken = !string.IsNullOrEmpty(config?.Refreshtoken);

                    if (useRefreshToken)
                    {
                        // 使用 refresh_token 刷新
                        endpoint = @"/api/v2/auth/access_token/get";
                        reqbody = new ShopeeRefreshToken()
                        {
                            Partner_id = config.ApiKey,
                            Refresh_token = config.Refreshtoken,
                            Shop_id = config.StoreID
                        };
                    }
                    else
                    {
                        // 使用 access_code 首次取得
                        endpoint = @"/api/v2/auth/token/get";
                        reqbody = new
                        {
                            code = config.AccessCode.ToString(),
                            shop_id = config.StoreID,
                            partner_id = config.ApiKey
                        };
                    }

                    string partnerkey = config.Scope;
                    string signStr = $@"{config.ApiKey}{endpoint}{timestamp}";
                    string sign = signStr.HmacSha256(partnerkey).ToLower();

                    var paramDic = new Dictionary<string, string>()
            {
                {"partner_id", config.ApiKey.ToString()},
                {"timestamp", timestamp.ToString()},
                {"sign", sign}
            };

                    var result = await _httpClientService.PostAsyncQueryString<object, ShopeeRefreshTokenResponse>(baseUri, endpoint.TrimStart('/'), reqbody, paramDic);

                    // 統一的資料庫更新邏輯
                    if (result != null && string.IsNullOrEmpty(result.Error))
                    {
                        await UpdateTokenInDatabase(platformID, result, timestamp);

                        key.Partner_id = config.ApiKey;
                        key.Access_token = result.Access_token;
                        key.Shop_id = config.StoreID;
                        key.Partner_key = config.Scope;
                    }
                }
                else
                {
                    // Token 還有效，直接使用
                    key.Partner_id = config.ApiKey;
                    key.Access_token = config.Accesstoken;
                    key.Shop_id = config.StoreID;
                    key.Partner_key = config.Scope;
                }

                return key ?? throw new Exception("API key not found.");
            });
        }

        private async Task UpdateTokenInDatabase(string platformID, ShopeeRefreshTokenResponse result, long timestamp)
        {
            string shopeeTokenUpdateSql = @"
        Update EcommerceStore
        set Refreshtoken = @Refreshtoken, AccessToken = @AccessToken, KeyValue = @Expire
        where OldID = @PlatformID";

            long expire = result.Expire_in + timestamp;
            var param = new DynamicParameters();
            param.Add("@PlatformID", platformID);
            param.Add("@Refreshtoken", result.Refresh_token);
            param.Add("@AccessToken", result.Access_token);
            param.Add("@Expire", expire);

            using (var connection = _context.Connection)
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();
                try
                {
                    await connection.ExecuteAsync(shopeeTokenUpdateSql, param, transaction: transaction, commandTimeout: 180);
                    transaction.Commit();
                }
                catch (SqlException e)
                {
                    throw new Exception();
                }
            }
        }
        public async Task<TCatApiKey> GetTCatApiKeyAsync(string CustomerID)
        {
            string sql = @"select DeliveryTypeID, CompanyID, CustomerToken
                           from EC_DeliverySetting WHERE CompanyID = @CustomerID";
            return await _cache.GetOrCreateAsync($"TCatApiKey{CustomerID}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30); // 快取 30 分鐘
                var config = await _context.Connection.QueryFirstOrDefaultAsync(sql, new { CustomerID = CustomerID });
                if (config == null)
                {
                    throw new Exception("API key not found.");
                }
                var key = new TCatApiKey()
                {
                    CustomerContractID = config.CompanyID,
                    CustomerContractCode = config.CustomerToken
                };

                return key;
            });
        }

        public async Task<HomeDeliveryAddressCommon> GetECStoreSenderAddressAsync(string platformID)
        {
            string sql = @"select SenderName, SenderPhone, ShippingAddress
                           from EcommerceStore WHERE OldID = @PlatformID";
            return await _cache.GetOrCreateAsync($"StoreSenderAddress{platformID}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30); // 快取 30 分鐘
                var config = await _context.Connection.QueryFirstOrDefaultAsync<HomeDeliveryAddressCommon>(sql, new { PlatformID = platformID });
                if (config == null)
                {
                    throw new Exception("API key not found.");
                }

                return config;
            });
        }

        public async Task<HCTKey> GetHCTApiKeyAsync(string CustomerID)
        {
            string sql = @"select DeliveryTypeID, CompanyID, CustomerToken
                           from EC_DeliverySetting WHERE CompanyID = @CustomerID";
            return await _cache.GetOrCreateAsync($"HCTApiKey{CustomerID}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30); // 快取 30 分鐘
                var config = await _context.Connection.QueryFirstOrDefaultAsync(sql, new { CustomerID = CustomerID });
                if (config == null)
                {
                    throw new Exception("API key not found.");
                }

                var key = new HCTKey()
                {
                    Company = config.CompanyID,
                    Password = config.CustomerToken
                };

                return key;
            });
        }
    }

    public static class StringExtensions
    {
        public static string AesEncryptYahoo(this string plainText, string base64Key, string base64IV)
        {
            if (string.IsNullOrEmpty(plainText)) throw new ArgumentNullException(nameof(plainText));
            if (string.IsNullOrEmpty(base64Key)) throw new ArgumentNullException(nameof(base64Key));
            if (string.IsNullOrEmpty(base64IV)) throw new ArgumentNullException(nameof(base64IV));

            var key = Convert.FromBase64String(base64Key); // 32 bytes
            var iv = Convert.FromBase64String(base64IV);   // 16 bytes

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var writer = new StreamWriter(cs))
            {
                writer.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public static string AesDecryptYahoo(this string cipherTextBase64, string base64Key, string base64IV)
        {
            if (string.IsNullOrEmpty(cipherTextBase64)) throw new ArgumentNullException(nameof(cipherTextBase64));
            if (string.IsNullOrEmpty(base64Key)) throw new ArgumentNullException(nameof(base64Key));
            if (string.IsNullOrEmpty(base64IV)) throw new ArgumentNullException(nameof(base64IV));

            var key = Convert.FromBase64String(base64Key); // 32 bytes
            var iv = Convert.FromBase64String(base64IV);   // 16 bytes

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var decryptor = aes.CreateDecryptor();
            var cipherBytes = Convert.FromBase64String(cipherTextBase64);

            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }

        public static string ComputeHmacSha512Yahoo(this string message, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            using var hmac = new HMACSHA512(keyBytes);
            var hashBytes = hmac.ComputeHash(messageBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();
        }

        public static string HmacSha256(this string message, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(messageBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();
        }
    }
}
