using CityHubCore.Application.Session;
using CityHubCore.Infrastructure.API;
using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using POVWebDomain.Models.API.StoreSrv.MachineSet;
using System.Threading.Tasks;

namespace StoreWeb.Controllers
{
    public class MachineSetController : ApiControllerBase
    {
        private readonly ILogger<MachineSetController> _logger;
        private readonly StoreSrvClient _srvClient;
        private readonly IDistributedCache _cache;

        public MachineSetController(ILogger<MachineSetController> logger, StoreSrvClient srvClient, IDistributedCache cache)
        {
            _logger = logger;
            _srvClient = srvClient;
            _cache = cache;
        }

        /// <summary>
        /// 讀取門市機台設定資訊
        /*
        {
          "sellBranch": "001",
          "terminalID": "A"
        }        
        */
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("GetMachineSet")]
        public async Task<ActionResult> GetMachineSet(GetMachineSetRequest request)
        {
            #region Check parameters
            if (string.IsNullOrEmpty(request.SellBranch))
                return BadRequest(FormatResultModel<dynamic>.Failure(
                    new
                    {
                        MSG = "請輸入銷售分店"
                    }));
            if (string.IsNullOrEmpty(request.TerminalID))
                return BadRequest(FormatResultModel<dynamic>.Failure(
                    new
                    {
                        MSG = "請輸入機台號"
                    }));
            #endregion

            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/MachineSet/V1/GetMachineSet", request);
            if (result is object && result.Succeeded)
            {
                return Ok(new
                {
                    machineSet = result.Data
                });
            }
            else
            {
                return NotFound(result);
            }
        }

        /// <summary>
        /// 更新門市機台設定FrontEndMemo欄位值
        /*
        {
          "sellBranch": "001",
          "terminalID": "A",
          "frontEndMemo": "JSON String Modify"
        }        
        */
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("UpdMachineSetFrontEndMemo")]
        public async Task<ActionResult> UpdMachineSetFrontEndMemo(UpdMachineSetFrontEndMemoRequest request)
        {

            #region Check parameters
            if (string.IsNullOrEmpty(request.SellBranch))
                return BadRequest(FormatResultModel<dynamic>.Failure(
                    new
                    {
                        MSG = "請輸入銷售分店"
                    }));
            if (string.IsNullOrEmpty(request.TerminalID))
                return BadRequest(FormatResultModel<dynamic>.Failure(
                    new
                    {
                        MSG = "請輸入機台號"
                    }));
            #endregion

            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/MachineSet/V1/UpdMachineSetFrontEndMemo", request);
            if (result is object && result.Succeeded)
            {
                return Ok(new
                {
                    machineSet = result.Data
                });
            }
            else
            {
                return NotFound(result);
            }
        }

        /// <summary>
        /// Machine heartbeat
        /*
        {
          "sellBranch": "001",
          "terminalID": "A",
          "Status": "0000"
        }        
        */
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("UpdHeartbeat")]
        public async Task<ActionResult> UpdHeartbeat(UpdHeartbeatRequest request)
        {

            #region Check parameters
            if (string.IsNullOrEmpty(request.SellBranch))
                return BadRequest(FormatResultModel<dynamic>.Failure(
                    new
                    {
                        MSG = "請輸入銷售分店"
                    }));
            if (string.IsNullOrEmpty(request.TerminalID))
                return BadRequest(FormatResultModel<dynamic>.Failure(
                    new
                    {
                        MSG = "請輸入機台號"
                    }));
            #endregion

            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/MachineSet/V1/UpdHeartbeat", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("UpdPIDStatus")]
        public ActionResult UpdPIDStatus(UpdPIDStatusRequest request)
        {
            var data = $"{request.IsBusy}-{request.IsFueling}-{request.Volume}-{request.Amount}";
            PrinterHelper.UpdPrinterStatueFromCache(_cache, data);
            return Ok();
        }

        [HttpPost("GetPIDStatus")]
        public ActionResult GetPIDStatus(UpdPIDStatusRequest request)
        {
            var data = PrinterHelper.GetPrinterStatueFromCache(_cache);
            return Ok(new { data });
        }

        [HttpPost("GetGroupProgram")]
        public async Task<ActionResult> GetGroupProgram(GetGroupProgramRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/MachineSet/V1/GetGroupProgram", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("EmployeeLogin")]
        public async Task<ActionResult> EmployeeLogin(EmployeeLoginRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/MachineSet/V1/EmployeeLogin", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("GetBookRemark")]
        public async Task<ActionResult> GetBookRemark(GetBookRemarkRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/MachineSet/V1/GetBookRemark", request);
            if (result is object && result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

        [HttpPost("UpdBookRemark")]
        public async Task<ActionResult> UpdBookRemark(UpdBookRemarkRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<dynamic>>("api/MachineSet/V1/UpdBookRemark", request);
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
