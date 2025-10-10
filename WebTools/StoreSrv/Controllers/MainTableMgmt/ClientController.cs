using CityHubCore.Infrastructure.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POVWebDomain.Models.DB.POVWeb;
using POVWebDomain.Models.API.StoreSrv.MainTableMgmt.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.Extensions.Configuration;
using POVWebDomain.Models.API.StoreSrv.Common;

namespace StoreSrv.Controllers.MainTableMgmt
{
    public class ClientController : ApiControllerBase
    {
        private readonly ILogger<HelloController> _logger;
        private readonly POVWebDbContext _POVWebDb;
        private readonly POVWebDbContextDapper _POVWebDbContextDapper;
        private readonly HqSrvClient _HqSrvClient;
        private readonly IConfiguration _Config;

        public ClientController(
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

        /// <summary>
        /// 門市(庫點)清冊；可複合條件查詢，可自訂義排序
        /*
        {
          "clientID_From": "00000000",
          "clientID_To": "10000000",
          "clientName_Like": "三重",
          "orderBy": "ClientID"
        }
        */
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("V1/GetClientList")]
        public async Task<ActionResult> GetClientList(GetClientListRequest request)
        {
            // Check parameters

            // Initial variables

            // Biz
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/Client/V1/GetClientList", request);

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        /// <summary>
        /// 門市(庫點)線上輔助查詢用；編號+簡稱+名稱，可自訂義排序預設為ClientID
        /*
        {
          "clientID_ClientShort_ClientName_Like": "愛迪",
          "orderBy": "ClientShort"
        }        
        */
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("V1/GetClientHelp")]
        public async Task<ActionResult> GetClientHelp(GetClientHelpRequest request)
        {
            // Check parameters

            // Initial variables

            // Biz
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/Client/V1/GetClientHelp", request);

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        /// <summary>
        /// 門市(庫點)編號檢查
        /*
        {
          "clientID": "123"
        }        
        */
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("V1/GetClientCheck")]
        public async Task<ActionResult> GetClientCheck(GetClientCheckRequest request)
        {
            // Check parameters
            if (string.IsNullOrEmpty(request.ClientID))
                return BadRequest(FormatResultModel<dynamic>.Failure(
                    new
                    {
                        MSG = "請輸入編號"
                    }));

            // Initial variables

            // Biz
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/Client/V1/GetClientCheck", request);

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetClientDetailByID")]
        public async Task<ActionResult> GetClientDetailByID(GetClientDetailByIDRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/Client/V1/GetClientDetailByID", request);

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        /// <summary>
        /// 新增客戶基本資料
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("V1/AddClient")]
        public async Task<ActionResult> AddClient(AddClientRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/Client/V1/AddClient", request);

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        /// <summary>
        /// 修改客戶基本資料
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("V1/UpdClient")]
        public async Task<ActionResult> UpdClient(UpdClientRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/Client/V1/UpdClient", request);

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        /// <summary>
        /// 刪除客戶基本資料
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("V1/DelClient")]
        public async Task<ActionResult> DelClient(DelClientRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/Client/V1/DelClient", request);

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetCatenationIDHelp")]
        public async Task<ActionResult> GetCatenationIDHelp(GetCatenationIDHelpRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/Client/V1/GetCatenationIDHelp", request);

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetClientOptions")]
        public async Task<ActionResult> GetClientOptions(GetClientStoreRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/GetClientOptions", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }
        [HttpPost("V1/GetClientMeStoreOptions")]
        public async Task<ActionResult> GetClientMeStoreOptions(GetClientMeStoreRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/GetClientMeStoreOptions", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpGet("V1/GetClientOptionsAll")]
        public async Task<ActionResult> GetClientOptionsAll()
        {
            var result = await _HqSrvClient.HttpGetAsync<ResultModel<dynamic>>("api/Client/V1/GetClientOptionsAll");

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpGet("V1/GetDistributorOptionsAll")]
        public async Task<ActionResult> GetDistributorOptionsAll()
        {
            var result = await _HqSrvClient.HttpGetAsync<ResultModel<dynamic>>("api/Client/V1/GetDistributorOptionsAll");

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpGet("V1/GetCatenationIDOptionsAll")]
        public async Task<ActionResult> GetCatenationIDOptionsAll()
        {
            var result = await _HqSrvClient.HttpGetAsync<ResultModel<dynamic>>("api/Client/V1/GetCatenationIDOptionsAll");

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpGet("V1/GetEStoreOptionsAll")]
        public async Task<ActionResult> GetEStoreOptionsAll()
        {
            var result = await _HqSrvClient.HttpGetAsync<ResultModel<dynamic>>("api/Client/V1/GetEStoreOptionsAll");

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpGet("V1/GetDepOptionsAll")]
        public async Task<ActionResult> GetDepOptionsAll()
        {
            var result = await _HqSrvClient.HttpGetAsync<ResultModel<dynamic>>("api/Client/V1/GetDepOptionsAll");

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetClientOptionsExcludeMainWH")]
        public async Task<ActionResult> GetClientOptionsExcludeMainWH(GetClientOptionsExcludeMainWHRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/GetClientOptionsExcludeMainWH", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }
        [HttpPost("V1/GetClientOptionsOnlyMainWH")]
        public async Task<ActionResult> GetClientOptionsOnlyMainWH(GetClientOptionsOnlyMainWHRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/GetClientOptionsOnlyMainWH", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetBillCustOptions")]
        public async Task<ActionResult> GetBillCustOptions(GetBillCustOptionsRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/GetBillCustOptions", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }
        [HttpGet("V1/GetClientOptionsAllWithCompany")]
        public async Task<ActionResult> GetClientOptionsAllWithCompany()
        {
            var result = await _HqSrvClient.HttpGetAsync<ResultModel<dynamic>>("api/Client/V1/GetClientOptionsAllWithCompany");

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }
        [HttpGet("V1/GetCompanyOptions")]
        public async Task<ActionResult> GetCompanyOptions()
        {
            var result = await _HqSrvClient.HttpGetAsync<ResultModel<dynamic>>("api/Client/V1/GetCompanyOptions");

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpGet("V1/GetClientHelp")]
        public async Task<ActionResult> GetClientHelp()
        {
            var result = await _HqSrvClient.HttpGetAsync<ResultModel<object>>("api/Client/V1/GetClientHelp");

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetData")]
        public async Task<ActionResult> GetData(GetDataRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/Client/V1/GetData", request);

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetSingleData")]
        public async Task<ActionResult> GetSingleData(GetSingleDataRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/Client/V1/GetSingleData", request);

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/AddClientData")]
        public async Task<ActionResult> AddClientData(AddClientDataRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/Client/V1/AddClientData", request);

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/DelClientData")]
        public async Task<ActionResult> DelClientData(DelClientDataRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/Client/V1/DelClientData", request);

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/UpdClientData")]
        public async Task<ActionResult> UpdClientData(UpdClientDataRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/Client/V1/UpdClientData", request);

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/ImportClient")]
        public async Task<ActionResult> ImportClient([FromForm] ImportRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsyncFormData<ResultModel<dynamic>>("api/Client/V1/ImportClient", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetClientIsConsignment")]
        public async Task<ActionResult> GetClientIsConsignment(GetClientCheckRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/GetClientIsConsignment", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetClientHelpOffset")]
        public async Task<ActionResult> GetClientHelpOffset(GetDataRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/GetClientHelpOffset", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }
    }
}