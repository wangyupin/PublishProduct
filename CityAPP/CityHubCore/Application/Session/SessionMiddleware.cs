using CityHubCore.Application.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace CityHubCore.Application.Session {
    /// <summary>
    /// SessionMiddleware Should be setting behind of JwtMiddleware
    /// </summary>
    public class SessionMiddleware {
        private readonly RequestDelegate _next;
        private readonly JwtConfig _jwtConfig;
        private readonly ILogger<JwtMiddleware> _logger;
        private readonly IDistributedCache _cache;

        public SessionMiddleware(RequestDelegate next, IOptions<JwtConfig> jwtConfig
            , ILogger<JwtMiddleware> logger, IDistributedCache cache) {
            _logger = logger;
            _next = next;
            _jwtConfig = jwtConfig.Value;
            _cache = cache;
        }

        public async Task Invoke(HttpContext context) {
            var jwtUserInfo = (JwtUserInfo)context.Items["UserJWTInfo"];
            SessionInfo sessionInfo = new();

            if (jwtUserInfo != null) {
                sessionInfo.JWTSID = jwtUserInfo.SID;
                sessionInfo.CacheSID = SessionHelper.GetSessionIDFromCache(_cache, jwtUserInfo);
                context.Items["SessionInfo"] = sessionInfo;
            }

            await _next(context);
        }

    }
}
