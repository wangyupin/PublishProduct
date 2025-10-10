using CityHubCore.Infrastructure.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POVWebDomain.Models.DB.POVWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using CityHubCore.Infrastructure.ServiceClient;
using System.Data;
using Microsoft.AspNetCore.Hosting;
using POVWebDomain.Models.API.StoreSrv.SettingMgmt.UserAccount;
using POVWebDomain.Models.API.StoreSrv.Common;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Hosting;
using ExcelDataReader;
using System.IO;
using Microsoft.Extensions.Hosting.Internal;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;

namespace HqSrv.Controllers.SettingMgmt
{
    public class UserAccountController : ApiControllerBase
    {
        private readonly ILogger<UserAccountController> _logger;
        private readonly POVWebDbContext _POVWebDb;
        private readonly POVWebDbContextDapper _POVWebDbContextDapper;
        private readonly CityAdminSrvClient _CityAdminSrvClient;
        private readonly IConfiguration _Config;
        private readonly IWebHostEnvironment _hostEnvironment;

        public UserAccountController(
            ILogger<UserAccountController> logger
            , POVWebDbContext pOVWebDbContext
            , POVWebDbContextDapper pOVWebDbContextDapper
            , CityAdminSrvClient cityAdminSrvClient
            , IConfiguration config
            , IWebHostEnvironment hostEnvironment
            )
        {
            _logger = logger;
            _POVWebDb = pOVWebDbContext;
            _POVWebDbContextDapper = pOVWebDbContextDapper;
            _CityAdminSrvClient = cityAdminSrvClient;
            _Config = config;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet("V1/GetAllData")]
        public async Task<ActionResult> GetAllData()
        {
            string selectStr = @$"
                SELECT Group_Name Value, Group_Name Label
                FROM UserGroup
                ORDER BY Group_Name
                
                SELECT EmpID AS Value, EmpName AS Label, PicturePath AS Avatar
                FROM Employee
                LEFT JOIN Users ON EmpID = UserNumber
                WHERE UserID IS NULL

                SELECT ClientID Value, ISNULL(ClientShort,'') Label
                FROM Client
                WHERE ISNULL(StoreID,'') != ''
                ORDER BY ClientID
            ";

            using (var connection = _POVWebDbContextDapper.Connection)
            {
                connection.Open();
                try
                {
                    var result = await connection.QueryMultipleAsync(
                    selectStr,
                    commandTimeout: 180);

                    var groupList = result.IsConsumed ? null : result.Read();
                    var empList = result.IsConsumed ? null : result.Read();
                    var clientList = result.IsConsumed ? null : result.Read();

                    return Ok(FormatResultModel<dynamic>.Success(new { groupList, empList, clientList }));
                }
                catch (Exception e)
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
                }
            }
        }

