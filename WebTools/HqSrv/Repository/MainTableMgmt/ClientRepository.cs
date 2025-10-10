using Dapper;
using POVWebDomain.Models.API.StoreSrv.MainTableMgmt.Client;
using POVWebDomain.Models.DB.POVWeb;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using POVWebDomain.Models.API.StoreSrv.Common;
using System.IO;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System.Security.Claims;

namespace HqSrv.Repository.MainTableMgmt
{
    public class ClientRepository
    {
        private readonly POVWebDbContextDapper _context;
        public ClientRepository(POVWebDbContextDapper context)
        {
            _context = context;
        }

        public async Task<IEnumerable<object>> GetClientOptions(GetClientStoreRequest request)
        {
            var paramList = GenerateParams(request);

            string query = @"Declare @t varchar(max)

                             select @t = ClientPermission
                             from Users
                             where UserID = @UserID

                             select C.ClientID as Value, C.ClientShort as Label
                             from Client C
                            ";
            var result = await _context.Connection.QueryAsync(query, paramList);

            return result;
        }
        public async Task<IEnumerable<object>> GetClientMeStoreOptions(GetClientMeStoreRequest request)
        {
            var paramList = GenerateParams(request);

            string query = @"
                             select C.ClientID as Value, C.ClientShort as Label
                             from Client C
                             LEFT JOIN (SELECT * FROM [dbo].[udfMeStore] (@SellBranch)) C1 ON C.ClientID = C1.ClientID
                             WHERE C1.ClientID IS NOT NULL
                             Order By C.ClientID
                            ";
            var result = await _context.Connection.QueryAsync(query, paramList);

            return result;
        }

        public async Task<IEnumerable<object>> GetClientOptionsAll()
        {

            string query = @"Select ClientID as Value, ClientShort as Label
                             from Client
                             Where Display = '0'
                             Order By CASE Remark02 WHEN '1' THEN 1 WHEN '2' THEN 2 WHEN '3' THEN 3 WHEN '4' THEN 4 WHEN '5' THEN 5 ELSE 6 END, ClientID
            ";
            var result = await _context.Connection.QueryAsync(query);

            return result;
        }

        public async Task<IEnumerable<object>> GetDistributorOptionsAll()
        {

            string query = @"Select ClientID as Value, ClientShort as Label
                             from Client
                             WHERE Remark02 = '5' AND Display = '0'
                             Order By ClientID
                            ";
            var result = await _context.Connection.QueryAsync(query);

            return result;
        }

        public async Task<IEnumerable<object>> GetCatenationIDOptionsAll()
        {

            string query = @"SELECT DISTINCT C1.CatenationID Value, C2.ClientShort Label
                                FROM Client C1
                                LEFT JOIN Client C2 ON C1.CatenationID = C2.ClientID
                                WHERE ISNULL(C1.CatenationID ,'') != ''AND C1.Display = '0' AND C1.Remark02 = '5'
                                Order By C1.CatenationID
                            ";
            var result = await _context.Connection.QueryAsync(query);

            return result;
        }

        public async Task<IEnumerable<object>> GetEStoreOptionsAll()
        {

            string query = @"Select ClientID as Value, ClientShort as Label
                             from Client
                             WHERE Remark02 = '4' AND Display = '0'
                             Order By ClientID
                            ";
            var result = await _context.Connection.QueryAsync(query);

            return result;
        }

        public async Task<IEnumerable<object>> GetDepOptionsAll()
        {

            string query = @"Select ClientID as Value, ClientShort as Label
                             from Client
                             WHERE Remark02 = '2' AND Display = '0'
                             Order By ClientID
                            ";
            var result = await _context.Connection.QueryAsync(query);

            return result;
        }

        public async Task<IEnumerable<object>> GetClientOptionsExcludeMainWH(GetClientOptionsExcludeMainWHRequest request)
        {

            string query = @"Select C.ClientID as Value, C.ClientShort as Label
							 from Client C
							 where C.Remark02 IN ('2','3') OR ClientID='0ZZZZ'
                             Order By CASE C.Remark02 WHEN '2' THEN 1 WHEN '3' THEN 2 ELSE 3 END, C.ClientID
                            ";
            var result = await _context.Connection.QueryAsync(query, request);

            return result;
        }

