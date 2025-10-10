using CityHubCore.Infrastructure.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POVWebDomain.Models.DB.POVWeb;
using POVWebDomain.Models.API.StoreSrv.MachineSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.Extensions.Configuration;

namespace StoreSrv.Controllers
{
    public class MachineSetController : ApiControllerBase
    {
        private readonly ILogger<HelloController> _logger;
        private readonly POVWebDbContext _POVWebDb;
        private readonly POVWebDbContextDapper _POVWebDbContextDapper;
        private readonly HqSrvClient _HqSrvClient;
        private readonly IConfiguration _Config;

        public MachineSetController(
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
        [HttpPost("V1/GetMachineSet")]
        public async Task<ActionResult> GetMachineSet(GetMachineSetRequest request)
        {
            // Check parameters
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

            // Initial variables

            // Biz
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/MachineSet/V1/GetMachineSet", request);

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
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
        [HttpPost("V1/UpdMachineSetFrontEndMemo")]
        public async Task<ActionResult> UpdMachineSetFrontEndMemo(UpdMachineSetFrontEndMemoRequest request)
        {
            // Check parameters
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

            // Initial variables

            // Biz
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/MachineSet/V1/UpdMachineSetFrontEndMemo", request);

            if (result is null || result.Succeeded is false || result.Data is null)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
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
        [HttpPost("V1/UpdHeartbeat")]
        public async Task<ActionResult> UpdHeartbeat(UpdHeartbeatRequest request) {
            // Check parameters
            #region Check parameters
            if (string.IsNullOrEmpty(request.SellBranch))
                return BadRequest(FormatResultModel<dynamic>.Failure(
                    new {
                        MSG = "請輸入銷售分店"
                    }));
            if (string.IsNullOrEmpty(request.TerminalID))
                return BadRequest(FormatResultModel<dynamic>.Failure(
                    new {
                        MSG = "請輸入機台號"
                    }));
            #endregion

            // Biz
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<object>>("api/MachineSet/V1/UpdHeartbeat", request);

            if (result is null || result.Succeeded is false || result.Data is null) {
                return Ok(FormatResultModel<dynamic>.Failure(result.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetGroupProgram")]
        public async Task<ActionResult> GetGroupProgram(GetGroupProgramRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/MachineSet/V1/GetGroupProgram", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/EmployeeLogin")]
        public async Task<ActionResult> EmployeeLogin(EmployeeLoginRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/MachineSet/V1/EmployeeLogin", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/GetBookRemark")]
        public async Task<ActionResult> GetBookRemark(GetBookRemarkRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/MachineSet/V1/GetBookRemark", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }

        [HttpPost("V1/UpdBookRemark")]
        public async Task<ActionResult> UpdBookRemark(UpdBookRemarkRequest request)
        {
            var result = await _HqSrvClient.HttpPostAsync<ResultModel<dynamic>>("api/MachineSet/V1/UpdBookRemark", request);

            if (result is null || result.Succeeded is false)
            {
                return Ok(FormatResultModel<dynamic>.Failure(result?.Data));
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(result.Data));
        }
    }
}
