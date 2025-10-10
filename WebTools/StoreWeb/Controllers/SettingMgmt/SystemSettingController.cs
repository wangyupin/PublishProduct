using CityHubCore.Infrastructure.API;
using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POVWebDomain.Models.API.StoreSrv.SettingMgmt.GroupAccount;
using POVWebDomain.Models.API.StoreSrv.SettingMgmt.SystemSetting;
using System.Runtime;
using System.Threading.Tasks;

namespace StoreWeb.Controllers.SettingMgmt
{
    public class SystemSettingController : ApiControllerBase
    {
        private readonly ILogger<SystemSettingController> _logger;
        private readonly StoreSrvClient _srvClient;

        public SystemSettingController(ILogger<SystemSettingController> logger, StoreSrvClient srvClient)
        {
            _logger = logger;
            _srvClient = srvClient;
        }

        [HttpPost("GetSettings")]
        public async Task<ActionResult> GetSettings(GetSettingsRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/SystemSetting/V1/GetSettings", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpGet("GetSingleData")]
        public async Task<ActionResult> GetSingleData()
        {
            var result = await _srvClient.HttpGetAsync<ResultModel<dynamic>>("api/SystemSetting/V1/GetSingleData");
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }


        [HttpPost("UpdateSettings")]
        public async Task<ActionResult> UpdateSettings(SystemSettingsEditable request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/SystemSetting/V1/UpdateSettings", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

    }
}