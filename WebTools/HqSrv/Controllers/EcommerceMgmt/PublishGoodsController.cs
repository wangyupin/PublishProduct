using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POVWebDomain.Models.DB.POVWeb;
using Microsoft.Extensions.Configuration;
using CityHubCore.Infrastructure.API;
using System.Threading.Tasks;
using System;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using HqSrv.Application.Services.EcommerceMgmt;

namespace HqSrv.Controllers.EcommerceMgmt
{
    public class PublishGoodsController : ApiControllerBase
    {
        private readonly ILogger<PublishGoodsController> _logger;
        private readonly IPublishGoodsApplicationService _applicationService;

        public PublishGoodsController(
            ILogger<PublishGoodsController> logger,
            IPublishGoodsApplicationService applicationService)
        {
            _logger = logger;
            _applicationService = applicationService;
        }

        [HttpPost("V1/GetOptionAll")]
        public async Task<ActionResult> GetOptionAll(GetOptionAllRequest request)
        {
            try
            {
                var result = await _applicationService.GetPublishOptionsAsync(request);

                if (result.IsFailure)
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = result.Error.Message }));

                return Ok(FormatResultModel<dynamic>.Success(result.Data));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetOptionAll 發生錯誤");
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
            }
        }

        [HttpPost("V1/SubmitMain")]
        public async Task<ActionResult> SubmitMain([FromForm] SubmitMainRequestAll request)
        {
            try
            {
                var result = await _applicationService.SubmitProductAsync(request);

                if (result.IsFailure)
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = result.Error.Message }));

                return Ok(FormatResultModel<dynamic>.Success(result.Data));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "SubmitMain 發生錯誤");
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
            }
        }

        [HttpPost("V1/GetSubmitMode")]
        public async Task<ActionResult> GetSubmitMode(GetSubmitModeRequest request)
        {
            try
            {
                var result = await _applicationService.GetSubmitModeAsync(request);

                if (result.IsFailure)
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = result.Error.Message }));

                return Ok(FormatResultModel<dynamic>.Success(result.Data));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetSubmitMode 發生錯誤");
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
            }
        }

        [HttpPost("V1/GetSubmitDefVal")]
        public async Task<ActionResult> GetSubmitDefVal(GetSubmitDefValRequest request)
        {
            try
            {
                var result = await _applicationService.GetSubmitDefaultValuesAsync(request);

                if (result.IsFailure)
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = result.Error.Message }));

                return Ok(FormatResultModel<dynamic>.Success(result.Data));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetSubmitDefVal 發生錯誤");
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
            }
        }

        [HttpGet("V1/GetEStoreCatOptions")]
        public async Task<ActionResult> GetEStoreCatOptions()
        {
            try
            {
                var result = await _applicationService.GetEStoreCategoryOptionsAsync();

                if (result.IsFailure)
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = result.Error.Message }));

                return Ok(FormatResultModel<dynamic>.Success(result.Data));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetEStoreCatOptions 發生錯誤");
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
            }
        }
    }
}