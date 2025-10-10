using CityHubCore.Application;
using CityHubCore.Application.Attributies;
using CityHubCore.Infrastructure.API;
using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using POVWebDomain.Models.API.StoreSrv.Common;
using POVWebDomain.Models.API.StoreSrv.MainTableMgmt.Client;
using System;
using System.Threading.Tasks;

namespace StoreWeb.Controllers
{
    public class ClientController : ApiControllerBase
    {
        private readonly ILogger<ClientController> _logger;
        private readonly StoreSrvClient _srvClient;

        public ClientController(ILogger<ClientController> logger, StoreSrvClient srvClient)
        {
            _logger = logger;
            _srvClient = srvClient;
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
        [HttpPost("GetClientList")]
        public async Task<ActionResult> GetClientList(GetClientListRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/GetClientList", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
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
        [HttpPost("GetClientHelp")]
        public async Task<ActionResult> GetClientHelp(GetClientHelpRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/GetClientHelp", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
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
        [HttpPost("GetClientCheck")]
        public async Task<ActionResult> GetClientCheck(GetClientCheckRequest request)
        {
            // Check parameters
            if (string.IsNullOrEmpty(request.ClientID))
                return BadRequest(FormatResultModel<dynamic>.Failure(
                    new
                    {
                        MSG = "請輸入編號"
                    }));

            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/GetClientCheck", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("GetClientDetailByID")]
        public async Task<ActionResult> GetClientDetailByID(GetClientDetailByIDRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/GetClientDetailByID", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("AddClient")]
        public async Task<ActionResult> AddClient(AddClientRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/AddClient", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("UpdClient")]
        public async Task<ActionResult> UpdClient(UpdClientRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/UpdClient", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("DelClient")]
        public async Task<ActionResult> DelClient(DelClientRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/DelClient", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("GetCatenationIDHelp")]
        public async Task<ActionResult> GetCatenationIDHelp(GetCatenationIDHelpRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/GetCatenationIDHelp", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }
        [HttpPost("GetClientMeStoreOptions")]
        public async Task<ActionResult> GetClientMeStoreOptions(GetClientMeStoreRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/GetClientMeStoreOptions", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }
        [HttpPost("GetClientOptions")]
        public async Task<ActionResult> GetClientOptions(GetClientStoreRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/GetClientOptions", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpGet("GetClientOptionsAll")]
        public async Task<ActionResult> GetClientOptionsAll()
        {
            var result = await _srvClient.HttpGetAsync<ResultModel<dynamic>>("api/Client/V1/GetClientOptionsAll");
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpGet("GetDistributorOptionsAll")]
        public async Task<ActionResult> GetCliGetDistributorOptionsAllentOptionsAll()
        {
            var result = await _srvClient.HttpGetAsync<ResultModel<dynamic>>("api/Client/V1/GetDistributorOptionsAll");
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpGet("GetCatenationIDOptionsAll")]
        public async Task<ActionResult> GetCatenationIDOptionsAll()
        {
            var result = await _srvClient.HttpGetAsync<ResultModel<dynamic>>("api/Client/V1/GetCatenationIDOptionsAll");
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
            var result = await _srvClient.HttpGetAsync<ResultModel<dynamic>>("api/Client/V1/GetEStoreOptionsAll");
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpGet("GetDepOptionsAll")]
        public async Task<ActionResult> GetDepOptionsAll()
        {
            var result = await _srvClient.HttpGetAsync<ResultModel<dynamic>>("api/Client/V1/GetDepOptionsAll");
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("GetClientOptionsExcludeMainWH")]
        public async Task<ActionResult> GetClientOptionsExcludeMainWH(GetClientOptionsExcludeMainWHRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/GetClientOptionsExcludeMainWH", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }
        

        [HttpPost("GetClientOptionsOnlyMainWH")]
        public async Task<ActionResult> GetClientOptionsOnlyMainWH(GetClientOptionsOnlyMainWHRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/GetClientOptionsOnlyMainWH", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("GetBillCustOptions")]
        public async Task<ActionResult> GetBillCustOptions(GetBillCustOptionsRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/GetBillCustOptions", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpGet("GetClientOptionsAllWithCompany")]
        public async Task<ActionResult> GetClientOptionsAllWithCompany()
        {
            var result = await _srvClient.HttpGetAsync<ResultModel<dynamic>>("api/Client/V1/GetClientOptionsAllWithCompany");
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }
        [HttpGet("GetCompanyOptions")]
        public async Task<ActionResult> GetCompanyOptions()
        {
            var result = await _srvClient.HttpGetAsync<ResultModel<dynamic>>("api/Client/V1/GetCompanyOptions");
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }
        [HttpGet("GetClientHelp")]
        public async Task<ActionResult> GetClientHelp()
        {
            var result = await _srvClient.HttpGetAsync<ResultModel<dynamic>>("api/Client/V1/GetClientHelp");
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("GetData")]
        public async Task<ActionResult> GetData(GetDataRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/GetData", request);
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
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/GetSingleData", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("AddClientData")]
        public async Task<ActionResult> AddClientData(AddClientDataRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/AddClientData", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("DelClientData")]
        public async Task<ActionResult> DelClientData(DelClientDataRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/DelClientData", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("UpdClientData")]
        public async Task<ActionResult> UpdClientData(UpdClientDataRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/UpdClientData", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("ImportClient")]
        public async Task<ActionResult> ImportClient([FromForm] ImportRequest request)
        {
            try
            {
                var result = await _srvClient.HttpPostAsyncFormData<ResultModel<dynamic>>("api/Client/V1/ImportClient", request);
                if (result is object && result.Succeeded)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound(result);
                }
            }
            catch (Exception e)
            {
                return NotFound(e);
            }
        }

        [HttpPost("GetClientIsConsignment")]
        public async Task<ActionResult> GetClientIsConsignment(GetClientCheckRequest request)
        {
            try
            {
                var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/GetClientIsConsignment", request);
                if (result is object && result.Succeeded)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound(result);
                }
            }
            catch (Exception e)
            {
                return NotFound(e);
            }
        }

        [HttpPost("GetClientHelpOffset")]
        public async Task<ActionResult> GetClientHelpOffset(GetDataRequest request)
        {
            try
            {
                var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/Client/V1/GetClientHelpOffset", request);
                if (result is object && result.Succeeded)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound(result);
                }
            }
            catch (Exception e)
            {
                return NotFound(e);
            }
        }
    }
}
