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
using POVWebDomain.Models.API.StoreSrv.DashBoard;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StoreSrv.Controllers.DashBoard
{
    public class DashboardController : ApiControllerBase
    {
        private readonly ILogger<HelloController> _logger;
        private readonly POVWebDbContext _POVWebDb;
        private readonly POVWebDbContextDapper _POVWebDbContextDapper;
        private readonly HqSrvClient _HqSrvClient;
        private readonly IConfiguration _Config;

        public DashboardController(
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

        [HttpPost("V1/GetPeersSaleEstimateData")]
        public async Task<ActionResult> GetPeersSaleEstimateData(GetPeersSaleEstimateDataRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/Dashboard/V1/GetPeersSaleEstimateData", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/CheckInvoiceData")]
        public async Task<ActionResult> CheckInvoiceData(CheckInvoiceRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/Dashboard/V1/CheckInvoiceData", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetInvoiceData")]
        public async Task<ActionResult> GetInvoiceData(GetInvoiceRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/Dashboard/V1/GetInvoiceData", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetCompanyTemplateData")]
        public async Task<ActionResult> GetCompanyTemplateData(GetCompanyTemplateDataRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/Dashboard/V1/GetCompanyTemplateData", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }
    }
    
}
