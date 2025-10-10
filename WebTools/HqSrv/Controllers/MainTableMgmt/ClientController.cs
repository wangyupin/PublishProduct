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
using Microsoft.Extensions.Configuration;
using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.Data.SqlClient;
using System.Data;
using HqSrv.Repository.MainTableMgmt;
using Microsoft.Extensions.Hosting;
using POVWebDomain.Models.API.StoreSrv.Common;

namespace HqSrv.Controllers.MainTableMgmt
{
    public class ClientController : ApiControllerBase
    {
        private readonly ILogger<HelloController> _logger;
        private readonly POVWebDbContext _POVWebDb;
        private readonly POVWebDbContextDapper _POVWebDbContextDapper;
        private readonly CityAdminSrvClient _CityAdminSrvClient;
        private readonly IConfiguration _Config;
        private readonly ClientRepository _ClientRepository;


        public ClientController(
            ILogger<HelloController> logger
            , POVWebDbContext pOVWebDbContext
            , POVWebDbContextDapper pOVWebDbContextDapper
            , CityAdminSrvClient cityAdminSrvClient
            , IConfiguration config,
            ClientRepository clientRepository
            )
        {
            _logger = logger;
            _POVWebDb = pOVWebDbContext;
            _POVWebDbContextDapper = pOVWebDbContextDapper;
            _CityAdminSrvClient = cityAdminSrvClient;
            _Config = config;
            _ClientRepository = clientRepository;
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
            string selectStr =
                @$"SELECT [ClientID],[ClientName],[InvoiceName],[ClientShort],[UniteID],[StoreID],[Principal],[ContactPerson],[TelPhon01],[TelPhon02],[TelPhon03],[Fax],[InvoiceAddr01],[AccountAddr01],[AccountAddr04], [SendAddress01],[SendAddress04],[TaxRate],[Discount],[CatenationID],C.[Remark01], CONVERT(VARCHAR, CAST(C.[ChangeDate] as date), 23) AS ChangeDate,[InvoicePost],[AccountPost],[SendPost],CONVERT(VARCHAR, CAST(C.[OpenDate] as date), 23) AS OpenDate,C.[ChangePerson],[CreditLimit],[Class],[PersonNum],[AreaNum],C.[Display],C.[Email],[Operation], ISNULL([Operation],'') Operation, ISNULL(E.EmpName,'') OperationName
                 FROM Client C
                 LEFT JOIN Employee E ON C.Operation = E.EmpID
                ";

            string whereCheck = "";
            string whereStr = "";
            string orderByStr = string.IsNullOrEmpty(request.OrderBy)
                ? " Order By ClientID "
                : " Order By " + request.OrderBy + " ";

            foreach (var p in request.GetType().GetProperties().Where(p => !p.GetGetMethod().GetParameters().Any()))
            {
                var tmpObjName = p.Name;
                var tmpObjValue = p.GetValue(request, null) == null ? "" : p.GetValue(request, null).ToString();
                string[] separators = { "_" };
                string[] tmpFieldName = tmpObjName.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                if (string.IsNullOrEmpty(whereCheck) && !"OrderBy".Contains(tmpObjName) && !string.IsNullOrEmpty(tmpObjValue))
                {
                    whereCheck += " WHERE 1 = 1 ";
                }

                if (!"OrderBy".Contains(tmpObjName) && !string.IsNullOrEmpty(tmpObjValue))
                {
                    switch (tmpFieldName[1])
                    {
                        case "Like":
                            whereStr += " And C." + tmpFieldName[0] + " Like @" + tmpObjName + " ";
                            p.SetValue(request, "%" + tmpObjValue + "%");
                            break;

                        case "From":
                            whereStr += " And C." + tmpFieldName[0] + " >= @" + tmpObjName + " ";
                            break;

                        case "To":
                            whereStr += " And C." + tmpFieldName[0] + " <= @" + tmpObjName + " ";
                            break;

                        default:
                            break;
                    }

                }
            }
            selectStr = selectStr + whereCheck + whereStr + orderByStr;

            // Biz
            var result = await _POVWebDbContextDapper.Connection.QueryAsync(selectStr, request);


            // Result
            return Ok(FormatResultModel<dynamic>.Success(result));
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
        public ActionResult GetClientHelp(GetClientHelpRequest request)
        {
            // Check parameters

            // Initial variables
            string selectStr =
                @$"SELECT ClientID, ClientShort, ClientName, StoreID 
                     FROM Client ";
            string whereCheck = " WHERE Display = '0' ";
            string whereStr = "";
            string orderByStr = string.IsNullOrEmpty(request.OrderBy)
                ? " Order By ClientID "
                : " Order By " + request.OrderBy + " ";

            if (!string.IsNullOrEmpty(request.ClientID_ClientShort_ClientName_Like))
            {
                whereStr += " And (ClientID + ClientShort + ClientName) Like @ClientID_ClientShort_ClientName_Like ";
                request.ClientID_ClientShort_ClientName_Like = "%" + request.ClientID_ClientShort_ClientName_Like + "%";
            }
            if (!string.IsNullOrEmpty(request.ClientID_NotDisplay))
            {
                whereStr += " And ClientID != @ClientID_NotDisplay ";
            }
            selectStr = selectStr + whereCheck + whereStr + orderByStr;

            // Biz
            List<GetClientHelpResponse> tmpClient = _POVWebDbContextDapper.Connection.Query<GetClientHelpResponse>(selectStr,
                request)
                .ToList();

            // Result
            return tmpClient.Count > 0
                ? Ok(FormatResultModel<dynamic>.Success(
                    tmpClient
                    ))
                : Ok(FormatResultModel<dynamic>.Failure(
                    new
                    {
                        MSG = $"查無符合條件資料"
                    }));
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
        public ActionResult GetClientCheck(GetClientCheckRequest request)
        {
            // Check parameters
            if (string.IsNullOrEmpty(request.ClientID))
                return BadRequest(FormatResultModel<dynamic>.Failure(
                    new
                    {
                        MSG = "請輸入編號"
                    }));

            // Initial variables
            string selectStr =
                @$"SELECT ClientID, ClientShort  
                     FROM Client ";
            string whereCheck = " WHERE Display = '0' ";
            string whereStr = " And ClientID = @ClientID ";
            string orderByStr = "";

            selectStr = selectStr + whereCheck + whereStr + orderByStr;

            // Biz
            List<GetClientCheckResponse> tmpClient = _POVWebDbContextDapper.Connection.Query<GetClientCheckResponse>(selectStr,
                request)
                .ToList();

            // Result
            return tmpClient.Count > 0
                ? Ok(FormatResultModel<dynamic>.Success(
                    tmpClient
                    ))
                : Ok(FormatResultModel<dynamic>.Failure(
                    new
                    {
                        MSG = $"查無此編號 {request.ClientID}"
                    }));
        }

        [HttpPost("V1/GetCatenationIDHelp")]
        public async Task<ActionResult> GetCatenationIDHelp(GetCatenationIDHelpRequest request)
        {
            string selectStr =
                @$"SELECT DISTINCT C1.CatenationID, ISNULL(C2.ClientShort,'')ClientShort,  ISNULL(C2.Principal,'')Principal
                    FROM Client C1
                    LEFT JOIN Client C2 ON C1.CatenationID=C2.ClientID
                    WHERE ISNULL(C1.CatenationID,'')!=''
                    ORDER BY C1.{request.OrderBy}";
            try
            {
                var result = await _POVWebDbContextDapper.Connection.QueryAsync(selectStr, commandTimeout: 180);

                return Ok(FormatResultModel<dynamic>.Success(result));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }

        [HttpPost("V1/GetClientDetailByID")]
        public async Task<ActionResult> GetClientDetailByID(GetClientDetailByIDRequest request)
        {
            // Initial variables
            string selectStr =
                @$"SELECT [ClientID],[ClientName],[InvoiceName],[ClientShort],[UniteID],[StoreID],[Principal],[ContactPerson],[TelPhon01],[TelPhon02],[TelPhon03],[Fax],[InvoiceAddr01],[AccountAddr01],[SendAddress01],[SendAddress04],[TaxRate],[Discount],[CatenationID],C.[Remark01], CONVERT(VARCHAR, CAST(C.[ChangeDate] as date), 23) AS ChangeDate,[InvoicePost],[AccountPost],[SendPost],CONVERT(VARCHAR, CAST(C.[OpenDate] as date), 23) AS OpenDate,C.[ChangePerson],[CreditLimit],[Class],[PersonNum],[AreaNum],C.[Display],C.[Email],[AccountAddr04], ISNULL([Operation],'') Operation, ISNULL(E.EmpName,'') OperationName
                 FROM Client C
                 LEFT JOIN Employee E ON C.Operation = E.EmpID
                ";

            string whereStr = "WHERE ClientID = @ClientID ";
            string orderByStr = "";

            selectStr = selectStr + whereStr + orderByStr;

            // Biz
            var result = await _POVWebDbContextDapper.Connection.QueryAsync(selectStr, request);
            // Result
            return Ok(FormatResultModel<dynamic>.Success(result));
        }

        /// <summary>
        /// 新增客戶基本資料
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("V1/AddClient")]
        public async Task<ActionResult> AddClient(AddClientRequest request)
        {
            // Initial variables
            var paramList = new DynamicParameters();
            foreach (var p in request.GetType().GetProperties().Where(p => !p.GetGetMethod().GetParameters().Any()))
            {
                var tmpObjName = p.Name;
                var tmpObjValue = p.GetValue(request, null) == null ? "" : p.GetValue(request, null).ToString();
                if (!"OrderBy".Contains(tmpObjName) && !string.IsNullOrEmpty(tmpObjValue))
                {
                    paramList.Add("@" + tmpObjName, p.GetValue(request, null));
                }
            }

            string addSQL = @"[POVWeb].[uspClientAdd]";

            // Biz
            int effectRows = 0;

            using (var connection = _POVWebDbContextDapper.Connection)
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();
                try
                {
                    //add
                    effectRows = await connection.ExecuteAsync(
                            sql: addSQL,
                            paramList,
                            transaction: transaction,
                            commandType: CommandType.StoredProcedure,
                            commandTimeout: 180);
                    transaction.Commit();
                }
                catch (SqlException e)
                {
                    //使用 using var transaction = connection.BeginTransaction(); 會自動Rollback
                    //transaction.Rollback(); 
                    return Ok(FormatResultModel<dynamic>.Failure(
                        new
                        {
                            MSG = e.Number == 6636001 ? e.Message : "6636001:新增客戶基本資料失敗，請洽系統人員"
                        }));
                }
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(
                new
                {
                    effectRows = Math.Abs(effectRows)
                }));

        }

        /// <summary>
        /// 修改客戶基本資料
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("V1/UpdClient")]
        public async Task<ActionResult> UpdClient(UpdClientRequest request)
        {
            //Initial variables
            string updateStr =
                @$"UPDATE Client ";
            string setString = "SET ";
            string whereString = "Where ";
            int i = 0;

            foreach (var p in request.GetType().GetProperties().Where(p => !p.GetGetMethod().GetParameters().Any()))
            {
                var tmpObjName = p.Name;
                var tmpObjValue = p.GetValue(request, null) == null ? "" : p.GetValue(request, null).ToString();
                var tmpObjType = p.PropertyType.Name;

                if (tmpObjName == "ClientID")
                {
                    whereString += "ClientID= '" + tmpObjValue + "'";
                }

                if (i == 0)
                {
                    setString += tmpObjName + "=";
                    if (tmpObjType == "String")
                        setString += "'" + tmpObjValue + "'";
                    else
                        setString += tmpObjValue;
                }
                else
                {
                    setString += "," + tmpObjName + "=";
                    if (tmpObjType == "String")
                        setString += "'" + tmpObjValue + "' ";
                    else
                        setString += tmpObjValue;
                }
                i++;
            }

            updateStr = updateStr + setString + whereString;

            // Biz
            int effectRows = 0;

            using (var connection = _POVWebDbContextDapper.Connection)
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();
                try
                {
                    effectRows = await connection.ExecuteAsync(
                        sql: updateStr,
                        request,
                        transaction: transaction,
                        commandTimeout: 180);
                    transaction.Commit();
                }
                catch (SqlException e)
                {
                    return Ok(FormatResultModel<dynamic>.Failure(
                        new
                        {
                            MSG = e.Number == 6636001 ? e.Message : "6636001:修改客戶基本資料失敗，請洽系統人員"
                        }));
                }
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(
                new
                {
                    effectRows = Math.Abs(effectRows),
                })); ;
        }

