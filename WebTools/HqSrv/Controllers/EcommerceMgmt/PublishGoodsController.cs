using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POVWebDomain.Models.DB.POVWeb;
using Microsoft.Extensions.Configuration;
using CityHubCore.Infrastructure.API;
using Dapper;
using System.Threading.Tasks;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Reflection.PortableExecutable;
using AutoMapper;
using System.Data.SqlTypes;
using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using HqSrv.Application.Services.ExternalApiServices.Store91;
using Microsoft.AspNetCore.Http;
using POVWebDomain.Models.ExternalApi.Store91;
using HqSrv.Repository.EcommerceMgmt;
using HqSrv.Repository.MainTableMgmt;
using Newtonsoft.Json.Linq;
using HqSrv.Factories.Ecommerce;
using POVWebDomain.Models.ExternalApi.Momo;
using POVWebDomain.Models.ExternalApi.ShopeeSCM;
using POVWebDomain.Models.API.StoreSrv.Common;
using POVWebDomain.Models.ExternalApi;
using IndexList = POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.IndexList;

namespace HqSrv.Controllers.EcommerceMgmt

{
    public class PublishGoodsController : ApiControllerBase
    {
        private readonly ILogger<PublishGoodsController> _logger;
        private readonly POVWebDbContext _POVWebDb;
        private readonly POVWebDbContextDapper _POVWebDbContextDapper;
        private readonly CityAdminSrvClient _CityAdminSrvClient;
        private readonly IConfiguration _Config;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly Store91ExternalApiService __91Api;
        private readonly PublishGoodsRepository _PublishGoodsRepository;
        private readonly IEcommerceFactoryManager _ecommerceFactoryManager;

        public PublishGoodsController(
            ILogger<PublishGoodsController> logger
            , POVWebDbContext pOVWebDbContext
            , POVWebDbContextDapper pOVWebDbContextDapper
            , CityAdminSrvClient cityAdminSrvClient
            , IConfiguration config
            , IWebHostEnvironment hostEnvironment
            , Store91ExternalApiService _91api
            , PublishGoodsRepository publishGoodsRepository
            , IEcommerceFactoryManager ecommerceFactoryManager
            )
        {
            _logger = logger;
            _POVWebDb = pOVWebDbContext;
            _POVWebDbContextDapper = pOVWebDbContextDapper;
            _CityAdminSrvClient = cityAdminSrvClient;
            _Config = config;
            _hostEnvironment = hostEnvironment;
            __91Api = _91api;
            _PublishGoodsRepository = publishGoodsRepository;
            _ecommerceFactoryManager = ecommerceFactoryManager;
        }

        [HttpPost("V1/GetOptionAll")]
        public async Task<ActionResult> GetOptionAll(GetOptionAllRequest request)
        {
            try
            {
                

                // 91App
                dynamic shipType_91app, payment, specChart, shopCategory, salesModeType, sellingDateTime ;

                shipType_91app = ((dynamic)(await __91Api.GetShipping(new GetShippingRequest { })).Item1)?.responseBody;
                payment = ((dynamic)(await __91Api.GetPayment(new GetPaymentRequest { })).Item1)?.responseBody;
                specChart = ((dynamic)(await __91Api.SalePageSpecChartGetList(new SalePageSpecChartGetListRequest
                {
                    SearchItem = new SearchItem(),
                    Skip = 0,
                    Take = 50
                })).Item1)?.responseBody;
                shopCategory = ((dynamic)(await __91Api.GetShopCategory(new GetShopCategoryRequest { })).Item1)?.responseBody;
                salesModeType = ((dynamic)(await __91Api.GetSalesModeType()).Item1)?.responseBody;
                sellingDateTime = ((dynamic)(await __91Api.GetSellingDateTime()).Item1)?.responseBody;

             
                // 屬性
                dynamic ecIndex;
                ecIndex = await _PublishGoodsRepository.MergeEcAttributes(platforms: new List<string> { "0002", "0003", "0004" }, categoryCode: "your_category_code");


                return Ok(FormatResultModel<dynamic>.Success(new {  shipType_91app, payment, specChart, shopCategory, salesModeType, sellingDateTime, ecIndex }));
            }
            catch (Exception e)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
            }
        }


