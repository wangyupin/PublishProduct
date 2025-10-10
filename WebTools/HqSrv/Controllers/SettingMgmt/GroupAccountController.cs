using CityHubCore.Infrastructure.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using POVWebDomain.Models.DB.POVWeb;
using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using CityHubCore.Infrastructure.ServiceClient;
using System.Data;
using Microsoft.AspNetCore.Hosting;
using POVWebDomain.Models.API.StoreSrv.SettingMgmt.GroupAccount;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace HqSrv.Controllers.SettingMgmt
{
    public class GroupAccountController : ApiControllerBase
    {
        private readonly ILogger<GroupAccountController> _logger;
        private readonly POVWebDbContext _POVWebDb;
        private readonly POVWebDbContextDapper _POVWebDbContextDapper;
        private readonly CityAdminSrvClient _CityAdminSrvClient;
        private readonly IConfiguration _Config;
        private readonly IWebHostEnvironment _hostEnvironment;

        public GroupAccountController(
            ILogger<GroupAccountController> logger
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
                SELECT DISTINCT Group_Name GroupName, Description FROM UserGroup

                SELECT GroupName, EmpName Content, PicturePath AS Avatar
                FROM Users
                LEFT JOIN Employee ON UserNumber = EmpID

                SELECT P.PermissionID, P.ProgramID, ISNULL(NULLIF(G1.DisplayName, ''), G1.Program_Name) ParentName, ISNULL(NULLIF(G1.DisplayName, '') ,G.Program_Name) ProgramName, P.PermissionName, P.Description
                FROM Permission P 
                LEFT JOIN Program G ON P.ProgramID = G.ProgramID
                LEFT JOIN Program G1 ON G.ParentID = G1.ProgramID
                WHERE G.Enable = 1
                ORDER BY G.ParentID
            ";
            try
            {
                var result = await _POVWebDbContextDapper.Connection.QueryMultipleAsync(
                selectStr,
                commandTimeout: 180);

                var groupResponse = result.IsConsumed ? null : result.Read<GroupResponse>();
                var userResponse = result.IsConsumed ? null : result.Read<UserResponse>();
                var permissionResponse = result.IsConsumed ? null : result.Read<PermissionResponse>();

                List<Group> groupList = new List<Group>();

                foreach (var group in groupResponse)
                {
                    List<UserResponse> users = userResponse.Where(user => Array.Exists(user.GroupName.Split(","), groupName=> groupName == group.GroupName)).ToList();
                    groupList.Add(new Group(group.GroupName, group.Description, users));
                }

                List<String> orderStr = new List<String>() { "查詢", "新增", "上架", "修改", "刪除", "匯出" };
                var permissionList = permissionResponse
                    .GroupBy(permission => new { permission.ParentName, permission.ProgramID, permission.ProgramName })
                    .Select(program => new SingleProgram()
                    {
                        ParentName = program.Key.ParentName,
                        ProgramID = program.Key.ProgramID,
                        ProgramName = program.Key.ProgramName,
                        Permission = program
                                        .Select(program => new Permission(program.PermissionID, program.PermissionName, program.Description))
                                        .OrderBy(program => {
                                            var index = orderStr.IndexOf(program.Description);
                                            return index == -1 ? int.MaxValue : index;
                                        })
                                        .ToList()
                    })
                    .ToList();

                return Ok(FormatResultModel<dynamic>.Success(new { groupList, permissionList }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }

        [HttpPost("V1/GetGroupPermission")]
        public async Task<ActionResult> GetGroupPermission(GetGroupPermissionRequest request)
        {
            
            string selectStr = @$"
                SELECT UP.PermissionID, P.ProgramID
                FROM UserGroupPermission UP 
                LEFT JOIN Permission P ON UP.PermissionID = P.PermissionID
                WHERE Group_Name = @GroupName
            ";

            List<GetGroupPermissionResponse> result;

            using (var connection = _POVWebDbContextDapper.Connection)
            {
                connection.Open();
                try
                {
                    result = (await connection.QueryAsync<GetGroupPermissionResponse>(
                       selectStr,
                       request,
                       commandTimeout: 180)).ToList();
                }
                catch (Exception e)
                {
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
                }
            }

            var programList = result
                    .GroupBy(permission => new { permission.ProgramID })
                    .Select(program => new
                    {
                        program.Key.ProgramID,
                        Permission = program.Select(program => new { program.PermissionID }).ToList()
                    });

            return Ok(FormatResultModel<dynamic>.Success(new { programList }));
        }

        [HttpPost("V1/AddGroupPermission")]
        public async Task<ActionResult> AddGroupPermission(AddGroupPermissionRequest request)
        {
            var dt = new DataTable();
            dt.Columns.Add("str1");

            foreach(int permissionID in request.Permission)
            {
                dt.Rows.Add(permissionID);
            }

            var paramList = new DynamicParameters();
            paramList.Add("@PermissionTable", dt.AsTableValuedParameter("[POVWeb].[udtStr]"));
            paramList.Add("@GroupName", request.GroupName);
            paramList.Add("@Description", request.Description);
            paramList.Add("@UserID", request.UserID);

            string executeStr = @$"
                Declare @O_MSG VARCHAR(MAX)=N''
                IF EXISTS(SELECT 1 FROM UserGroup WHERE Group_Name = @GroupName) BEGIN
				    SET @O_MSG = '此群組名稱已經存在!'
				    ;THROW 6636001, @O_MSG, 1;
			    END  
                IF (SELECT [POVWeb].[udfHadAuthority]('群組權限管理', @UserID, 'create'))=0 BEGIN
				    SET @O_MSG = '此用戶沒有執行權限!'
				    ;THROW 6636001, @O_MSG, 1;
			    END 

                INSERT INTO UserGroup(Group_Name, Description) VALUES(@GroupName, @Description)
                
                INSERT INTO UserGroupPermission(Group_Name, PermissionID, ChangePerson)
                SELECT @GroupName, str1, @UserID
                FROM @PermissionTable
            ";

            using (var connection = _POVWebDbContextDapper.Connection)
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();
                try
                {
                    var result = await connection.ExecuteAsync(
                        executeStr,
                        paramList,
                        transaction: transaction,
                        commandTimeout: 180);
                    transaction.Commit();
                    return Ok(FormatResultModel<dynamic>.Success(result));
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
                }
            }
        }

        [HttpPost("V1/UpdGroupPermission")]
        public async Task<ActionResult> UpdGroupPermission(UpdGroupPermissionRequest request)
        {
            var dt = new DataTable();
            dt.Columns.Add("str1");

            foreach (int permissionID in request.Permission)
            {
                dt.Rows.Add(permissionID);
            }

            var paramList = new DynamicParameters();
            paramList.Add("@PermissionTable", dt.AsTableValuedParameter("[POVWeb].[udtStr]"));
            paramList.Add("@OriginalGroupName", request.OriginalGroupName);
            paramList.Add("@GroupName", request.GroupName);
            paramList.Add("@Description", request.Description);
            paramList.Add("@UserID", request.UserID);

            string executeStr = @$"
                Declare @O_MSG VARCHAR(MAX)=N''
                IF @GroupName != @OriginalGroupName AND EXISTS(SELECT 1 FROM UserGroup WHERE Group_Name = @GroupName) BEGIN
				    SET @O_MSG = '此群組名稱已經存在!'
				    ;THROW 6636001, @O_MSG, 1;
			    END
                IF (SELECT [POVWeb].[udfHadAuthority]('群組權限管理', @UserID, 'update'))=0 BEGIN
				    SET @O_MSG = '此用戶沒有執行權限!'
				    ;THROW 6636001, @O_MSG, 1;
			    END 

                UPDATE UserGroup SET Group_Name = @GroupName, Description = @Description WHERE Group_Name = @OriginalGroupName
            
                DELETE FROM UserGroupPermission WHERE Group_Name = @OriginalGroupName   

                INSERT INTO UserGroupPermission(Group_Name, PermissionID, ChangePerson)
                SELECT @GroupName, str1, @UserID
                FROM @PermissionTable
            ";

            using (var connection = _POVWebDbContextDapper.Connection)
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();
                try
                {
                    var result = await connection.ExecuteAsync(
                        executeStr,
                        paramList,
                        transaction: transaction,
                        commandTimeout: 180);
                    transaction.Commit();
                    return Ok(FormatResultModel<dynamic>.Success(result));
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
                }
            }
        }

        [HttpPost("V1/DelGroupPermission")]
        public async Task<ActionResult> DelGroupPermission(DelGroupPermissionRequest request)
        {

            string executeStr = @$"
                Declare @O_MSG VARCHAR(MAX)=N''
                IF (SELECT [POVWeb].[udfHadAuthority]('群組權限管理', @UserID, 'delete'))=0 BEGIN
				    SET @O_MSG = '此用戶沒有執行權限!'
				    ;THROW 6636001, @O_MSG, 1;
			    END 

                DELETE FROM UserGroup WHERE Group_Name = @GroupName
            
                DELETE FROM UserGroupPermission WHERE Group_Name = @GroupName
            ";

            using (var connection = _POVWebDbContextDapper.Connection)
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();
                try
                {
                    var result = await connection.ExecuteAsync(
                        executeStr,
                        request,
                        transaction: transaction,
                        commandTimeout: 180);
                    transaction.Commit();
                    return Ok(FormatResultModel<dynamic>.Success(result));
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
                }
            }
        }
    }
}
