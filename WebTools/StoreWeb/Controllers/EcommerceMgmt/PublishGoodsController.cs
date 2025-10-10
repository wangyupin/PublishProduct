using CityHubCore.Infrastructure.API;
using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using System.Threading.Tasks;
using POVWebDomain.Models.ExternalApi.Store91;

namespace StoreWeb.Controllers.EcommerceMgmt
{
    public class PublishGoodsController : ApiControllerBase
    {
        private readonly ILogger<PublishGoodsController> _logger;
        private readonly StoreSrvClient _srvClient;

        public PublishGoodsController(ILogger<PublishGoodsController> logger, StoreSrvClient srvClient)
        {
            _logger = logger;
            _srvClient = srvClient;
        }

        [HttpPost("GetOptionAll")]
        public async Task<ActionResult> GetOptionAll(GetOptionAllRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/PublishGoods/V1/GetOptionAll", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("GetCategory")]
        public async Task<ActionResult> GetCategory(GetCategoryRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/PublishGoods/V1/GetCategory", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("GetShopCategory")]
        public async Task<ActionResult> GetShopCategory(GetShopCategoryRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/PublishGoods/V1/GetShopCategory", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("GetPayment")]
        public async Task<ActionResult> GetPayment(GetPaymentRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/PublishGoods/V1/GetPayment", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("GetShipping")]
        public async Task<ActionResult> GetShipping(GetShippingRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/PublishGoods/V1/GetShipping", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("SubmitMain")]
        public async Task<ActionResult> SubmitMain([FromForm] SubmitMainRequestAll request)
        {
            var result = await _srvClient.HttpPostAsyncFormData<ResultModel<dynamic>>("api/PublishGoods/V1/SubmitMain", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("SavePicture")]
        public async Task<ActionResult> SavePicture([FromForm] SavePictureRequest request)
        {
            var result = await _srvClient.HttpPostAsyncFormData<ResultModel<dynamic>>("api/PublishGoods/V1/SavePicture", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpGet("GetOptions")]
        public async Task<ActionResult> GetOptions()
        {
            var result = await _srvClient.HttpGetAsync<ResultModel<dynamic>>("api/PublishGoods/V1/GetOptions");
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("GetSubmitMode")]
        public async Task<ActionResult> GetSubmitMode(GetSubmitModeRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/PublishGoods/V1/GetSubmitMode", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("GetSubmitDefVal")]
        public async Task<ActionResult> GetSubmitDefVal(GetSubmitDefValRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/PublishGoods/V1/GetSubmitDefVal", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpGet("GetEStoreCatOptions")]
        public async Task<ActionResult> GetEStoreCatOptions()
        {
            var result = await _srvClient.HttpGetAsync<ResultModel<dynamic>>("api/PublishGoods/V1/GetEStoreCatOptions");
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