        [HttpPost("V1/TestApi")]
        public async Task<ActionResult> TestApi()
        {
            try
            {

                dynamic result;
                result = ((dynamic)(await __91Api.GetCategory(new GetCategoryRequest { })).Item1);

                return Ok(FormatResultModel<dynamic>.Success(new { result }));
            }
            catch (Exception e)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
            }
        }

        [HttpPost("V1/TestImageUploadApi")]
        public async Task<ActionResult> TestImageUploadApi([FromForm] UploadImageRequest request)
        {
            try
            {

                dynamic result;
                result = ((dynamic)(await __91Api.GetCategory(new GetCategoryRequest { })).Item1);

                return Ok(FormatResultModel<dynamic>.Success(new { result }));
            }
            catch (Exception e)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
            }
        }
        [HttpPost("V1/SubmitMain")]
        public async Task<ActionResult> SubmitMain([FromForm] SubmitMainRequestAll request)
        {
            try
            {
                SubmitMainResponseAll apiResponse = new SubmitMainResponseAll();
                (object, string) resultDB = (null, "");
                List<string> errorStr = new List<string>();


                POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.SubmitMainRequest basicInfo = JsonConvert.DeserializeObject<POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods.SubmitMainRequest>(request.BasicInfo);

                await _PublishGoodsRepository.HandleImage(request);
                await _PublishGoodsRepository.SaveSubmitGoodsReq(request);


                List<StoreSetting> storeSettings = JsonConvert.DeserializeObject<List<StoreSetting>>(request.StoreSettings);

                foreach (StoreSetting store in storeSettings)
                {
                    if (store.Publish == false ) continue;
                    //if (store.Publish == false || store.CategoryId == null) continue;

                    var factory = _ecommerceFactoryManager.GetFactory(
                        store.EStoreID switch
                        {
                            "0001" => "91App",
                            "0002" => "Yahoo",
                            "0003" => "Momo",
                            "0004" => "Shopee",
                            _ => null
                        }
                    );
                    var service = factory.CreateEcommerceService();

                    GetSubmitModeReponse resData = await _PublishGoodsRepository.GetSubmitResByStore(request.ParentID, store.PlatformID);
                    GetLookupAndCommonValueResponse commonInfo = await _PublishGoodsRepository.GetLookupAndCommonValue(request.ParentID, store.EStoreID);

                    object requestDto;
                    if (resData == null)
                    {

                        requestDto = await factory.CreateRequestDtoAdd(request, store, commonInfo);
                        var response = await service.SubmitGoodsAdd(requestDto, store.PlatformID);
                        apiResponse.Response = response.Item1;
                        if (!string.IsNullOrEmpty(response.Item2)) errorStr.Add(response.Item2);
                    }
                    else
                    {
                        requestDto = factory.CreateRequestDtoEdit(request, resData.RequestParams, resData.ResponseData, store);
                        apiResponse.Response = (await service.SubmitGoodsEdit(requestDto, store.PlatformID)).Item1;
                    }

                    resultDB = await _PublishGoodsRepository.SaveSubmitGoodsRes(request, requestDto, apiResponse, store);
                    if (!string.IsNullOrEmpty(resultDB.Item2)) errorStr.Add(resultDB.Item2);
                }


                if (errorStr.Count==0)
                {
                    return Ok(FormatResultModel<dynamic>.Success(null));
                }
                else
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = errorStr }));
                }
            }
            catch (Exception e)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
            }

        }

        [HttpPost("V1/SavePicture")]
        public async Task<ActionResult> SavePicture([FromForm] SavePictureRequest request)
        {
            try
            {
                var result = await _PublishGoodsRepository.SavePicture(request.Image,request.BaseName, request.Type, request.Index);
                if (string.IsNullOrEmpty(result.Item2))
                {
                    return Ok(FormatResultModel<dynamic>.Success(result.Item1));
                }
                else
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = result.Item2 }));
                }
            }
            catch (Exception e)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
            }

        }

      
        [HttpPost("V1/GetSubmitMode")]
        public async Task<ActionResult> GetSubmitMode(GetSubmitModeRequest request)
        {
            try
            {
                var result = await _PublishGoodsRepository.GetSubmitMode(request);
                if (string.IsNullOrEmpty(result.Item2))
                {
                    return Ok(FormatResultModel<dynamic>.Success(result.Item1));
                }
                else
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = result.Item2 }));
                }
            }
            catch (Exception e)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
            }
        }

        [HttpPost("V1/GetSubmitDefVal")]
        public async Task<ActionResult> GetSubmitDefVal(GetSubmitDefValRequest request)
        {
            try
            {
                var result = await _PublishGoodsRepository.GetSubmitDefVal(request);
                if (string.IsNullOrEmpty(result.Item2))
                {
                    return Ok(FormatResultModel<dynamic>.Success(result.Item1));
                }
                else
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = result.Item2 }));
                }
            }
            catch (Exception e)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
            }

        }

        [HttpGet("V1/GetEStoreCatOptions")]
        public async Task<ActionResult> GetEStoreCatOptions()
        {
            try
            {
                var result = await _PublishGoodsRepository.GetEStoreCatOptions();
                if (string.IsNullOrEmpty(result.Item2))
                {
                    return Ok(FormatResultModel<dynamic>.Success(result.Item1));
                }
                else
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = result.Item2 }));
                }
            }
            catch (Exception e)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
            }
        }
    }
}
