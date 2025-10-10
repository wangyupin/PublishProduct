using CityHubCore.Infrastructure.ServiceClient;
using HqSrv.Repository.SettingMgmt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POVWebDomain.Models.DB.POVWeb;
using Microsoft.Extensions.Configuration;
using CityHubCore.Infrastructure.API;
using HqSrv.Repository.MainTableMgmt;
using System.Threading.Tasks;
using System;
using POVWebDomain.Models.API.StoreSrv.SettingMgmt.SystemSetting;
using System.Runtime;

namespace HqSrv.Controllers.SettingMgmt
{
    public class SystemSettingController : ApiControllerBase
    {
        private readonly ILogger<SystemSettingController> _logger;
        private readonly POVWebDbContext _POVWebDb;
        private readonly POVWebDbContextDapper _POVWebDbContextDapper;
        private readonly CityAdminSrvClient _CityAdminSrvClient;
        private readonly IConfiguration _Config;
        private readonly SystemSettingsRepository _SystemSettingsRepository;

        public SystemSettingController(
            ILogger<SystemSettingController> logger
            , POVWebDbContext pOVWebDbContext
            , POVWebDbContextDapper pOVWebDbContextDapper
            , CityAdminSrvClient cityAdminSrvClient
            , IConfiguration config
            , SystemSettingsRepository systemSettingsRepository
            )
        {
            _logger = logger;
            _POVWebDb = pOVWebDbContext;
            _POVWebDbContextDapper = pOVWebDbContextDapper;
            _CityAdminSrvClient = cityAdminSrvClient;
            _Config = config;
            _SystemSettingsRepository = systemSettingsRepository;
        }

        [HttpPost("V1/GetSettings")]
        public async Task<ActionResult> GetSettings(GetSettingsRequest request)
        {
            try
            {
                var settings = await _SystemSettingsRepository.GetSettingsAsync(request);

                return Ok(FormatResultModel<dynamic>.Success(new { settings }));
            }
            catch (Exception e)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
            }
        }

        [HttpGet("V1/GetSingleData")]
        public async Task<ActionResult> GetSingleData()
        {

            try
            {
                var result = await _SystemSettingsRepository.GetSingleData();
                if (string.IsNullOrEmpty(result.Item2))
                {
                    return Ok(FormatResultModel<dynamic>.Success(result.Item1));
                }
                else
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = result.Item2 }));
                }
            }
            catch (Exception e)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
            }
        }



        [HttpPost("V1/UpdateSettings")]
        public async Task<ActionResult> UpdateSettings(SystemSettingsEditable request)
        {
            try
            {
                var result = await _SystemSettingsRepository.UpdateSettingsAsync(request);
                if (string.IsNullOrEmpty(result.Item2))
                {
                    return Ok(FormatResultModel<dynamic>.Success(result.Item1));
                }
                else
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = result.Item2 }));
                }
            }
            catch (Exception e)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
            }

        }
    }
}
