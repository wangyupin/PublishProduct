using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POVWebDomain.Models.DB.POVWeb;
using Microsoft.Extensions.Configuration;
using CityHubCore.Infrastructure.API;
using System.Threading.Tasks;
using POVWebDomain.Models.API.StoreSrv.SettingMgmt.SystemSetting;
using System.Runtime;

namespace StoreSrv.Controllers.SettingMgmt
{
    public class SystemSettingController : ApiControllerBase
    {
        private readonly ILogger<SystemSettingController> _logger;
        private readonly POVWebDbContext _POVWebDb;
        private readonly POVWebDbContextDapper _POVWebDbContextDapper;
        private readonly HqSrvClient _HqSrvClient;
        private readonly IConfiguration _Config;

        public SystemSettingController(
            ILogger<SystemSettingController> logger
            , POVWebDbContext pOVWebDbContext
            , POVWebDbContextDapper pOVWebDbContextDapper
            , HqSrvClient hqSrvClient
            , IConfiguration config
            )
        {
            _logger = logger;
            _POVWebDb = pOVWebDbContext;
            _POVWebDbContextDapper = pOVWebDbContextDapper;
            _HqSrvClient = hqSrvClient;
            _Config = config;
        }

        [HttpPost("V1/GetSettings")]
        public async Task<ActionResult> GetSettings(GetSettingsRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/SystemSetting/V1/GetSettings", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpGet("V1/GetSingleData")]
        public async Task<ActionResult> GetSingleData()
        {
            var result = await _HqSrvClient.HttpGetAsync<ResultModel<object>>("api/SystemSetting/V1/GetSingleData");

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/UpdateSettings")]
        public async Task<ActionResult> UpdateSettings(SystemSettingsEditable request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/SystemSetting/V1/UpdateSettings", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }
    }
}