        /// <summary>
        /// 刪除客戶基本資料
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("V1/DelClient")]
        public async Task<ActionResult> DelClient(DelClientRequest request)
        {
            //Initial variables
            string deleteStr =
                @$"DELETE FROM Client ";
            string whereString = "Where ClientID = @ClientID";




            deleteStr = deleteStr + whereString;

            // Biz
            int effectRows = 0;

            using (var connection = _POVWebDbContextDapper.Connection)
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();
                try
                {
                    effectRows = await connection.ExecuteAsync(
                        sql: deleteStr,
                        request,
                        transaction: transaction,
                        commandTimeout: 180);
                    transaction.Commit();
                }
                catch (SqlException e)
                {
                    return Ok(FormatResultModel<dynamic>.Failure(
                        new
                        {
                            MSG = e.Number == 6636001 ? e.Message : "6636001:刪除客戶基本資料失敗，請洽系統人員"
                        }));
                }
            }

            // Result
            return Ok(FormatResultModel<dynamic>.Success(
                new
                {
                    effectRows = Math.Abs(effectRows),
                })); ;
        }

        [HttpPost("V1/GetClientOptions")]
        public async Task<ActionResult> GetClientOptions(GetClientStoreRequest request)
        {

            // Biz
            try
            {
                var data = await _ClientRepository.GetClientOptions(request);

                // Result
                return Ok(FormatResultModel<dynamic>.Success(data));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Success(new { ex.Message }));
            }

        }
        [HttpPost("V1/GetClientMeStoreOptions")]
        public async Task<ActionResult> GetClientMeStoreOptions(GetClientMeStoreRequest request)
        {

            // Biz
            try
            {
                var data = await _ClientRepository.GetClientMeStoreOptions(request);

                // Result
                return Ok(FormatResultModel<dynamic>.Success(data));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Success(new { ex.Message }));
            }

        }
        [HttpGet("V1/GetClientOptionsAll")]
        public async Task<ActionResult> GetClientOptionsAll()
        {

            // Biz
            try
            {
                var data = await _ClientRepository.GetClientOptionsAll();

                // Result
                return Ok(FormatResultModel<dynamic>.Success(data));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Success(new { ex.Message }));
            }

        }

        [HttpGet("V1/GetDistributorOptionsAll")]
        public async Task<ActionResult> GetDistributorOptionsAll()
        {

            // Biz
            try
            {
                var data = await _ClientRepository.GetDistributorOptionsAll();

                // Result
                return Ok(FormatResultModel<dynamic>.Success(data));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Success(new { ex.Message }));
            }

        }

        [HttpGet("V1/GetCatenationIDOptionsAll")]
        public async Task<ActionResult> GetCatenationIDOptionsAll()
        {

            // Biz
            try
            {
                var data = await _ClientRepository.GetCatenationIDOptionsAll();

                // Result
                return Ok(FormatResultModel<dynamic>.Success(data));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Success(new { ex.Message }));
            }

        }

        [HttpGet("V1/GetEStoreOptionsAll")]
        public async Task<ActionResult> GetEStoreOptionsAll()
        {

            // Biz
            try
            {
                var data = await _ClientRepository.GetEStoreOptionsAll();

                // Result
                return Ok(FormatResultModel<dynamic>.Success(data));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Success(new { ex.Message }));
            }

        }

        [HttpGet("V1/GetDepOptionsAll")]
        public async Task<ActionResult> GetDepOptionsAll()
        {

            // Biz
            try
            {
                var data = await _ClientRepository.GetDepOptionsAll();

                // Result
                return Ok(FormatResultModel<dynamic>.Success(data));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Success(new { ex.Message }));
            }

        }

        [HttpPost("V1/GetClientOptionsExcludeMainWH")]
        public async Task<ActionResult> GetClientOptionsExcludeMainWH(GetClientOptionsExcludeMainWHRequest request)
        {

            // Biz
            try
            {
                var data = await _ClientRepository.GetClientOptionsExcludeMainWH(request);

                // Result
                return Ok(FormatResultModel<dynamic>.Success(data));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Success(new { ex.Message }));
            }

        }
        [HttpPost("V1/GetClientOptionsOnlyMainWH")]
        public async Task<ActionResult> GetClientOptionsOnlyMainWH(GetClientOptionsOnlyMainWHRequest request)
        {

            // Biz
            try
            {
                var data = await _ClientRepository.GetClientOptionsOnlyMainWH(request);

                // Result
                return Ok(FormatResultModel<dynamic>.Success(data));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Success(new { ex.Message }));
            }

        }

        [HttpPost("V1/GetBillCustOptions")]
        public async Task<ActionResult> GetBillCustOptions(GetBillCustOptionsRequest request)
        {

            // Biz
            try
            {
                var data = await _ClientRepository.GetBillCustOptions(request);

                // Result
                return Ok(FormatResultModel<dynamic>.Success(data));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Success(new { ex.Message }));
            }

        }
        [HttpGet("V1/GetClientOptionsAllWithCompany")]
        public async Task<ActionResult> GetClientOptionsAllWithCompany()
        {

            // Biz
            try
            {
                var data = await _ClientRepository.GetClientOptionsAllWithCompany();

                // Result
                return Ok(FormatResultModel<dynamic>.Success(data));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Success(new { ex.Message }));
            }

        }

        [HttpGet("V1/GetCompanyOptions")]
        public async Task<ActionResult> GetCompanyOptions()
        {

            // Biz
            try
            {
                var data = await _ClientRepository.GetCompanyOptions();

                // Result
                return Ok(FormatResultModel<dynamic>.Success(data));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Success(new { ex.Message }));
            }

        }

        [HttpGet("V1/GetClientHelp")]
        public async Task<ActionResult> GetClientHelp()
        {

            string selectStr =
                @$"SELECT clientID Value, clientName Label FROM Client ";

            try
            {
                var clientList = await _POVWebDbContextDapper.Connection.QueryAsync(selectStr, commandTimeout: 180);

                return Ok(FormatResultModel<dynamic>.Success(new { clientList }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }

        [HttpPost("V1/GetData")]
        public async Task<ActionResult> GetData(GetDataRequest request)
        {

            string userOrderByStr = string.Format("ORDER BY {0} {1}", request.SortColumn, request.Sort);
            string offSetStr = request.Mode == String.Empty ?
                @$"OFFSET (@Page-1)*@PerPage ROWS 
                   FETCH NEXT @PerPage ROWS ONLY" :
                "";

            string advanceRequestStr = "";
            string searchTermRequestStr = "";
            if (request.AdvanceRequest != null)
            {
                advanceRequestStr = request.AdvanceRequest.GenerateSqlString();
            }
            if (request.SearchTerm != null)
            {
                searchTermRequestStr = request.SearchTerm.GenerateSqlString();
            }

            string selectStr = @$"
                SELECT COUNT(*) Length
                FROM Client
                WHERE
                    1=1
                    {searchTermRequestStr}
                    {advanceRequestStr}
                
                SELECT 
                ClientID, ClientName, InvoiceName, ClientShort, UniteID, StoreID, Principal, ContactPerson, Principal, TelPhon01, TelPhon02, Fax, Operation, InvoiceAddr01, AccountAddr01, SendAddress01, TaxRate, Discount, CatenationID, Remark01, ChangeDate, FirstAccount, BeforeAccount, OpenDate, CASE Display WHEN '1' THEN 0 ELSE 1 END Display, ChangePerson, CreditLimit, Email, CompanyName, BankName, InvoicePost, AccountPost, SendPost, AccountAddr04, SendAddress04, Class, SendAddress02, Unit, ClassNew
                FROM 
                Client
                WHERE
                1=1
                {searchTermRequestStr}
                {advanceRequestStr}
                {userOrderByStr}
                {offSetStr}
            ";
            try
            {
                var result = await _POVWebDbContextDapper.Connection.QueryMultipleAsync(
                selectStr,
                request,
                commandTimeout: 180);

                var clientTotal = result.IsConsumed ? 0 : result.Read<ClientDataLength>().FirstOrDefault().Length;
                var clientList = result.IsConsumed ? null : result.Read<ClientData>();

                return Ok(FormatResultModel<dynamic>.Success(new { clientTotal, clientList }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }

        [HttpPost("V1/GetSingleData")]
        public async Task<ActionResult> GetSingleData(GetSingleDataRequest request)
        {
            string selectStr = @$"
                SELECT ClientID, ClientName, InvoiceName, ClientShort, UniteID, StoreID, Principal, ContactPerson, TelPhon01, TelPhon02, Fax, Operation, InvoiceAddr01, AccountAddr01, SendAddress01, TaxRate, Discount, CatenationID, Remark01, ChangeDate, FirstAccount, BeforeAccount, CONVERT(VARCHAR, CAST(OpenDate as date), 23) AS OpenDate, ChangePerson, CreditLimit, Email, CompanyName, BankName, CASE Display WHEN '1' THEN 0 ELSE 1 END Display, InvoicePost, AccountPost, SendPost, AccountAddr04, SendAddress04, Class, Remark02, SendAddress02, OldDate, Unit, ClassNew, Company                

                FROM Client
                WHERE ClientID=@ClientID
            ";
            try
            {
                var result = await _POVWebDbContextDapper.Connection.QueryMultipleAsync(
                selectStr,
                request,
                commandTimeout: 180);

                var client = result.IsConsumed ? null : result.Read<ClientData>().FirstOrDefault();

                if (client == null) throw new Exception("找不到客戶名稱!");

                return Ok(FormatResultModel<dynamic>.Success(new { client }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }

        [HttpPost("V1/AddClientData")]
        public async Task<ActionResult> AddUsersData(AddClientDataRequest request)
        {
            string displayChange = "1";
            if (request.Display == true)
            {
                displayChange = "0";
            }

            request.ChangeDate = request.ChangeDate?.Replace("-", "");
            request.OpenDate = request.OpenDate?.Replace("-", "");

            string actionStr = null;
            actionStr =
           @$"	Declare @O_MSG VARCHAR(MAX)=N''
                    
                    BEGIN TRY
                        IF LEN(@StoreID) > 0 AND EXISTS (
                            SELECT 1 
                            FROM [dbo].[Client]
                            WHERE StoreID = @StoreID
                        )
                        BEGIN
                            SET @O_MSG = '分店代號重複>>' + @StoreID;
                            THROW 6636001, @O_MSG, 1;
                        END
	                INSERT INTO [dbo].[Client]
                           (ClientID, ClientName, InvoiceName, ClientShort, UniteID, StoreID, Principal, ContactPerson, TelPhon01, TelPhon02, Fax, Operation, InvoiceAddr01, AccountAddr01, SendAddress01, TaxRate, Discount, CatenationID, Remark01, ChangeDate, FirstAccount, BeforeAccount, OpenDate, ChangePerson, CreditLimit, Email, CompanyName, BankName, Display, InvoicePost, AccountPost, SendPost, AccountAddr04, SendAddress04, Class, Remark02, SendAddress02, OldDate, Unit, ClassNew, Company)
                     VALUES
			                (@ClientID, @ClientName, @InvoiceName, @ClientShort, @UniteID, @StoreID, @Principal, @ContactPerson, @TelPhon01, @TelPhon02, @Fax, @Operation, @InvoiceAddr01, @AccountAddr01, @SendAddress01, @TaxRate, @Discount, @CatenationID, @Remark01, @ChangeDate, @FirstAccount, @BeforeAccount, @OpenDate, @ChangePerson, @CreditLimit, @Email, @CompanyName, @BankName, {displayChange}, @InvoicePost, @AccountPost, @SendPost, @AccountAddr04, @SendAddress04, @Class, @Remark02, @SendAddress02, @OldDate, @Unit, @ClassNew, @Company )

	                END TRY
	                BEGIN CATCH
		                SET @O_MSG = '新增客戶失敗。'
					                + ERROR_MESSAGE() 
		                ;THROW 6636001, @O_MSG, 1;
	                END CATCH
                ";
            try
            {
                var result = await _POVWebDbContextDapper.Connection.ExecuteAsync(actionStr, request);

                return Ok(FormatResultModel<dynamic>.Success(new { MSG = result }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }

        [HttpPost("V1/DelClientData")]
        public async Task<ActionResult> DelUsersData(DelClientDataRequest request)
        {

            string actionStr =
                @$" Declare @O_MSG VARCHAR(MAX)=N''
                    BEGIN TRY
                    IF EXISTS (SELECT 1 FROM Orders WHERE CustID = @ClientID)
                        BEGIN
                            SET @O_MSG = '無法刪除，進貨中存在與該客戶相關聯的記錄。'
                            ;THROW 6636002, @O_MSG, 1;
                    END
		                DELETE FROM Client WHERE ClientID = @ClientID
	                END TRY
	                BEGIN CATCH
		                SET @O_MSG = '刪除資料失敗。' + ERROR_MESSAGE() 
		                ;THROW 6636001, @O_MSG, 1;
	                END CATCH	
                ";
            try
            {
                var result = await _POVWebDbContextDapper.Connection.ExecuteAsync(actionStr, request.DelList);

                return Ok(FormatResultModel<dynamic>.Success(new { result }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }

        [HttpPost("V1/UpdClientData")]
        public async Task<ActionResult> UpdPeerData(UpdClientDataRequest request)
        {
            request.ChangeDate = request.ChangeDate?.Replace("-", "");
            request.OpenDate = request.OpenDate?.Replace("-", "");
            string opendate = request.OpenDate;

            string displayChange = "1";
            if (request.Display == true)
            {
                displayChange = "0";
            }

            string actionStr = null;
            actionStr =
           @$"Declare @O_MSG VARCHAR(MAX)=N''
                    BEGIN TRY
                    IF LEN(@StoreID) > 0 AND EXISTS (
                        SELECT 1 
                        FROM [dbo].[Client]
                        WHERE StoreID = @StoreID
                        AND ClientID <> @OriginalClientID
                    )
                    BEGIN
                        SET @O_MSG = '分店代號重複>>' + @StoreID;
                        THROW 6636001, @O_MSG, 1;
                    END

	                UPDATE [dbo].[Client] 
                    SET ClientName=@ClientName, InvoiceName=@InvoiceName, ClientShort=@ClientShort, UniteID=@UniteID, StoreID=@StoreID, Principal=@Principal, ContactPerson=@ContactPerson, TelPhon01=@TelPhon01, TelPhon02=@TelPhon02, Fax=@Fax, Operation=@Operation, InvoiceAddr01=@InvoiceAddr01, AccountAddr01=@AccountAddr01, SendAddress01=@SendAddress01, TaxRate=@TaxRate, Discount=@Discount, CatenationID=@CatenationID, Remark01=@Remark01, ChangeDate=@ChangeDate, FirstAccount=@FirstAccount, BeforeAccount=@BeforeAccount, OpenDate={opendate}, ChangePerson=@ChangePerson, CreditLimit=@CreditLimit, Email=@Email, CompanyName=@CompanyName, BankName=@BankName, Display={displayChange}, InvoicePost=@InvoicePost, AccountPost=@AccountPost, SendPost=@SendPost, AccountAddr04=@AccountAddr04, SendAddress04=@SendAddress04, Class=@Class, Remark02=@Remark02, SendAddress02=@SendAddress02, OldDate=@OldDate, Unit=@Unit, ClassNew=@ClassNew, Company=@Company

                    WHERE ClientID=@OriginalClientID

	                END TRY
	                BEGIN CATCH
		                SET @O_MSG = '更改品牌失敗。 編號：' + CONVERT(NVARCHAR(50), @OriginalClientID + '   ')
					                + ERROR_MESSAGE() 
		                ;THROW 6636001, @O_MSG, 1;
	                END CATCH	
                ";
            try
            {
                var result = await _POVWebDbContextDapper.Connection.ExecuteAsync(actionStr, request);

                return Ok(FormatResultModel<dynamic>.Success(new { result }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }

        [HttpPost("V1/ImportClient")]
        public async Task<ActionResult> ImportClient([FromForm] ImportRequest request)
        {
            try
            {
                var result = await _ClientRepository.ImportClient(request);
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

        [HttpPost("V1/GetClientIsConsignment")]
        public async Task<ActionResult> GetClientIsConsignment(GetClientCheckRequest request)
        {
            // Check parameters
            if (string.IsNullOrEmpty(request.ClientID))
                return BadRequest(FormatResultModel<dynamic>.Failure(
                    new
                    {
                        MSG = "請輸入編號"
                    }));

            // Initial variables
            var tmpClient = await _ClientRepository.GetClientIsConsignment(request);

            // Result
            return tmpClient.Count > 0
                ? Ok(FormatResultModel<dynamic>.Success(
                    tmpClient
                    ))
                : Ok(FormatResultModel<dynamic>.Failure(
                    new
                    {
                        MSG = $"查無此編號 {request.ClientID}"
                    }));
        }

        [HttpPost("V1/GetClientHelpOffset")]
        public async Task<ActionResult> GetClientHelpOffset(GetDataRequest request)
        {
            string advanceRequestStr = "";
            string searchTermRequestStr = "";
            string userOrderByStr = string.Format("ORDER BY {0} {1}", request.SortColumn, request.Sort);
            string offSetStr = request.Mode == String.Empty ?
                @$"OFFSET (@Page-1)*@PerPage ROWS 
                   FETCH NEXT @PerPage ROWS ONLY" : "";
            if (request.AdvanceRequest != null)
            {
                advanceRequestStr = request.AdvanceRequest.GenerateSqlString();
            }
            if (request.SearchTerm != null)
            {
                searchTermRequestStr = request.SearchTerm.GenerateSqlString();
            }
            string sql = @$"
                select ClientID, ClientShort
                from Client
                where 1=1
                {searchTermRequestStr}
                {advanceRequestStr}
                {userOrderByStr}
                {offSetStr}
                ";
            string sqlNum = @$"
                select Count(*)
                from Client
                where 1=1
                {searchTermRequestStr}
                {advanceRequestStr}
                ";

            using (var connection = _POVWebDbContextDapper.Connection)
            {
                connection.Open();
                try
                {
                    var resultNum = await connection.ExecuteScalarAsync<int>(sqlNum, request, commandTimeout: 180);
                    var result = await connection.QueryAsync(sql, request, commandTimeout: 180);
                    //int num = resultNum % request.Offset == 0 ? resultNum / request.Offset : (resultNum / request.Offset) + 1;
                    return Ok(FormatResultModel<dynamic>.Success(new { result, resultNum }));
                }
                catch (Exception ex)
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { Msg = ex.Message }));
                }
            }
        }

    }
}