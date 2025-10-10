using Microsoft.AspNetCore.Mvc;
using CityHubCore.Infrastructure.API;
using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.Extensions.Logging;
using POVWebDomain.Models.API.StoreSrv.DashBoard.Announcement;
using System.Threading.Tasks;

namespace StoreWeb.Controllers.DashBoard
{
    public class AnnouncementController : ApiControllerBase
    {
        private readonly ILogger<AnnouncementController> _logger;
        private readonly StoreSrvClient _srvClient;

        public AnnouncementController(ILogger<AnnouncementController> logger, StoreSrvClient srvClient)
        {
            _logger = logger;
            _srvClient = srvClient;
        }

        [HttpGet("GetAnnouncementData")]
        public async Task<ActionResult> GetAnnouncementData()
        {
            var result = await _srvClient.HttpGetAsync<ResultModel<dynamic>>("api/Announcement/V1/GetAnnouncementData");
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("GetViewAnnouncementData")]
        public async Task<ActionResult> GetViewAnnouncementData(GetViewAnnouncementDataRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Announcement/V1/GetViewAnnouncementData", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("GetAnnouncementAllData")]
        public async Task<ActionResult> GetAnnouncementAllData(GetAnnouncementDataRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Announcement/V1/GetAnnouncementAllData", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("AddAnnouncementData")]
        public async Task<ActionResult> AddAnnouncementData(AddAnnouncementDataRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Announcement/V1/AddAnnouncementData", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("UpdAnnouncement")]
        public async Task<ActionResult> UpdAnnouncement(UpdAnnouncementDataRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Announcement/V1/UpdAnnouncement", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("DelAnnouncementData")]
        public async Task<ActionResult> DelAnnouncementData(DelAnnouncementDataRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Announcement/V1/DelAnnouncementData", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("UpdViews")]
        public async Task<ActionResult> UpdViews(UpdViewsRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Announcement/V1/UpdViews", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("AnnouncementContentEdit")]
        public async Task<ActionResult> AnnouncementContentEdit(AnnouncementContentEditRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Announcement/V1/AnnouncementContentEdit", request);
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