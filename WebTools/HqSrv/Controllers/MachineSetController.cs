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
using Microsoft.Extensions.Configuration;
using CityHubCore.Infrastructure.ServiceClient;
using System.Data;
using Microsoft.Data.SqlClient;
using QRCoder;
using System.Reflection;
using System.Text.Json;


namespace HqSrv.Controllers
{
    public class MachineSetController : ApiControllerBase
    {
        private readonly ILogger<HelloController> _logger;
        private readonly POVWebDbContext _POVWebDb;
        private readonly POVWebDbContextDapper _POVWebDbContextDapper;
        private readonly CityAdminSrvClient _CityAdminSrvClient;
        private readonly IConfiguration _Config;

        public MachineSetController(
            ILogger<HelloController> logger
            , POVWebDbContext pOVWebDbContext
            , POVWebDbContextDapper pOVWebDbContextDapper
            , CityAdminSrvClient cityAdminSrvClient
            , IConfiguration config
            )
        {
            _logger = logger;
            _POVWebDb = pOVWebDbContext;
            _POVWebDbContextDapper = pOVWebDbContextDapper;
            _CityAdminSrvClient = cityAdminSrvClient;
            _Config = config;
        }

        /// <summary>
        /// 讀取門市機台設定資訊
        /*
        {
          "sellBranch": "010",
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
            var paramList = new DynamicParameters();
            paramList.Add("@SellBranch", request.SellBranch);
            paramList.Add("@TerminalID", request.TerminalID);
            paramList.Add("@O_MSG", "", dbType: DbType.String, direction: ParameterDirection.Output);

            string selectStr =
                @"[POVWeb].[uspMachineGet]";

            // Biz
            IEnumerable<dynamic> tmpMachineSet = null;
            try
            {
                tmpMachineSet = await _POVWebDbContextDapper.Connection.QueryAsync(
                selectStr,
                paramList,
                commandType: CommandType.StoredProcedure,
                commandTimeout: 180);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            

            // Result
            var oMsg = paramList.Get<string>("@O_MSG");

            if (string.IsNullOrEmpty(oMsg))
            {
                return Ok(FormatResultModel<dynamic>.Success(
                    tmpMachineSet.FirstOrDefault()
                    ));
            }
            else
            {
                return Ok(FormatResultModel<dynamic>.Failure(
                    new
                    {
                        MSG = oMsg
                    }));
            }
        }

        /// <summary>
        /// 更新門市機台設定FrontEndMemo欄位值
        /*
        {
          "sellBranch": "010",
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
            var paramList = new DynamicParameters();
            paramList.Add("@SellBranch", request.SellBranch);
            paramList.Add("@TerminalID", request.TerminalID);
            paramList.Add("@FrontEndMemo", request.FrontEndMemo);
            paramList.Add("@O_MSG", "", dbType: DbType.String, direction: ParameterDirection.Output);

            string selectStr =
                @"[POVWeb].[uspMachineEditFrontEndMemo]";

            // Biz
            var tmpMachineSet = await _POVWebDbContextDapper.Connection.QueryAsync(
                selectStr,
                paramList,
                commandType: CommandType.StoredProcedure,
                commandTimeout: 180);

            // Result
            var oMsg = paramList.Get<string>("@O_MSG");

            if (string.IsNullOrEmpty(oMsg))
            {
                return Ok(FormatResultModel<dynamic>.Success(
                    tmpMachineSet.FirstOrDefault()
                    ));
            }
            else
            {
                return Ok(FormatResultModel<dynamic>.Failure(
                    new
                    {
                        MSG = oMsg
                    }));
            }
        }

        /// <summary>
        /// Machine heartbeat
        /*
        {
          "sellBranch": "010",
          "terminalID": "A",
          "Status": "0000"
        }        
        */
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("V1/UpdHeartbeat")]
        public async Task<ActionResult> UpdHeartbeat(UpdHeartbeatRequest request) 
        {
            return Ok(FormatResultModel<dynamic>.Success(
                    new {
                        SellBranch = request.SellBranch,
                        TerminalID = request.TerminalID,
                        RC = "0000",
                        ServiceStatus = new {
                            // normal = 1, office line = 0
                            hqSrv = new {
                                status = 1,
                                msg = "正常"
                            },
                            // normal = 1, office line = 0, paper low = -1, paper jam = -2, paper out = -3
                            printer = new {
                                status = 1,
                                msg = "正常"
                            },
                            // normal = 1, office line = 0
                            edc = new {
                                status = 1,
                                msg = "正常"
                            }
                        }
                    }));
        }


