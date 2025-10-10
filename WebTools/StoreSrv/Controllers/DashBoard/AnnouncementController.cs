using CityHubCore.Infrastructure.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POVWebDomain.Models.DB.POVWeb;
using POVWebDomain.Models.API.StoreSrv.DashBoard.Announcement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StoreSrv.Controllers.DashBoard
{
    public class AnnouncementController : ApiControllerBase
    {
        private readonly ILogger<HelloController> _logger;
        private readonly POVWebDbContext _POVWebDb;
        private readonly POVWebDbContextDapper _POVWebDbContextDapper;
        private readonly HqSrvClient _HqSrvClient;
        private readonly IConfiguration _Config;

        public AnnouncementController(
            ILogger<HelloController> logger
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

        [HttpGet("V1/GetAnnouncementData")]
        public async Task<ActionResult> GetAnnouncementData()
        {
            var result = await _HqSrvClient.HttpGetAsync<ResultModel<object>>("api/Announcement/V1/GetAnnouncementData");

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetViewAnnouncementData")]
        public async Task<ActionResult> GetViewAnnouncementData(GetViewAnnouncementDataRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/Announcement/V1/GetViewAnnouncementData", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetAnnouncementAllData")]
        public async Task<ActionResult> GetAnnouncementData(GetAnnouncementDataRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/Announcement/V1/GetAnnouncementAllData", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/AddAnnouncementData")]
        public async Task<ActionResult> AddAnnouncementData(AddAnnouncementDataRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/Announcement/V1/AddAnnouncementData", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/UpdAnnouncement")]
        public async Task<ActionResult> UpdAnnouncement(UpdAnnouncementDataRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/Announcement/V1/UpdAnnouncement", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/DelAnnouncementData")]
        public async Task<ActionResult> DelAnnouncementData(DelAnnouncementDataRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/Announcement/V1/DelAnnouncementData", request);

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/UpdViews")]
        public async Task<ActionResult> UpdViews(UpdViewsRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/Announcement/V1/UpdViews", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/AnnouncementContentEdit")]
        public async Task<ActionResult> AnnouncementContentEdit(AnnouncementContentEditRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/Announcement/V1/AnnouncementContentEdit", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }
    }
    
}
