using CityHubCore.Application.Attributies;
using CityHubCore.Application.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace CityHubCore.Application.Jwt {
    /// <summary>
    /// JWT Auth
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class JwtAuthorizeAttribute : Attribute, IAuthorizationFilter {
        public void OnAuthorization(AuthorizationFilterContext context) {
            // skip authorization if action is decorated with [AllowAnonymous] attribute
            if ( context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any() ) return;

            // authorization
            _ = (JwtUserInfo)context.HttpContext.Items["UserJWTInfo"] ?? throw new JwtAuthorizeNotAllowException();
        }
    }
}