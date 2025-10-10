using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using CityAdminDomain.Models.API.User.AccountSetting;
using CityAdminDomain.Models.DB.CityAdmin;
using CityHubCore.Infrastructure.API;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CityAdminSrv.Controllers
{
    public class AccountSettingController : ApiControllerBase
    {
        private readonly CityAdminDbContext _cityAdminDb;
        private readonly CityAdminDbContextDapper _cityAdminDbContextDapper;
        private readonly CityAdminDapper _cityAdminDapper;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AccountSettingController(
            CityAdminDbContext cityAdminDb
            , CityAdminDbContextDapper cityAdminDbContextDapper
            , CityAdminDapper cityAdminDapper
            , IWebHostEnvironment hostEnvironment
            )
        {
            _cityAdminDb = cityAdminDb;
            _cityAdminDbContextDapper = cityAdminDbContextDapper;
            _cityAdminDapper = cityAdminDapper;
            _hostEnvironment = hostEnvironment;
        }


        [HttpPost("V1/GetUserList")]
        public async Task<ActionResult> GetUserList(GetUserListRequest request)
        {
            string selectStr = @"
SELECT [id]
      ,[company_id]
      ,[user_name]
      ,[data]
      ,[name]
      ,[email]
      ,[role]
      ,[status]
      ,u.[updateDTM]
      ,u.[createDTM]
      ,[role_detail]
      ,[store_id]
      ,[terminal_id]
      ,[Group_Name] Group_Name
      ,[Description] Description
	  FROM [SSO].[Users] u
	  JOIN SSO.Identities i
	  On i.users_id = u.id
      WHERE [company_id] = @company_id      
";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@company_id", request.company_id);

            try
            {
                var result = await _cityAdminDbContextDapper.Connection.QueryMultipleAsync(
                selectStr,
                parameters,
                commandTimeout: 180);

                var userList = result.IsConsumed ? null : result.Read<dynamic>();

                return Ok(FormatResultModel<dynamic>.Success(new
                {
                    userList

                }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }
        [HttpPost("V1/GetUserGroupList")]
        public async Task<ActionResult> GetGroupList(GetGroupListRequest request)
        {
            string selectStr = @"
SELECT *
      FROM [SSO].[UserGroup]
      WHERE [company_id] = @company_id      
";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@company_id", request.company_id);

            try
            {
                var result = await _cityAdminDbContextDapper.Connection.QueryMultipleAsync(
                selectStr, parameters,
                commandTimeout: 180);

                var groupList = result.IsConsumed ? null : result.Read<dynamic>();

                return Ok(FormatResultModel<dynamic>.Success(new
                {
                    groupList

                }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }


        [HttpPost("V1/GetGroupProgramDetail")]
        public async Task<ActionResult> GetGroupProgramDetail(GetGroupProgramDetailRequest request)
        {

            string selectStr =
                @$"SELECT P.Program_Name Program_Name, P.ID ProgramID, Group_Name GroupName,
		            ISNULL(Insert_Flag, '0') Insert_Flag, ISNULL(View_Flag, '0') View_Flag, ISNULL(Edit_Flag, '0') Edit_Flag, ISNULL(Delete_Flag, '0') Delete_Flag, 
		            ISNULL(Print_Flag, '0') Print_Flag, ISNULL(Money_Flag, '0') Money_Flag, ISNULL(Cost_Flag , '0') Cost_Flag,
                    ChangePerson, ChangeDate
                   FROM SSO.Program P
                   LEFT JOIN (SELECT * FROM SSO.GroupProgram WHERE Group_Name = @GroupName AND company_id = @company_id) G ON P.Program_Name = G.Program_Name AND G.company_id = P.company_id 
                   WHERE P.company_id = @company_id
                ";


            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@GroupName", request.GroupName);
            parameters.Add("@company_id", request.company_id);
            try
            {
                var groupProgramDetail = await _cityAdminDbContextDapper.Connection.QueryAsync(selectStr, parameters);

                return Ok(FormatResultModel<dynamic>.Success(new { groupProgramDetail }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }

        [HttpPost("V1/AddUsers")]
        public async Task<ActionResult> AddUser(AddUserRequest request)
        {

            string actionStr =
                @$" 
DECLARE @O_MSG VARCHAR(MAX) = N''

IF EXISTS (SELECT 1 FROM [SSO].Users WHERE [user_name] = @user_name AND [company_id] = @company_id) 
BEGIN
    SET @O_MSG = '使用者：' + @user_name + '已有帳號!';
    THROW 6636001, @O_MSG, 1;
END   

INSERT INTO [SSO].[Users] ([company_id], [user_name], [name], [email], [role], [status], [updateDTM], [createDTM], [Group_Name], [Description])
VALUES (@company_id, @user_name, @name, @email, @role, @status, GETDATE(), GETDATE(), @Group_Name, @Description)

DECLARE @UserID BIGINT

SELECT @UserID = id
FROM [CityAdminDB].[SSO].[Users]
WHERE company_id = @company_id AND user_name = @user_name

INSERT INTO [CityAdminDB].[SSO].[Identities]
    ([users_id], [method], [data], [is_enable], [updateDTM], [createDTM])
VALUES
    (@UserID, 'PASSWORD', @password, '1', GETDATE(), GETDATE())

INSERT INTO [CityAdminDB].[SSO].[ACL]
    ([users_id], [action], [subject], [updateDTM], [createDTM])
VALUES
    (@UserID, 'manage', 'all', GETDATE(), GETDATE())

SELECT [id]
      ,[company_id]
      ,[user_name]
      ,[data]
      ,[name]
      ,[email]
      ,[role]
      ,[status]
      ,u.[updateDTM]
      ,u.[createDTM]
      ,[role_detail]
      ,[store_id]
      ,[terminal_id]
      ,[Group_Name]
      ,[Description]
	  FROM [SSO].[Users] u
	  JOIN SSO.Identities i
	  On i.users_id = u.id
      WHERE [company_id] = @company_id   
                ";
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@user_name", request.user_name);
                parameters.Add("@company_id", request.company_id);
                parameters.Add("@password", request.Password);
                parameters.Add("@role", "RPT");
                parameters.Add("@status", 10);
                parameters.Add("@name", request.name); ;
                parameters.Add("@email", request.Email);
                parameters.Add("@Group_Name", request.GroupName);
                parameters.Add("@Description", request.Description);

                var userList = await _cityAdminDbContextDapper.Connection.QueryAsync(actionStr, parameters);

                return Ok(FormatResultModel<dynamic>.Success(new { userList }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }

        [HttpPost("V1/DelUsers")]
        public async Task<ActionResult> DelUser(DelUserRequest request)
        {

            string actionStr =
                @$" Declare @O_MSG VARCHAR(MAX)=N''
                    BEGIN TRY
		                DECLARE @UserID BIGINT

                        SELECT @UserID = id
                        FROM [CityAdminDB].[SSO].[Users]
                        WHERE company_id = @company_id AND user_name = @user


                        DELETE FROM [SSO].[Session]
                              WHERE [users_id] = @UserID;
                        DELETE FROM [SSO].[SessionHistory]
                              WHERE [users_id] = @UserID;
                        DELETE FROM [SSO].[ACL]
                              WHERE [users_id] = @UserID;
                        DELETE FROM [SSO].[Identities]
                              WHERE [users_id] = @UserID;
                        DELETE FROM [SSO].[Users]
                              WHERE [ID] = @UserID;          
SELECT [id]
      ,[company_id]
      ,[user_name]
      ,[data]
      ,[name]
      ,[email]
      ,[role]
      ,[status]
      ,u.[updateDTM]
      ,u.[createDTM]
      ,[role_detail]
      ,[store_id]
      ,[terminal_id]
      ,[Group_Name]
      ,[Description]
	  FROM [SSO].[Users] u
	  JOIN SSO.Identities i
	  On i.users_id = u.id
      WHERE [company_id] = @company_id   
	                END TRY
	                BEGIN CATCH
		                SET @O_MSG = '刪除資料失敗。' + ERROR_MESSAGE() 
		                ;THROW 6636001, @O_MSG, 1;
	                END CATCH	
                ";

            var paramList = new DynamicParameters();
            paramList.Add("@user", request.user_name);
            paramList.Add("@company_id", request.company_id);
            try
            {

                var userList = await _cityAdminDbContextDapper.Connection.QueryAsync(actionStr, paramList);


                return Ok(FormatResultModel<dynamic>.Success(new { userList }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }

        [HttpPost("V1/UpdUsers")]
        public async Task<ActionResult> UpdUser(UpdUserRequest request)
        {
            string actionStr =
                 @$"
DECLARE @O_MSG VARCHAR(MAX) = N''

IF (EXISTS(SELECT 1 FROM [SSO].[Users] WHERE user_name = @new_user AND company_id = @company_id) AND (@new_user <> @user)) 
BEGIN
    SET @O_MSG = '使用者代號：' + @new_user + '不可重覆!';
    THROW 6636001, @O_MSG, 1;
END  

BEGIN TRY
    
	Declare @users_id bigint
	SELECT @users_id = [id] FROM [SSO].[Users] WHERE user_name = @user AND company_id = @company_id

    UPDATE [SSO].[Users] 
    SET [user_name] = @new_user, [name] = @name, [email] = @email
, [Group_Name] = @GroupName
, [Description] = @Description, [updateDTM] = GETDATE()
--, [role] = @role, [status] = @status, [store_id] = @store_id

    WHERE [user_name] = @user AND [company_id] = @company_id

    UPDATE [SSO].[Identities]
    SET [data] = @password,
       -- [is_enable] = @is_enable,
        [updateDTM] = GETDATE()
    WHERE [users_id] = @users_id


SELECT [id]
      ,[company_id]
      ,[user_name]
      ,[data]
      ,[name]
      ,[email]
      ,[role]
      ,[status]
      ,u.[updateDTM]
      ,u.[createDTM]
      ,[role_detail]
      ,[store_id]
      ,[terminal_id]
      ,[Group_Name]
      ,[Description]
	  FROM [SSO].[Users] u
	  JOIN SSO.Identities i
	  On i.users_id = u.id
      WHERE [company_id] = @company_id 
END TRY

BEGIN CATCH
    SET @O_MSG = '更改使用者失敗。 編號：' + @user + ERROR_MESSAGE();
    THROW 6636001, @O_MSG, 1;
END CATCH
                ";
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@user", request.user_name);
                parameters.Add("@new_user", request.new_username);
                parameters.Add("@company_id", request.company_id);
                parameters.Add("@password", request.Password);
                //parameters.Add("@role", request.role);
                //parameters.Add("@status", request.status);
                parameters.Add("@name", request.name); ;
                parameters.Add("@email", request.email);
                //parameters.Add("@status", request.status);
                parameters.Add("@email", request.email);
                //parameters.Add("@store_id", request.store_id);
                parameters.Add("@data", request.Password);
                //parameters.Add("@is_enable", request.is_enable);
                parameters.Add("@GroupName", request.GroupName);
                parameters.Add("@description", request.Description);

                var userList = await _cityAdminDbContextDapper.Connection.QueryAsync(actionStr, parameters);

                return Ok(FormatResultModel<dynamic>.Success(new { userList }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }

        [HttpPost("V1/AddUserGroup")]
        public async Task<ActionResult> AddUserGroup(AddUserGroupRequest request)
        {
            var paramList = GetGroupProgramDetaiParamList(request.GroupProgramDetail);
            paramList.Add("@ChangePerson", request.ChangePerson);
            paramList.Add("@ChangeDate", request.ChangeDate);
            paramList.Add("@GroupName", request.GroupName);
            paramList.Add("@Description", request.Description);
            paramList.Add("@company_id", request.company_id);

            string actionStr =
                @$" Declare @O_MSG VARCHAR(MAX)=N''
IF EXISTS(SELECT 1 FROM [SSO].[UserGroup] WHERE [Group_Name] = @GroupName AND [company_id] = @company_id) 
BEGIN
    SET @O_MSG = '群組名稱：' + @GroupName + '不可重覆!'
    ;THROW 6636001, @O_MSG, 1;
END     

BEGIN TRY
INSERT INTO [SSO].[UserGroup]
       ([Group_Name], [Description],[company_id])
 VALUES
        (@GroupName, @Description, @company_id)

INSERT INTO [SSO].[GroupProgram]
        ( [Group_Name],[Program_ID], [Program_Name],[company_id], [Insert_Flag], [View_Flag], [Edit_Flag], [Delete_Flag], [Print_Flag],
          [Money_Flag], [Cost_Flag], [ChangePerson], [ChangeDate]) 
SELECT
        @GroupName, D.Program_ID, D.Program_Name, @company_id,D.Insert_Flag, D.View_Flag, D.Edit_Flag, D.Delete_Flag, D.Print_Flag,
        D.Money_Flag, D.Cost_Flag, @ChangePerson, @ChangeDate
FROM @GroupProgramDetail D

SELECT [Group_Name], [Description], [company_id] FROM [SSO].[UserGroup]

END TRY
BEGIN CATCH
    SET @O_MSG = '新增群組失敗。 編號：' + @GroupName
                + ERROR_MESSAGE() 
    ;THROW 6636001, @O_MSG, 1;
END CATCH
                ";
            try
            {
                var result = await _cityAdminDbContextDapper.Connection.QueryMultipleAsync(actionStr, paramList);
                var groupList = result.IsConsumed ? null : result.Read();

                return Ok(FormatResultModel<dynamic>.Success(new { groupList }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }

        [HttpPost("V1/DelUserGroup")]
        public async Task<ActionResult> DelUserGroup(DelUserGroupRequest request)
        {

            string actionStr =
                @$" 
Declare @O_MSG VARCHAR(MAX)=N''
BEGIN TRY
    DELETE FROM [SSO].[UserGroup] WHERE [Group_Name] = @GroupName AND [company_id] = @company_id
    DELETE FROM [SSO].[GroupProgram] WHERE [Group_Name] = @GroupName AND [company_id] = @company_id
SELECT *
      FROM [SSO].[UserGroup]
      WHERE [company_id] = @company_id  

END TRY
BEGIN CATCH
    SET @O_MSG = '刪除群組失敗。' + ERROR_MESSAGE() 
    ;THROW 6636001, @O_MSG, 1;
END CATCH	
                ";
            try
            {
                var result = await _cityAdminDbContextDapper.Connection.QueryMultipleAsync(actionStr, request);
                var groupList = result.IsConsumed ? null : result.Read();

                return Ok(FormatResultModel<dynamic>.Success(new { groupList }));
            }
            catch (Exception ex)
            {
                return Ok(FormatResultModel<dynamic>.Failure(new { MSG = ex.Message }));
            }
        }

        [HttpPost("V1/UpdUserGroup")]
        public async Task<ActionResult> UpdUserGroup(UpdUserGroupRequest request)
        {
            var paramList = new DynamicParameters();
            paramList.Add("@OriginalGroupName", request.OriginalGroupName);
            paramList.Add("@Description", request.Description);
            paramList.Add("@GroupName", request.GroupName);
            paramList.Add("@company_id", request.company_id);
            var paramupdDetail = GetGroupProgramDetaiParamList(request.GroupProgramDetail);
            paramupdDetail.Add("@ChangePerson", request.ChangePerson);
            paramupdDetail.Add("@ChangeDate", request.ChangeDate);
            paramupdDetail.Add("@OriginalGroupName", request.OriginalGroupName);
            paramupdDetail.Add("@GroupName", request.GroupName);
            paramupdDetail.Add("@Description", request.Description);
            paramupdDetail.Add("@company_id", request.company_id);
            string actionStr =
                            @$" 
Declare @O_MSG VARCHAR(MAX)=N''
IF EXISTS(SELECT 1 FROM [SSO].[UserGroup] WHERE [Group_Name] = @GroupName AND [Group_Name] != @OriginalGroupName AND [company_id] = @company_id) BEGIN
    SET @O_MSG = '群組代號：' + @GroupName + '不可重覆!';
    THROW 6636001, @O_MSG, 1;
END  

BEGIN TRY
UPDATE [SSO].[UserGroup] 
SET Group_Name=@GroupName, Description=@Description
WHERE Group_Name=@OriginalGroupName AND [company_id] = @company_id
UPDATE [SSO].[Users] 
SET [Group_Name] = @GroupName
WHERE Group_Name=@OriginalGroupName AND [company_id] = @company_id

SELECT [id]
      ,[company_id]
      ,[user_name]
      ,[data]
      ,[name]
      ,[email]
      ,[role]
      ,[status]
      ,u.[updateDTM]
      ,u.[createDTM]
      ,[role_detail]
      ,[store_id]
      ,[terminal_id]
      ,[Group_Name]
      ,[Description]
	  FROM [SSO].[Users] u
	  JOIN SSO.Identities i
	  On i.users_id = u.id
      WHERE [company_id] = @company_id   

SELECT * FROM [SSO].[UserGroup] WHERE [company_id] = @company_id
END TRY
BEGIN CATCH
    SET @O_MSG = '更改群組失敗。 編號：' + @OriginalGroupName
                + ERROR_MESSAGE() 
    ;THROW 6636001, @O_MSG, 1;
END CATCH	
                ";
            string updDetailStr = @"
DELETE G
	FROM [SSO].[GroupProgram] G
	LEFT JOIN @GroupProgramDetail D ON G.Program_Name = D.Program_Name 
	WHERE Group_Name = @OriginalGroupName AND D.Program_Name IS NULL AND G.company_id = @company_id

	MERGE [SSO].[GroupProgram] G
	USING @GroupProgramDetail AS D 
	ON G.Program_Name = D.Program_Name AND G.Group_Name = @OriginalGroupName AND D.company_id = G.company_id
	WHEN MATCHED THEN
		UPDATE SET  
			G.Group_Name = @GroupName,
			G.Insert_Flag = D.Insert_Flag, G.View_Flag = D.View_Flag, G.Edit_Flag = D.Edit_Flag, 
			G.Delete_Flag = D.Delete_Flag, G.Print_Flag = D.Print_Flag, G.Money_Flag = D.Money_Flag, G.Cost_Flag = D.Cost_Flag,
			G.ChangeDate = @ChangeDate, G.ChangePerson = @ChangePerson
	WHEN NOT MATCHED BY TARGET
		THEN INSERT (
			Group_Name, company_id, Program_ID, Program_Name, Insert_Flag, View_Flag, Edit_Flag, Delete_Flag, Print_Flag,
			Money_Flag, Cost_Flag, ChangePerson, ChangeDate) 
		VALUES (
			@GroupName, @company_id, D.Program_ID, D.Program_Name, D.Insert_Flag, D.View_Flag, D.Edit_Flag, D.Delete_Flag, D.Print_Flag,
			D.Money_Flag, D.Cost_Flag, @ChangePerson, @ChangeDate);
";


            using (var conn = _cityAdminDbContextDapper.Connection)
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {

                    try
                    {
                        var result = await conn.QueryMultipleAsync(actionStr, paramList, transaction);
                        var users = result.IsConsumed ? null : result.Read();
                        var usergroup = result.IsConsumed ? null : result.Read();
                        var result2 = await conn.ExecuteAsync(
                            updDetailStr,
                            paramupdDetail,
                            transaction
                            );
                        transaction.Commit();
                        return Ok(FormatResultModel<dynamic>.Success(new { users, usergroup }));
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        transaction.Dispose();
                        return Ok(FormatResultModel<dynamic>.Failure(new { MSG = e.Message }));
                    }
                }
            }


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

        [NonAction]
        public DynamicParameters GetGroupProgramDetaiParamList(List<GroupProgramDetail> request)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Program_Name", typeof(string));
            dt.Columns.Add("Program_ID", typeof(string));
            dt.Columns.Add("company_id", typeof(string));
            dt.Columns.Add("Insert_Flag", typeof(bool));
            dt.Columns.Add("View_Flag", typeof(bool));
            dt.Columns.Add("Edit_Flag", typeof(bool));
            dt.Columns.Add("Delete_Flag", typeof(bool));
            dt.Columns.Add("Print_Flag", typeof(bool));
            dt.Columns.Add("Money_Flag", typeof(bool));
            dt.Columns.Add("Cost_Flag", typeof(bool));

            foreach (var row in request)
            {
                dt.Rows.Add(row.Program_Name, row.Program_ID, row.company_id, bool.Parse(row.Insert_Flag), bool.Parse(row.View_Flag), bool.Parse(row.Edit_Flag), bool.Parse(row.Delete_Flag), bool.Parse(row.Print_Flag), bool.Parse(row.Money_Flag), bool.Parse(row.Cost_Flag));
            }

            var paramList = new DynamicParameters();
            paramList.Add("@GroupProgramDetail", dt.AsTableValuedParameter("[GroupProgramDetail]"));
            return paramList;
        }
    }
}
