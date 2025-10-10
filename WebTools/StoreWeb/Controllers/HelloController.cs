using CityHubCore.Application.Attributies;
using CityHubCore.Infrastructure.API;
using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoreWeb.Controllers {
    public class GetVersionRequest {
        public string Token { get; set; }
    }

    public class GetHelloRequest : GetVersionRequest {
        public int id { get; set; }
    }

    public class AddHelloAllRequest : GetVersionRequest {

    }

    public class AddHelloRequest : GetVersionRequest {
        public string memo { get; set; }
    }

    public class AddHelloListRequest : GetVersionRequest {
        public List<string> memoList { get; set; }
    }

    public class HelloController : ApiControllerBase {
        private readonly ILogger<HelloController> _logger;
        private readonly StoreSrvClient _StoreSrvClient;
        private readonly IConfiguration _Config;

        public HelloController(
            ILogger<HelloController> logger
            , StoreSrvClient storeSrvClient
            , IConfiguration config
            ) {
            _logger = logger;
            _StoreSrvClient = storeSrvClient;
            _Config = config;
        }

        private object GetSrvClientSetting(string SessionName) {
            return new {
                Uri = _Config[$"{SessionName}:Uri"],
                UserAgent = _Config[$"{SessionName}:UserAgent"],
                Timeout = _Config[$"{SessionName}:Timeout"]
            };
        }

        private object GetLocalHostInfo() {
            return new {
                Product = "StoreSrv",
                Version = "V0.0.0.1",
                Date = "2021/12/13",
                ServerDT = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}",
                StoreSrv = GetSrvClientSetting("StoreSrv")
            };
        }

        private bool IsTokenBad(string token) {
            // Check parameters
            token ??= "";

            // Initial variables
            var nowDTM = DateTime.UtcNow;
            var nowToken = $"{nowDTM:MMddmm}";
            var lastToken = $"{nowDTM.AddMinutes(-1):MMddmm}";

            // Biz
            var result = token.Equals(nowToken) || token.Equals(lastToken);

            // Result
            return !result;
        }

        [AllowAnonymous]
        [HttpPost("GetVersion")]
        public ActionResult GetVersion(GetVersionRequest request) {
            // Check parameters
            if (IsTokenBad(request.Token))
                return BadRequest(FormatResultModel<dynamic>.Failure(
                    new {
                        MSG = "Bad Token",
                        ServerDT = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}"
                    }));

            // Initial variables

            // Biz

            // Result
            return Ok(FormatResultModel<dynamic>.Success(GetLocalHostInfo()));
        }

        [AllowAnonymous]
        [HttpPost("GetAllVersion")]
        public ActionResult GetAllVersion(GetVersionRequest request) {
            // Check parameters
            if (IsTokenBad(request.Token))
                return BadRequest(FormatResultModel<dynamic>.Failure(
                    new {
                        MSG = "Bad Token",
                        ServerDT = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}"
                    }));

            // Initial variables

            // Biz
            var result = _StoreSrvClient.HttpPost<ResultModel<object>>("api/Hello/V1/GetVersion",
                new GetVersionRequest() { Token = $"{DateTime.UtcNow:MMddmm}" }
                );

            if (result is null || result.Succeeded is false || result.Data is null) {
                return Ok(FormatResultModel<dynamic>.Failure(
                    new {
                        MSG = "StoreSrvClient not exist",
                        ServerDT = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}"
                    }));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(
                new {
                    storeSrv = GetLocalHostInfo(),
                    hqSrv = result
                }));
        }

        [AllowAnonymous]
        [HttpPost("GetVersionsAsync")]
        public ActionResult GetVersions(GetVersionRequest request)
        {
            // Check parameters
            if (IsTokenBad(request.Token))
                return BadRequest(FormatResultModel<dynamic>.Failure(
                    new
                    {
                        MSG = "Bad Token",
                        ServerDT = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}"
                    }));

            // Initial variables

            // Biz

            // Result
            return Ok(FormatResultModel<dynamic>.Success(GetLocalHostInfo()));
        }

        [AllowAnonymous]
        [HttpPost("GetAllVersionAsync")]
        public async Task<ActionResult> GetAllVersionAsync(GetVersionRequest request) {
            // Check parameters
            if (IsTokenBad(request.Token))
                return BadRequest(FormatResultModel<dynamic>.Failure(
                    new {
                        MSG = "Bad Token",
                        ServerDT = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}"
                    }));

            // Initial variables

            // Biz
            var result = await _StoreSrvClient.HttpPostAsync<ResultModel<object>>("api/Hello/V1/GetAllVersionAsync",
                new GetVersionRequest() { Token = $"{DateTime.UtcNow:MMddmm}" }
                );

            if (result is null || result.Succeeded is false || result.Data is null) {
                return Ok(FormatResultModel<dynamic>.Failure(
                    new {
                        MSG = "StoreSrvClient not exist",
                        ServerDT = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}"
                    }));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(
                new {
                    storeSrv = GetLocalHostInfo(),
                    hqSrv = result
                }));
        }

    }
}
