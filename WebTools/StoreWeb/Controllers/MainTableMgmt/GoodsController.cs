using CityHubCore.Infrastructure.API;
using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POVWebDomain.Models.API.StoreSrv.MainTableMgmt.Goods;
using System.Threading.Tasks;

namespace StoreWeb.Controllers
{
    public class GoodsController : ApiControllerBase
    {
        private readonly ILogger<GoodsController> _logger;
        private readonly StoreSrvClient _srvClient;

        public GoodsController(ILogger<GoodsController> logger, StoreSrvClient srvClient)
        {
            _logger = logger;
            _srvClient = srvClient;
        }


        [HttpPost("GetGoodsHelpOffset")]
        public async Task<ActionResult> GetGoodsHelpOffset(GetGoodsHelpOffsetRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Goods/V1/GetGoodsHelpOffset", request);
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
