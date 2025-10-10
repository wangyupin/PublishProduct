using CityHubCore.Infrastructure.API;
using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using POVWebDomain.Models.DB.POVWeb;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using POVWebDomain.Models.ExternalApi.Store91;

namespace StoreSrv.Controllers.EcommerceMgmt
{
    public class PublishGoodsController : ApiControllerBase
    {
        private readonly ILogger<PublishGoodsController> _logger;
        private readonly POVWebDbContext _POVWebDb;
        private readonly POVWebDbContextDapper _POVWebDbContextDapper;
        private readonly HqSrvClient _HqSrvClient;
        private readonly IConfiguration _Config;

        public PublishGoodsController(
            ILogger<PublishGoodsController> logger
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

        [HttpPost("V1/GetOptionAll")]
        public async Task<ActionResult> GetOptionAll(GetOptionAllRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/PublishGoods/V1/GetOptionAll", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetCategory")]
        public async Task<ActionResult> GetCategory(GetCategoryRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/PublishGoods/V1/GetCategory", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetShopCategory")]
        public async Task<ActionResult> GetShopCategory(GetShopCategoryRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/PublishGoods/V1/GetShopCategory", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetPayment")]
        public async Task<ActionResult> GetPayment(GetPaymentRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/PublishGoods/V1/GetPayment", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetShipping")]
        public async Task<ActionResult> GetShipping(GetShippingRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/PublishGoods/V1/GetShipping", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/SubmitMain")]
        public async Task<ActionResult> SubmitMain([FromForm] SubmitMainRequestAll request)
        {
            var result = await _HqSrvClient.HttpPostAsyncFormData<ResultModel<dynamic>>("api/PublishGoods/V1/SubmitMain", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/SavePicture")]
        public async Task<ActionResult> SavePicture([FromForm] SavePictureRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsyncFormData<ResultModel<dynamic>>("api/PublishGoods/V1/SavePicture", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpGet("V1/GetOptions")]
        public async Task<ActionResult> GetOptions()
        {
            var result = await _HqSrvClient.HttpGetAsync<ResultModel<object>>("api/PublishGoods/V1/GetOptions");

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetSubmitMode")]
        public async Task<ActionResult> GetSubmitMode(GetSubmitModeRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/PublishGoods/V1/GetSubmitMode", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetSubmitDefVal")]
        public async Task<ActionResult> GetSubmitDefVal(GetSubmitDefValRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/PublishGoods/V1/GetSubmitDefVal", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpGet("V1/GetEStoreCatOptions")]
        public async Task<ActionResult> GetEStoreCatOptions()
        {
            var result = await _HqSrvClient.HttpGetAsync<ResultModel<object>>("api/PublishGoods/V1/GetEStoreCatOptions");

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }
    }
}
