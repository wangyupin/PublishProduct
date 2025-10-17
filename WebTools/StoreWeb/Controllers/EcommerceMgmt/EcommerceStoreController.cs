using Microsoft.AspNetCore.Mvc;
using CityHubCore.Infrastructure.API;
using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.Extensions.Logging;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.EcommerceStore;
using System.Threading.Tasks;

namespace StoreWeb.Controllers.EcommerceStoreMgmt
{
    public class EcommerceStoreController : ApiControllerBase
    {
        private readonly ILogger<EcommerceStoreController> _logger;
        private readonly StoreSrvClient _srvClient;

        public EcommerceStoreController(ILogger<EcommerceStoreController> logger, StoreSrvClient srvClient)
        {
            _logger = logger;
            _srvClient = srvClient;
        }

        [HttpGet("GetEcommerceStoreData")]
        public async Task<ActionResult> GetEcommerceStoreData()
        {
            var result = await _srvClient.HttpGetAsync<ResultModel<dynamic>>("api/EcommerceStore/V1/GetEcommerceStoreData");
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
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/EcommerceStore/V1/GetSingleData", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("AddEcommerceStoreData")]
        public async Task<ActionResult> AddEcommerceStoreData(AddEcommerceStoreDataRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/EcommerceStore/V1/AddEcommerceStoreData", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("UpdEcommerceStore")]
        public async Task<ActionResult> UpdEcommerceStore(UpdEcommerceStoreDataRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/EcommerceStore/V1/UpdEcommerceStore", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("DelEcommerceStoreData")]
        public async Task<ActionResult> DelEcommerceStoreData(DelEcommerceStoreDataRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/EcommerceStore/V1/DelEcommerceStoreData", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("GetEcommerceStoreDetailByID")]
        public async Task<ActionResult> GetEcommerceStoreDetailByID(GetIDRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/EcommerceStore/V1/GetEcommerceStoreDetailByID", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpGet("GetEStoreImage")]
        public async Task<ActionResult> GetEStoreImage()
        {
            var result = await _srvClient.HttpGetAsync<ResultModel<dynamic>>("api/EcommerceStore/V1/GetEStoreImage");
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpGet("GetStoreOption")]
        public async Task<ActionResult> GetStoreOption()
        {
            var result = await _srvClient.HttpGetAsync<ResultModel<dynamic>>("api/EcommerceStore/V1/GetStoreOption");
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpGet("GetEStoreTag")]
        public async Task<ActionResult> GetEStoreTag()
        {
            var result = await _srvClient.HttpGetAsync<ResultModel<dynamic>>("api/EcommerceStore/V1/GetEStoreTag");
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpGet("GetEStoreOptionsAll")]
        public async Task<ActionResult> GetEStoreOptionsAll()
        {
            var result = await _srvClient.HttpGetAsync<ResultModel<dynamic>>("api/EcommerceStore/V1/GetEStoreOptionsAll");
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("GetECStore")]
        public async Task<ActionResult> GetECStore(GetECStoreRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/EcommerceStore/V1/GetECStore", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("UpdECStore")]
        public async Task<ActionResult> UpdECStore(UpdECStoreRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/EcommerceStore/V1/UpdECStore", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("GetOptionAllSetting")]
        public async Task<ActionResult> GetOptionAllSetting(GetOptionAllSettingRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/EcommerceStore/V1/GetOptionAllSetting", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpGet("GetStoreNumber")]
        public async Task<ActionResult> GetStoreNumber()
        {
            var result = await _srvClient.HttpGetAsync<ResultModel<dynamic>>("api/EcommerceStore/V1/GetStoreNumber");
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