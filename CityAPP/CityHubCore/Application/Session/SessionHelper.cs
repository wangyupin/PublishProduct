using CityHubCore.Application.Jwt;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text;

namespace CityHubCore.Application.Session {
    public class SessionHelper {
        private static string getCacheKey(JwtUserInfo jwtUserInfo) {
            return $"SID-{jwtUserInfo.CompanyId}-{jwtUserInfo.UserId}";
        }

        static public string GetSessionIDFromCache(IDistributedCache cache, JwtUserInfo jwtUserInfo) {
            string cacheKey = getCacheKey(jwtUserInfo);
            string cacheData = cache.GetString(cacheKey) ?? "";

            return cacheData.Split('-').Length > 1 ? cacheData.Split('-')[0] : "";
        }

        static public string AddSessionIDToCache(IDistributedCache cache, JwtConfig jwtConfig, JwtUserInfo jwtUserInfo) {
            string cacheKey = getCacheKey(jwtUserInfo);
            string write2Cache = $"{jwtUserInfo.SID}-{DateTime.Now:yyyy/MM/dd HH:mm:ss}";

            cache.SetString(cacheKey, write2Cache, new DistributedCacheEntryOptions() {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(jwtConfig.SessionIDExpiresInHours)
            });

            return jwtUserInfo.SID;
        }

        static public void DelSessionIDCache(IDistributedCache cache, JwtUserInfo jwtUserInfo) {
            string cacheKey = getCacheKey(jwtUserInfo);
            cache.Remove(cacheKey);
        }
    }
}
