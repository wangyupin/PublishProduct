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
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.EcommerceStore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StoreSrv.Controllers.DashBoard
{
    public class EcommerceStoreController : ApiControllerBase
    {
        private readonly ILogger<HelloController> _logger;
        private readonly POVWebDbContext _POVWebDb;
        private readonly POVWebDbContextDapper _POVWebDbContextDapper;
        private readonly HqSrvClient _HqSrvClient;
        private readonly IConfiguration _Config;

        public EcommerceStoreController(
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

        [HttpGet("V1/GetEcommerceStoreData")]
        public async Task<ActionResult> GetEcommerceStoreData()
        {
            var result = await _HqSrvClient.HttpGetAsync<ResultModel<object>>("api/EcommerceStore/V1/GetEcommerceStoreData");

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetSingleData")]
        public async Task<ActionResult> GetSingleData(GetSingleDataRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/EcommerceStore/V1/GetSingleData", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/AddEcommerceStoreData")]
        public async Task<ActionResult> AddEcommerceStoreData(AddEcommerceStoreDataRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/EcommerceStore/V1/AddEcommerceStoreData", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/UpdEcommerceStore")]
        public async Task<ActionResult> UpdEcommerceStore(UpdEcommerceStoreDataRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/EcommerceStore/V1/UpdEcommerceStore", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/DelEcommerceStoreData")]
        public async Task<ActionResult> DelEcommerceStoreData(DelEcommerceStoreDataRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/EcommerceStore/V1/DelEcommerceStoreData", request);

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetEcommerceStoreDetailByID")]
        public async Task<ActionResult> GetEcommerceStoreDetailByID(GetIDRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/EcommerceStore/V1/GetEcommerceStoreDetailByID", request);

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpGet("V1/GetEStoreImage")]
        public async Task<ActionResult> GetEStoreImage()
        {
            var result = await _HqSrvClient.HttpGetAsync<ResultModel<object>>("api/EcommerceStore/V1/GetEStoreImage");

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpGet("V1/GetStoreOption")]
        public async Task<ActionResult> GetStoreOption()
        {
            var result = await _HqSrvClient.HttpGetAsync<ResultModel<object>>("api/EcommerceStore/V1/GetStoreOption");

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpGet("V1/GetEStoreTag")]
        public async Task<ActionResult> GetEStoreTag()
        {
            var result = await _HqSrvClient.HttpGetAsync<ResultModel<object>>("api/EcommerceStore/V1/GetEStoreTag");

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpGet("V1/GetEStoreOptionsAll")]
        public async Task<ActionResult> GetEStoreOptionsAll()
        {
            var result = await _HqSrvClient.HttpGetAsync<ResultModel<dynamic>>("api/EcommerceStore/V1/GetEStoreOptionsAll");

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetECStore")]
        public async Task<ActionResult> GetECStore(GetECStoreRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/EcommerceStore/V1/GetECStore", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/UpdECStore")]
        public async Task<ActionResult> UpdECStore(UpdECStoreRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/EcommerceStore/V1/UpdECStore", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetOptionAllSetting")]
        public async Task<ActionResult> GetOptionAllSetting(GetOptionAllSettingRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/EcommerceStore/V1/GetOptionAllSetting", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }
    }
    
}