        public async Task<IEnumerable<object>> GetClientOptionsOnlyMainWH(GetClientOptionsOnlyMainWHRequest request)
        {

            string query = @"Select C.ClientID as Value, C.ClientShort as Label
							 from Client C
							 where C.Remark02 = '1'
                             Order By C.ClientID
                            ";
            var result = await _context.Connection.QueryAsync(query, request);

            return result;
        }
        public async Task<IEnumerable<object>> GetClientOptionsAllWithCompany()
        {

            string query = @"Select ClientID as Value, ClientShort as Label, t1.CompanyID as Value1, t1.CompanyName as Label1
                            from Client t0
                            left join Company t1 on t1.CompanyID = t0.Company
                            Where Display = '0'
                            Order By ClientID
            ";
            var result = await _context.Connection.QueryAsync(query);

            return result;
        }

        public async Task<IEnumerable<object>> GetBillCustOptions(GetBillCustOptionsRequest request)
        {

            string query = @"--Declare @CustID varchar(20) = 'ST01-A'
                            Select C.ClientID as Value, C.ClientShort as Label
                            from Client C
                            where C.ClientID = @CustID
                            union
                            Select C1.ClientID as Value, C1.ClientShort as Label
                            from Client C
                            left join Client C1 on C1.ClientID = C.CatenationID
                            where C.ClientID = @CustID
                            ";
            var result = await _context.Connection.QueryAsync(query, request);

            return result;
        }

        public async Task<IEnumerable<object>> GetCompanyOptions()
        {

            string query = @"select CompanyID as Value, CompanyName as Label
                             from Company
            ";
            var result = await _context.Connection.QueryAsync(query);

            return result;
        }

        public DynamicParameters GenerateParams(object request)
        {
            var paramList = new DynamicParameters();
            foreach (PropertyInfo propertyInfo in request.GetType().GetProperties())
            {
                Type t = propertyInfo.PropertyType;
                string paramName = $"@{propertyInfo.Name}";
                if (!t.IsGenericType)
                {
                    var paramValue = propertyInfo.GetValue(request, null)?.ToString() ?? "";
                    paramList.Add(paramName, paramValue);
                }
                else
                {
                    DataTable RET1 = new();
                    RET1.Columns.Add("Str1");
                    foreach (var i in (IEnumerable<object>)propertyInfo.GetValue(request, null))
                    {
                        RET1.Rows.Add(i.ToString());
                    }
                    paramList.Add(paramName, RET1.AsTableValuedParameter($"[POVWeb].[udtStr]"));
                }
            }
            return paramList;
        }

        public async Task<IEnumerable<object>> GetClientCheck(GetClientCheckRequest request)
        {

            string selectStr =
                @$"SELECT ClientID, ClientShort  
                     FROM Client ";
            string whereCheck = " WHERE Display = '0' ";
            string whereStr = " And ClientID = @ClientID ";
            string orderByStr = "";

            selectStr = selectStr + whereCheck + whereStr + orderByStr;
            var result = (await _context.Connection.QueryAsync<GetClientCheckResponse>(selectStr, request)).ToList();

            return result;
        }

        public async Task<IEnumerable<object>> GetCatenationIDHelp(GetCatenationIDHelpRequest request)
        {

            string selectStr =
                @$"SELECT DISTINCT C1.CatenationID, ISNULL(C2.ClientShort,'')ClientShort,  ISNULL(C2.Principal,'')Principal
                    FROM Client C1
                    LEFT JOIN Client C2 ON C1.CatenationID=C2.ClientID
                    WHERE ISNULL(C1.CatenationID,'')!=''
                    ORDER BY C1.{request.OrderBy}";

            var result = await _context.Connection.QueryAsync(selectStr, request);

            return result;
        }

