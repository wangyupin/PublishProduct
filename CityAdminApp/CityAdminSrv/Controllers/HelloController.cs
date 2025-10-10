using CityHubCore.Infrastructure.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace CityAdminSrv.Controllers {
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
        private readonly IConfiguration _Config;

        public HelloController(
            ILogger<HelloController> logger
            , IConfiguration config
            ) {
            _logger = logger;
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
                Product = "CityAdminSrv",
                Version = "V0.0.0.3",
                Date = "2022/1/3",
                ServerDT = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}"
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

        [HttpPost("V1/GetVersion")]
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

    }
}
