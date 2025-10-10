using CityHubCore.Application;
using CityHubCore.Application.Attributies;
using CityHubCore.Infrastructure.API;
using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using POVWebDomain.Models.API.StoreSrv.SettingMgmt.GroupAccount;

namespace StoreWeb.Controllers.SettingMgmt
{
    public class GroupAccountController : ApiControllerBase
    {
        private readonly ILogger<GroupAccountController> _logger;
        private readonly StoreSrvClient _srvClient;

        public GroupAccountController(ILogger<GroupAccountController> logger, StoreSrvClient srvClient)
        {
            _logger = logger;
            _srvClient = srvClient;
        }

        [HttpGet("GetAllData")]
        public async Task<ActionResult> GetAllData()
        {
            var result = await _srvClient.HttpGetAsync<ResultModel<dynamic>>("api/GroupAccount/V1/GetAllData");
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("GetGroupPermission")]
        public async Task<ActionResult> GetGroupPermission(GetGroupPermissionRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/GroupAccount/V1/GetGroupPermission", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("AddGroupPermission")]
        public async Task<ActionResult> AddGroupPermission(AddGroupPermissionRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/GroupAccount/V1/AddGroupPermission", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("UpdGroupPermission")]
        public async Task<ActionResult> UpdGroupPermission(UpdGroupPermissionRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/GroupAccount/V1/UpdGroupPermission", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("DelGroupPermission")]
        public async Task<ActionResult> DelGroupPermission(DelGroupPermissionRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/GroupAccount/V1/DelGroupPermission", request);
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

