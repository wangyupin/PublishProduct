using CityHubCore.Application;
using CityHubCore.Application.Attributies;
using CityHubCore.Infrastructure.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace StoreWeb.Controllers {
    public class AppInfoController : ApiControllerBase {
        private readonly ILogger<AppInfoController> _logger;
        private readonly AppInfo _appInfo;

        public AppInfoController(ILogger<AppInfoController> logger, IOptions<AppInfo> appInfo) {
            _logger = logger;
            _appInfo = appInfo.Value;
        }

        [AllowAnonymous]
        [HttpGet("GetAppInfo")]
        public ActionResult GetAppInfo() {
            return Ok(
                FormatResultModel<dynamic>.Success(_appInfo)
                );
        }
    }
}