        public async Task<(object, string)> ImportClient(ImportRequest request)
        {
            List<ClientDB> table = new List<ClientDB>();

            using (var stream = new MemoryStream())
            {
                for (var i = 0; i < request.Files.Count; i++)
                {
                    var file = request.Files[i];
                    file.CopyTo(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];

                        int rowCount = worksheet.Dimension.Rows;
                        int colCount = worksheet.Dimension.Columns;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            table.Add(new ClientDB
                            {
                                ClientID = worksheet.Cells[row, 1].Value?.ToString().Trim() ?? "",
                                StoreID = worksheet.Cells[row, 2].Value?.ToString().Trim() ?? "",
                                CatenationID = worksheet.Cells[row, 3].Value?.ToString().Trim() ?? "",
                                ClientName = worksheet.Cells[row, 4].Value?.ToString().Trim() ?? "",
                                ClientShort = worksheet.Cells[row, 5].Value?.ToString().Trim() ?? "",
                                Class = worksheet.Cells[row, 6].Value?.ToString().Trim() ?? "",
                                PersonNum = (float)Convert.ToDecimal(worksheet.Cells[row, 7].Value),
                                AreaNum = (float)Convert.ToDecimal(worksheet.Cells[row, 8].Value),
                                UniteID = worksheet.Cells[row, 9].Value?.ToString().Trim() ?? "",
                                InvoiceName = worksheet.Cells[row, 10].Value?.ToString().Trim() ?? "",
                                Principal = worksheet.Cells[row, 11].Value?.ToString().Trim() ?? "",
                                ContactPerson = worksheet.Cells[row, 12].Value?.ToString().Trim() ?? "",
                                TelPhon01 = worksheet.Cells[row, 13].Value?.ToString().Trim() ?? "",
                                TelPhon02 = worksheet.Cells[row, 14].Value?.ToString().Trim() ?? "",
                                TelPhon03 = worksheet.Cells[row, 15].Value?.ToString().Trim() ?? "",
                                Fax = worksheet.Cells[row, 16].Value?.ToString().Trim() ?? "",
                                InvoicePost = worksheet.Cells[row, 17].Value?.ToString().Trim() ?? "",
                                InvoiceAddr01 = worksheet.Cells[row, 18].Value?.ToString().Trim() ?? "",
                                AccountPost = worksheet.Cells[row, 19].Value?.ToString().Trim() ?? "",
                                AccountAddr01 = worksheet.Cells[row, 20].Value?.ToString().Trim() ?? "",
                                SendPost = worksheet.Cells[row, 21].Value?.ToString().Trim() ?? "",
                                SendAddress01 = worksheet.Cells[row, 22].Value?.ToString().Trim() ?? "",
                                TaxRate = (float)Convert.ToDecimal(worksheet.Cells[row, 23].Value),
                                Discount = (float)Convert.ToDecimal(worksheet.Cells[row, 24].Value),
                                OpenDate = worksheet.Cells[row, 25].Value?.ToString().Trim() ?? "",
                                CreditLimit = (float)Convert.ToDecimal(worksheet.Cells[row, 26].Value),
                                Email = worksheet.Cells[row, 27].Value?.ToString().Trim() ?? "",
                                Remark01 = worksheet.Cells[row, 28].Value?.ToString().Trim() ?? "",
                                AccountAddr04 = worksheet.Cells[row, 29].Value?.ToString().Trim() ?? "",
                                SendAddress04 = worksheet.Cells[row, 30].Value?.ToString().Trim() ?? "",
                                Operation = worksheet.Cells[row, 31].Value?.ToString().Trim() ?? "",
                                ChangePerson = request.ChangePerson,
                                ChangeDate = DateTime.Now.ToString("yyyyMMdd")
                            });
                        }
                    }
                }

            }

            string importStr = @$"
                DECLARE @O_MSG VARCHAR(MAX) = N''

                --負責人檢查
                 IF @Operation !=''
	            BEGIN
                    SET @Operation = ISNULL((SELECT TOP 1 EmpID FROM Employee WHERE EmpID=@Operation OR EmpName LIKE '%' + @Operation + '%'),'')
                    IF @Operation=''
                    BEGIN
                        SET @O_MSG = '匯入代號' + @ClientID + '失敗 -> ' + '找不到負責人: '+@Operation
                        ;THROW 6636001, @O_MSG, 1
                    END
	            END
              

