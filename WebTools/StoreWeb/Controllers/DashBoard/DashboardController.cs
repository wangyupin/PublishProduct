using Microsoft.AspNetCore.Mvc;
using CityHubCore.Infrastructure.API;
using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using POVWebDomain.Models.API.StoreSrv.DashBoard;


namespace StoreWeb.Controllers.DashBoard
{
    public class DashboardController : ApiControllerBase
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly StoreSrvClient _srvClient;

        public DashboardController(ILogger<DashboardController> logger, StoreSrvClient srvClient)
        {
            _logger = logger;
            _srvClient = srvClient;
        }

        [HttpPost("GetPeersSaleEstimateData")]
        public async Task<ActionResult> GetPeersSaleEstimateData(GetPeersSaleEstimateDataRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Dashboard/V1/GetPeersSaleEstimateData", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("CheckInvoiceData")]
        public async Task<ActionResult> CheckInvoiceData(CheckInvoiceRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Dashboard/V1/CheckInvoiceData", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("GetInvoiceData")]
        public async Task<ActionResult> GetInvoiceData(GetInvoiceRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Dashboard/V1/GetInvoiceData", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("GetCompanyTemplateData")]
        public async Task<ActionResult> GetCompanyTemplateData(GetCompanyTemplateDataRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Dashboard/V1/GetCompanyTemplateData", request);
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