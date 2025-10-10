using CityHubCore.Infrastructure.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POVWebDomain.Models.DB.POVWeb;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using CityHubCore.Infrastructure.ServiceClient;
using POVWebDomain.Models.API.StoreSrv.SettingMgmt.UserAccount;

namespace StoreSrv.Controllers.SettingMgmt
{
    public class UserAccountController : ApiControllerBase
    {
        private readonly ILogger<UserAccountController> _logger;
        private readonly POVWebDbContext _POVWebDb;
        private readonly POVWebDbContextDapper _POVWebDbContextDapper;
        private readonly HqSrvClient _HqSrvClient;
        private readonly IConfiguration _Config;

        public UserAccountController(
            ILogger<UserAccountController> logger
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
            var result = await _HqSrvClient.HttpGetAsync<ResultModel<object>>("api/UserAccount/V1/GetAllData");

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetData")]
        public async Task<ActionResult> GetData(GetDataRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/UserAccount/V1/GetData", request);

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetSingleData")]
        public async Task<ActionResult> GetSingleData(GetSingleDataRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/UserAccount/V1/GetSingleData", request);

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/AddUsersData")]
        public async Task<ActionResult> AddUsersData(AddUsersDataRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/UserAccount/V1/AddUsersData", request);

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/DelUsersData")]
        public async Task<ActionResult> DelUsersData(DelUsersDataRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/UserAccount/V1/DelUsersData", request);

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/UpdUsersData")]
        public async Task<ActionResult> UpdUsersData(UpdUsersDataRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/UserAccount/V1/UpdUsersData", request);

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }
    }
}
