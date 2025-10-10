using CityHubCore.Application;
using CityHubCore.Application.Attributies;
using CityHubCore.Infrastructure.API;
using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using POVWebDomain.Models.API.StoreSrv.SettingMgmt.UserAccount;
using POVWebDomain.Models.API.StoreSrv.Common;
using Dapper;
using POVWebDomain.Models.DB.POVWeb;
using System.Data;
using System.IO;
using System;

namespace StoreWeb.Controllers.SettingMgmt
{
    public class UserAccountController : ApiControllerBase
    {
        private readonly ILogger<UserAccountController> _logger;
        private readonly StoreSrvClient _srvClient;

        public UserAccountController(ILogger<UserAccountController> logger, StoreSrvClient srvClient)
        {
            _logger = logger;
            _srvClient = srvClient;
        }

        [HttpGet("GetAllData")]
        public async Task<ActionResult> GetAllData()
        {
            var result = await _srvClient.HttpGetAsync<ResultModel<dynamic>>("api/UserAccount/V1/GetAllData");
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("GetData")]
        public async Task<ActionResult> GetData(GetDataRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/UserAccount/V1/GetData", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("GetSingleData")]
        public async Task<ActionResult> GetSingleData(GetSingleDataRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/UserAccount/V1/GetSingleData", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("AddUsersData")]
        public async Task<ActionResult> AddUsersData(AddUsersDataRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/UserAccount/V1/AddUsersData", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("DelUsersData")]
        public async Task<ActionResult> DelUsersData(DelUsersDataRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/UserAccount/V1/DelUsersData", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("UpdUsersData")]
        public async Task<ActionResult> UpdUsersData(UpdUsersDataRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/UserAccount/V1/UpdUsersData", request);
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