               IF EXISTS (SELECT 1 FROM Client WHERE ClientID=@ClientID)
               BEGIN
                    UPDATE Client
                    SET ClientName=@ClientName, InvoiceName=@InvoiceName, ClientShort=@ClientShort, UniteID=@UniteID, StoreID=@StoreID, Principal=@Principal, ContactPerson=@ContactPerson,
                           TelPhon01=@TelPhon01, TelPhon02=@TelPhon02, TelPhon03=@TelPhon03, Fax=@Fax, Operation=@Operation, InvoiceAddr01=@InvoiceAddr01,
                           AccountAddr01=@AccountAddr01, SendAddress01=@SendAddress01, 
                           TaxRate=@TaxRate, Discount=@Discount, CatenationID=@CatenationID, Remark01=@Remark01, ChangeDate=@ChangeDate, InvoicePost=@InvoicePost, AccountPost=@AccountPost,
                           SendPost=@SendPost, OpenDate=@OpenDate, ChangePerson=@ChangePerson, CreditLimit=@CreditLimit, Class=@Class, PersonNum=@PersonNum, AreaNum=@AreaNum, Email=@Email
                    WHERE ClientID=@ClientID
               END
               ELSE
               BEGIN
                    INSERT INTO [dbo].[Client]
                           ([ClientID],[ClientName],[InvoiceName],[ClientShort],[UniteID],[StoreID],[Principal],[ContactPerson],
                            [TelPhon01],[TelPhon02],[TelPhon03],[Fax],[Operation],[InvoiceAddr01],[InvoiceAddr02],[InvoiceAddr03],[InvoiceAddr04],
			                [AccountAddr01],[AccountAddr02],[AccountAddr03],[AccountAddr04],[SendAddress01],[SendAddress02],[SendAddress03],[SendAddress04],
                            [TaxRate],[Discount],[CatenationID],[Remark01],[Remark02],[ChangeDate],[FirstAccount],[BeforeAccount],[InvoicePost],[AccountPost],
                            [SendPost],[OpenDate],[ChangePerson],[CreditLimit],[Class],[PersonNum],[AreaNum],[Display],[Email],[OldYear],[OldMonth],[OldDate],
			                [Make],[MakeNum],[MakeAmt],[Unit],[ClassNew],[CompanyName],[BankName],[BranchName],[AccountName],[AccountNumber],[Month],[sortCode1]
		                   )
                     VALUES
			               (@ClientID, @ClientName, @InvoiceName, @ClientShort, @UniteID, @StoreID, @Principal, @ContactPerson,
                            @TelPhon01, @TelPhon02, @TelPhon03, @Fax, @Operation, @InvoiceAddr01, '', '', '',
                            @AccountAddr01, '', '', @AccountAddr04, @SendAddress01, '', '', @SendAddress04,
                            @TaxRate, @Discount, @CatenationID, @Remark01, '', @ChangeDate, 0, 0, @InvoicePost, @AccountPost,
                            @SendPost, @OpenDate, @ChangePerson, @CreditLimit, @Class, @PersonNum, @AreaNum,'0', @Email, '', '', '',
                            '', '', '', '', '', '', '', '', '', '',0, null 
			               )
               END
               

            ";

            string message = string.Empty;
            using (var connection = _context.Connection)
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();
                try
                {
                    var result = await connection.ExecuteAsync(
                        importStr,
                        table,
                        commandTimeout: 180,
                        transaction: transaction);

                    transaction.Commit();
                    return (new { result }, message);
                }
                catch (SqlException e)
                {
                    message = e.Message;
                    return (null, message);
                }
            }
        }

        public async Task<List<object>> GetClientIsConsignment(GetClientCheckRequest request)
        {

            string selectStr =
                @$"SELECT ClientID, ClientShort, Remark02 
                     FROM Client ";
            string whereCheck = " WHERE Display = '0' ";
            string whereStr = "Where ClientID = @ClientID ";
            string orderByStr = "";

            //selectStr = selectStr + whereCheck + whereStr + orderByStr;
            selectStr = selectStr + whereStr + orderByStr;

            // Biz
            var result = _context.Connection.Query(selectStr,
                request)
                .ToList();

            return result;
        }

    }
}