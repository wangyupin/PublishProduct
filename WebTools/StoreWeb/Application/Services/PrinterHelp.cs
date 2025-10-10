using CityHubCore.Application.Jwt;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text;

namespace CityHubCore.Application.Session {
    public class PrinterHelper {
        private static string getCacheKey() {
            return $"SID-PID-01";
        }

        static public string GetPrinterStatueFromCache(IDistributedCache cache) {
            string cacheKey = getCacheKey();
            string cacheData = cache.GetString(cacheKey) ?? "False-False-0-0";
            //if (cacheData.Equals("0-0-0")) UpdPrinterStatueFromCache(cache, cacheData);
            return cacheData;
        }

        static public string UpdPrinterStatueFromCache(IDistributedCache cache, string data) {
            string cacheKey = getCacheKey();

            cache.SetString(cacheKey, data, new DistributedCacheEntryOptions() {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(1)
            });

            return data;
        }

    }
}
