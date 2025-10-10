using CityHubCore.Application.Attributies;
using CityHubCore.Application.Exceptions;
using CityHubCore.Application.Jwt;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Text.Json;

namespace CityHubCore.Application.Session {
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SessionAuthorizeAttribute : Attribute, IAuthorizationFilter {
        private string GetRemoteIpAddress(AuthorizationFilterContext context) {
            var Request = context.HttpContext.Request;

            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return context.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        private string GetRemoteUserAgent(AuthorizationFilterContext context) {
            var Request = context.HttpContext.Request;

            if (Request.Headers.ContainsKey("User-Agent"))
                return Request.Headers["User-Agent"];
            else
                return null;
        }

        private string GetUserLogoutRequestJson(AuthorizationFilterContext context) {

            JwtUserInfo jwtUserInfo = (JwtUserInfo)context.HttpContext.Items["UserJWTInfo"] ?? throw new JwtAuthorizeNotAllowException();
            SessionUserInfo request = new() {
                CompanyId = jwtUserInfo.CompanyId,
                UserId = jwtUserInfo.UserId,
                SID = jwtUserInfo.SID,
                IpAddress = GetRemoteIpAddress(context),
                UserAgent = GetRemoteUserAgent(context)
            };

            return JsonSerializer.Serialize(request);
        }

        public void OnAuthorization(AuthorizationFilterContext context) {
            // skip authorization if action is decorated with [AllowAnonymous] attribute
            if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any()) return;

            // authorization
            SessionInfo sessionInfo = (SessionInfo)context.HttpContext.Items["SessionInfo"] ??
                throw new SessionNotFoundException(GetUserLogoutRequestJson(context));

            if (sessionInfo.CacheSID.Length < 1)
                throw new SessionNotFoundException(GetUserLogoutRequestJson(context));

            if (sessionInfo.JWTSID != sessionInfo.CacheSID) throw new SessionNotAllowException();
        }
    }
}
