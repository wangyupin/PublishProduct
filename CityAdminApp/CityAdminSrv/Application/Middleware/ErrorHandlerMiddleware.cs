using CityHubCore.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace CityAdminSrv.Application.Middleware {
    public class ErrorHandlerMiddleware {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger) {
            _next = next;
            _logger = logger;
        }

        //private async void doClearSession(string userLogoutRequestJson) {
        //    SessionUserInfo request = JsonSerializer.Deserialize<SessionUserInfo>(userLogoutRequestJson, new JsonSerializerOptions {
        //        PropertyNameCaseInsensitive = true
        //    });

        //    await _srvClient.HttpPostAsync<ResultModel<string>>("api/user/v1/ClearSession", request);
        //}

        public async Task Invoke(HttpContext context) {
            try {
                await _next(context);
            } catch (Exception ex) {
                // _logger.LogError(ex, "Divide By Zero Exception occurred");

                var response = context.Response;
                response.ContentType = "application/json";

                switch (ex) {
                    case LoginDuplicationException:
                        // 300
                        response.StatusCode = (int)HttpStatusCode.MultipleChoices;
                        break;
                    case AppException:
                        // 400
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case KeyNotFoundException:
                        // 404
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case JwtAccessTokenExpiredException:
                        // 401
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        break;
                    case JwtAuthorizeNotAllowException:
                        //403
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        break;
                    case JwtRefreshTokenExpiredException:
                        //400
                        //Todo: redirect to login
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case JwtRefreshTokenNotFoundException:
                        //400
                        // Todo : redirect to login 
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case SessionNotFoundException:
                        // 440 登入逾時，請重新登入
                        response.StatusCode = 440;
                        //doClearSession(ex?.Message);
                        break;
                    case SessionNotAllowException:
                        // 441 本帳號已從其它裝置登入。
                        response.StatusCode = 441;
                        break;
                    default:
                        // 500
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var result = JsonSerializer.Serialize(new { message = ex?.Message });
                await response.WriteAsync(result);
            }
        }
    }
}