        [HttpPost("V1/EmployeeLogin")]
        public async Task<ActionResult> EmployeeLogin(EmployeeLoginRequest request)
        {
            //DECODE
            string decodedPassword = "";
            if (request.EmpPassword == "")
            {
                decodedPassword = "Daizy";
            }
            else
            {
                foreach (char c in request.EmpPassword)
                {
                    char tmp = Convert.ToChar(Convert.ToInt32(c) + 4);
                    decodedPassword += tmp;
                }
                int divider = (int)Math.Ceiling(decodedPassword.Length / 2.0);
                char[] arr = (decodedPassword.Substring(divider) + decodedPassword.Substring(0, decodedPassword.Length % 2 == 0 ? decodedPassword.Length - divider : decodedPassword.Length - divider + 1)).ToCharArray();
                Array.Reverse(arr);
                decodedPassword = new string(arr);
            }
            request.EmpPassword = decodedPassword;

            var paramList = new DynamicParameters();
            paramList.Add("@EmpId", request.EmpId);
            paramList.Add("@EmpPassword", request.EmpPassword);

            DataTable RET1 = new();
            RET1.Columns.Add("Str1");
            paramList.Add("@Permission", RET1.AsTableValuedParameter($"[POVWeb].[udtStr]"));


            string selectSQL = @"
                Declare @O_MSG VARCHAR(MAX)=N''
                IF NOT EXISTS(SELECT 1 FROM Users Where UserID = @EmpId AND Password = @EmpPassword) BEGIN
				    SET @O_MSG = '員工帳號或密碼錯誤!'
				    ;THROW 6636001, @O_MSG, 1;
			    END  

                DECLARE @GroupName VARCHAR(30) = ''

                SELECT UserNumber AS UserID, UserName, GroupName FROM Users WHERE UserID = @EmpId AND Password = @EmpPassword
                SELECT @GroupName = GroupName FROM Users WHERE UserID = @EmpId AND Password = @EmpPassword
                
                SELECT DISTINCT PS.PermissionName Action, CAST(PG.ProgramID AS VARCHAR(5)) AS Subject
                FROM UserGroupPermission GP
                LEFT JOIN Permission PS ON GP.PermissionID = PS.PermissionID
                LEFT JOIN Program PG ON PS.ProgramID = PG.ProgramID
                LEFT JOIN (SELECT VALUE FROM STRING_SPLIT(REPLACE(@GroupName, ', ', ','),',')) S ON GP.Group_Name = S.VALUE
                WHERE S.VALUE IS NOT NULL

            ";

            // Biz
            using (var connection = _POVWebDbContextDapper.Connection)
            {
                connection.Open();
                try
                {
                    var result = await connection.QueryMultipleAsync(
                           sql: selectSQL,
                           paramList,
                           commandTimeout: 180);
                    var empData = result.IsConsumed ? null : result.Read().ToList().FirstOrDefault();
                    var detailList = result.IsConsumed ? null : result.Read().ToList();

                    return Ok(FormatResultModel<dynamic>.Success(new { empData, detailList }));
                }
                catch (SqlException e)
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
                }
            }
        }


        [HttpPost("V1/GetBookRemark")]
        public async Task<ActionResult> GetBookRemark(GetBookRemarkRequest request)
        {

            string selectSQL = @"
                SELECT ISNULL(P.ProgramID, s.value) as ID
                FROM Users U
                CROSS APPLY STRING_SPLIT(U.BookRemark, ',') s
                LEFT JOIN Program p ON p.ProgramID = s.value
                WHERE U.UserID = @EmpID AND U.BookRemark IS NOT NULL
            ";

            // Biz
            using (var connection = _POVWebDbContextDapper.Connection)
            {
                connection.Open();
                try
                {
                    var result = await connection.QueryAsync(
                           sql: selectSQL,
                    request,
                    commandTimeout: 180);

                    return Ok(FormatResultModel<dynamic>.Success(new { result }));
                }
                catch (SqlException e)
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
                }
            }
        }

        [HttpPost("V1/UpdBookRemark")]
        public async Task<ActionResult> UpdBookRemark(UpdBookRemarkRequest request)
        {
            string delSQL = @"
                UPDATE Users
                SET BookRemark = (
                    SELECT STRING_AGG(value, ',') 
                    FROM STRING_SPLIT(BookRemark, ',') s
                    WHERE value != @ProgramID
                )
                WHERE UserID = @EmpID AND BookRemark IS NOT NULL
            ";

            string addSQL = @"
                DECLARE @ProgramNew NVARCHAR(20) = CAST(@ProgramID AS NVARCHAR(20))

                UPDATE Users
                SET BookRemark = CASE 
                    WHEN BookRemark IS NULL THEN @ProgramNew
                    WHEN BookRemark = '' THEN @ProgramNew
                    WHEN EXISTS (
                        SELECT 1 
                        FROM STRING_SPLIT(BookRemark, ',') 
                        WHERE value = @ProgramNew
                    ) THEN BookRemark
                    ELSE BookRemark + ',' + @ProgramNew
                    END
                WHERE UserID = @EmpID
            ";

            using (var connection = _POVWebDbContextDapper.Connection)
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();
                try
                {
                    var result = await connection.ExecuteAsync(
                           sql: request.Flag? addSQL:delSQL,
                           request,
                           transaction: transaction,
                           commandTimeout: 180);
                    transaction.Commit();
                    return Ok(FormatResultModel<dynamic>.Success(new { result }));
                }
                catch (SqlException e)
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
                }
            }
        }

    }
}