        [HttpPost("V1/GetData")]
        public async Task<ActionResult> GetData(GetDataRequest request)
        {
            string userOrderByStr = string.Format("ORDER BY {0} {1}",request.SortColumn, request.Sort);
            string offSetStr = request.Mode == String.Empty ?
                @$"OFFSET (@Page-1)*@PerPage ROWS 
                   FETCH NEXT @PerPage ROWS ONLY" :
                "";

            string advanceRequestStr="";
            if (request.AdvanceRequest != null)
            {
                advanceRequestStr=request.AdvanceRequest.GenerateSqlString();
            }

            string selectStr = @$"
                SELECT COUNT(*) Length
                FROM Users
                LEFT JOIN Employee ON UserNumber=EmpID 
                WHERE
	                (UserID LIKE '%'+LOWER(@Q)+'%'
	                OR LOWER(UserName) LIKE '%'+LOWER(@Q)+'%'
	                OR LOWER(UserNumber) LIKE '%'+LOWER(@Q)+'%'
	                OR LOWER(Description) LIKE '%'+LOWER(@Q)+'%'
	                OR LOWER(GroupName) LIKE '%'+LOWER(@Q)+'%')
                    {advanceRequestStr}
                
                SELECT UserID, ISNULL(EmpName,'') UserName, UserNumber AS UserNumberStr, Password, Description, GroupName AS GroupNameStr, Email, PicturePath AS Avatar
                FROM Users
	            LEFT JOIN Employee ON UserNumber=EmpID 
                WHERE
	                (UserID LIKE '%'+LOWER(@Q)+'%'
	                OR LOWER(UserName) LIKE '%'+LOWER(@Q)+'%'
	                OR LOWER(UserNumber) LIKE '%'+LOWER(@Q)+'%'
	                OR LOWER(Description) LIKE '%'+LOWER(@Q)+'%'
	                OR LOWER(GroupName) LIKE '%'+LOWER(@Q)+'%')
                    {advanceRequestStr}
                {userOrderByStr}
                {offSetStr}
            ";

            using (var connection = _POVWebDbContextDapper.Connection)
            {
                connection.Open();
                try
                {
                    var result = await connection.QueryMultipleAsync(
                    selectStr,
                    request,
                    commandTimeout: 180);

                    var userTotal = result.IsConsumed ? 0 : result.Read<UserDataLength>().FirstOrDefault().Length;
                    var userList = result.IsConsumed ? null : result.Read<UserData>();

                    foreach (UserData user in userList)
                    {
                        user.Password = DecodePassword(user.Password);
                        user.UserNumber = new OptionWA(user.UserNumberStr, user.UserName,user.Avatar);
                        user.GroupName = user.GroupNameStr.Split(",").ToList().ConvertAll<Option<string>>(str => new Option<string>(str, str));
                    }

                    return Ok(FormatResultModel<dynamic>.Success(new { userTotal, userList }));
                }
                catch (Exception e)
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
                }
            }
        }

        [HttpPost("V1/GetSingleData")]
        public async Task<ActionResult> GetSingleData(GetSingleDataRequest request)
        {
            string selectStr = @$"
                SELECT C1.value Value, ClientShort Label 
                FROM String_Split((SELECT Top 1 ClientPermission FROM Users WHERE UserID = @UserID),',') C1
                LEFT JOIN Client C2 ON C1.value = C2.ClientID

                SELECT UserID, ISNULL(EmpName,'') UserName, UserNumber AS UserNumberStr, Password, Description, GroupName AS GroupNameStr, Email, PicturePath AS Avatar,
                       CostPermission
                FROM Users
	            LEFT JOIN Employee ON UserNumber=EmpID 
                WHERE UserID=@UserID
            ";

            using (var connection = _POVWebDbContextDapper.Connection)
            {
                connection.Open();
                try
                {
                   var result = await connection.QueryMultipleAsync(
                   selectStr,
                   request,
                   commandTimeout: 180);

                    List<Option<string>> clientPermission = result.IsConsumed ? null : result.Read<Option<string>>().ToList();

                    var user = result.IsConsumed ? null : result.Read<UserData>().FirstOrDefault();

                    if (user == null) throw new Exception("找不到使用者!");


                    user.Password = DecodePassword(user.Password);
                    user.UserNumber = new OptionWA(user.UserNumberStr, user.UserName, user.Avatar);
                    user.GroupName = user.GroupNameStr.Split(",").ToList().ConvertAll<Option<string>>(str => new Option<string>(str, str));
                    user.ClientPermission = clientPermission;

                    return Ok(FormatResultModel<dynamic>.Success(new { user }));
                }
                catch (Exception e)
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
                }
            }
        }

        [HttpPost("V1/AddUsersData")]
        public async Task<ActionResult> AddUsersData(AddUsersDataRequest request)
        {
            request.Password = EncodePassword(request.Password);

            string actionStr =
                @$" Declare @O_MSG VARCHAR(MAX)=N''
			        IF EXISTS(SELECT 1 FROM Users WHERE UserID = @UserID) BEGIN
				        SET @O_MSG = '使用者代號：' + @UserID + '不可重覆!'
				        ;THROW 6636001, @O_MSG, 1;
			        END   

                    DECLARE @UserName VARCHAR(10) = (SELECT TOP 1 EmpName FROM Employee WHERE EmpID = @UserNumber)
                    IF EXISTS(SELECT 1 FROM Users WHERE UserNumber = @UserNumber) BEGIN
				        SET @O_MSG = '人員：' + @UserName + '不可重覆!'
				        ;THROW 6636001, @O_MSG, 1;
			        END   
		
	                BEGIN TRY
	                INSERT INTO [dbo].[Users]
                           (UserID, UserName, UserNumber, Password, Description, GroupName, CostPermission, ClientPermission)
                     VALUES
			                (@UserID, @UserName, @UserNumber, @Password, @Description, @GroupName, @CostPermission, @ClientPermission)

	                END TRY
	                BEGIN CATCH
		                SET @O_MSG = '新增使用者失敗。 編號：' + @UserID
					                + ERROR_MESSAGE() 
		                ;THROW 6636001, @O_MSG, 1;
	                END CATCH	
                ";

            using (var connection = _POVWebDbContextDapper.Connection)
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();
                try
                {
                    var result = await connection.ExecuteAsync(actionStr, request, transaction, commandTimeout: 180);
                    transaction.Commit();
                    return Ok(FormatResultModel<dynamic>.Success(new { MSG = result }));
                }
                catch (Exception e)
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
                }
            }
        }

        [HttpPost("V1/DelUsersData")]
        public async Task<ActionResult> DelUsersData(DelUsersDataRequest request)
        {

            string actionStr =
                @$" Declare @O_MSG VARCHAR(MAX)=N''
                    BEGIN TRY
		                DELETE FROM Users WHERE UserID = @UserID
	                END TRY
	                BEGIN CATCH
		                SET @O_MSG = '刪除資料失敗。' + ERROR_MESSAGE() 
		                ;THROW 6636001, @O_MSG, 1;
	                END CATCH	
                ";

            using (var connection = _POVWebDbContextDapper.Connection)
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();
                try
                {
                    var result = await connection.ExecuteAsync(actionStr, request.DelList, transaction, commandTimeout: 180);
                    transaction.Commit();
                    return Ok(FormatResultModel<dynamic>.Success(new { MSG = result }));
                }
                catch (Exception e)
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
                }
            }
        }

        [HttpPost("V1/UpdUsersData")]
        public async Task<ActionResult> UpdUsersData(UpdUsersDataRequest request)
        {
            request.Password = EncodePassword(request.Password);
            string actionStr =
                 @$" Declare @O_MSG VARCHAR(MAX)=N''
                    IF EXISTS(SELECT 1 FROM Users WHERE UserID = @UserID AND UserID != @OriginalUserID) BEGIN
				        SET @O_MSG = '使用者代號：' + @UserID + '不可重覆!'
				        ;THROW 6636001, @O_MSG, 1;
			        END  

                    DECLARE @UserName VARCHAR(10) = (SELECT TOP 1 EmpName FROM Employee WHERE EmpID = @UserNumber)
                    IF EXISTS(SELECT 1 FROM Users WHERE UserNumber = @UserNumber AND UserID != @OriginalUserID) BEGIN
				        SET @O_MSG = '人員：' + @UserName + '不可重覆!'
				        ;THROW 6636001, @O_MSG, 1;
			        END   

	                BEGIN TRY
	                UPDATE [dbo].[Users] 
                    SET UserID=@UserID, UserName=@UserName, UserNumber=@UserNumber, Password=@Password, Description=@Description, GroupName=@GroupName,
                        CostPermission=@CostPermission, ClientPermission=@ClientPermission
                    WHERE UserID=@OriginalUserID

	                END TRY
	                BEGIN CATCH
		                SET @O_MSG = '更改使用者失敗。 編號：' + @OriginalUserID
					                + ERROR_MESSAGE() 
		                ;THROW 6636001, @O_MSG, 1;
	                END CATCH	
                ";
            using (var connection = _POVWebDbContextDapper.Connection)
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();
                try
                {
                    var result = await connection.ExecuteAsync(actionStr, request, transaction, commandTimeout: 180);
                    transaction.Commit();
                    return Ok(FormatResultModel<dynamic>.Success(new { MSG = result }));
                }
                catch (Exception e)
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
                }
            }
        }

        [HttpPost("V1/ImportExcelData")]
        public  ActionResult ImportExcelData([FromForm] ImportExcelDataRequest request)
        {
            var file = request.File;
            string uploadsPath = Path.Combine(_hostEnvironment.WebRootPath, "images/UserAccount");

            List<UserDataDB> table = new List<UserDataDB>();

            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];

                    int rowCount = worksheet.Dimension.Rows;
                    int colCount = worksheet.Dimension.Columns;
                    
                    for (int row = 2; row <= rowCount; row++)
                    {
                        table.Add(new UserDataDB
                        {
                            UserID = worksheet.Cells[row, 1].Value?.ToString().Trim(),
                            UserName = worksheet.Cells[row, 2].Value?.ToString().Trim(),
                            UserNumber = worksheet.Cells[row, 3].Value?.ToString().Trim(),
                            Password = worksheet.Cells[row, 4].Value?.ToString().Trim(),
                            Description = worksheet.Cells[row, 5].Value?.ToString().Trim(),
                            GroupName = worksheet.Cells[row, 6].Value?.ToString().Trim(),
                            ClientPermission = worksheet.Cells[row, 7].Value?.ToString().Trim(),
                            CostPermission = Convert.ToBoolean(worksheet.Cells[row, 8].Value)
                        });
                    }

                    var images = worksheet.Drawings.Where(d => d is ExcelPicture).Cast<ExcelPicture>();
                    foreach (var image in images)
                    {
                        var imageStream = new MemoryStream(image.Image.ImageBytes);
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.Name)}.png";
                        var filePath = Path.Combine(uploadsPath, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            imageStream.CopyTo(fileStream);
                        }
 
                    }
                }
            }
            return Ok(FormatResultModel<dynamic>.Success(new { msg = "匯入完成" }));
        }

        [NonAction]
        public string EncodePassword(string password)
        {
            string encodedPassword = "";
            if (password == "")
            {
                encodedPassword = "Daizy";
            }
            else
            {
                foreach (char c in password)
                {
                    char tmp = Convert.ToChar(Convert.ToInt32(c) + 4);
                    encodedPassword += tmp;
                }
                int divider = (int)Math.Ceiling(encodedPassword.Length / 2.0);
                char[] arr = (encodedPassword.Substring(divider) + encodedPassword.Substring(0, encodedPassword.Length % 2 == 0 ? encodedPassword.Length - divider : encodedPassword.Length - divider + 1)).ToCharArray();
                Array.Reverse(arr);
                encodedPassword = new string(arr);
            }
            return encodedPassword;
        }

        [NonAction]
        public string DecodePassword(string password)
        {
            string decodedPassword = "";
            if (password == "Daizy")
            {
                decodedPassword = "";
            }
            else
            {
                foreach (char c in password)
                {
                    char tmp = Convert.ToChar(Convert.ToInt32(c) - 4);
                    decodedPassword += tmp;
                }
                int divider = (int)Math.Ceiling(decodedPassword.Length / 2.0);
                char[] arr = (decodedPassword.Substring(divider) + decodedPassword.Substring(0, decodedPassword.Length % 2 == 0 ? decodedPassword.Length - divider : decodedPassword.Length - divider + 1)).ToCharArray();
                Array.Reverse(arr);
                decodedPassword = new string(arr);
            }
            return decodedPassword;
        }
    }
}
