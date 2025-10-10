using CityHubCore.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityHubCore.Application.Jwt {
    public class JwtMiddleware {
        private readonly RequestDelegate _next;
        private readonly JwtConfig _jwtConfig;
        private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(RequestDelegate next, IOptions<JwtConfig> jwtConfig, ILogger<JwtMiddleware> logger) {
            _logger = logger;
            _next = next;
            _jwtConfig = jwtConfig.Value;
        }

        public async Task Invoke(HttpContext context) {
            var token = context.Request.Headers[_jwtConfig.HeaderName].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                AttachUserToContext(context, token);

            await _next(context);
        }

        /// <summary>
        /// ValidateToken
        /// REF https://jasonwatmore.com/post/2021/06/15/net-5-api-jwt-authentication-with-refresh-tokens#allow-anonymous-attribute-cs
        /// REF https://mp.weixin.qq.com/s/fWVR-y9C5vqmGZ_aOApq5A 
        /// MoonFeng @ 2021/08/16
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userService"></param>
        /// <param name="token"></param>
        private void AttachUserToContext(HttpContext context, string token) {
            try {
                context.Items["UserJWTInfo"] = JwtHelper.ValidateJwtToken(_jwtConfig.Issuer, _jwtConfig.Audience, _jwtConfig.Secret, token);
            } catch (SecurityTokenExpiredException) {
                throw new JwtAccessTokenExpiredException();
            }
        }
    }
}