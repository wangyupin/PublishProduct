using CityHubCore.Infrastructure.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POVWebDomain.Models.DB.POVWeb;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using CityHubCore.Infrastructure.ServiceClient;
using POVWebDomain.Models.API.StoreSrv.SettingMgmt.GroupAccount;

namespace StoreSrv.Controllers.SettingMgmt
{
    public class GroupAccountController : ApiControllerBase
    {
        private readonly ILogger<GroupAccountController> _logger;
        private readonly POVWebDbContext _POVWebDb;
        private readonly POVWebDbContextDapper _POVWebDbContextDapper;
        private readonly HqSrvClient _HqSrvClient;
        private readonly IConfiguration _Config;

        public GroupAccountController(
            ILogger<GroupAccountController> logger
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

        [HttpGet("V1/GetAllData")]
        public async Task<ActionResult> GetAllData()
        {
            var result = await _HqSrvClient.HttpGetAsync<ResultModel<object>>("api/GroupAccount/V1/GetAllData");

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetGroupPermission")]
        public async Task<ActionResult> GetGroupPermission(GetGroupPermissionRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/GroupAccount/V1/GetGroupPermission",request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/AddGroupPermission")]
        public async Task<ActionResult> AddGroupPermission(AddGroupPermissionRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/GroupAccount/V1/AddGroupPermission", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }


        [HttpPost("V1/UpdGroupPermission")]
        public async Task<ActionResult> UpdGroupPermission(UpdGroupPermissionRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/GroupAccount/V1/UpdGroupPermission", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }


        [HttpPost("V1/DelGroupPermission")]
        public async Task<ActionResult> DelGroupPermission(DelGroupPermissionRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/GroupAccount/V1/DelGroupPermission", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

    }
}
