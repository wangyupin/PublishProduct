using CityAdminDomain.Models.API.User;
using CityAdminDomain.Models.Common;
using CityAdminDomain.Models.DB.CityAdmin;
using CityAdminDomain.Models.Enum;
using CityHubCore.Application.Session;
using CityHubCore.Application.Totp;
using CityHubCore.Infrastructure.API;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace CityAdminSrv.Controllers {
    [Route("api/[controller]")]
    public class UserController : ApiControllerBase {
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _config;
        private readonly CityAdminDbContext _cityAdminDb;
        private readonly CityAdminDbContextDapper _cityAdminDbContextDapper;
        private readonly CityAdminDapper _cityAdminDapper;
        private readonly TotpHelper _toptHelper;

        public UserController(ILogger<UserController> logger, IConfiguration config
            , CityAdminDbContext cityAdminDb
            , CityAdminDbContextDapper cityAdminDbContextDapper
            , CityAdminDapper cityAdminDapper
            , TotpHelper toptHelper
            ) {
            _logger = logger;
            _config = config;
            _cityAdminDb = cityAdminDb;
            _cityAdminDbContextDapper = cityAdminDbContextDapper;
            _cityAdminDapper = cityAdminDapper;
            _toptHelper = toptHelper;
        }

        private List<UserSessionModel> CreateSession(long UserId, UserLoginRequest userLoginInfo) {
            Session session = new() {
                users_id = UserId,
                token = Guid.NewGuid().ToString("N").ToUpper(),
                loginIP = userLoginInfo.IpAddress,
                memo = JsonSerializer.Serialize((RequestBase)userLoginInfo),
                createDTM = DateTime.Now
            };

            string insertSessionSql =
                @"INSERT INTO SSO.Session (users_id, token, loginIP, memo, createDTM) 
                VALUES(@users_id, @token, @loginIP, @memo, @createDTM)";

            string insertSessionHistorySql =
                @"INSERT INTO SSO.SessionHistory (users_id, token, loginIP, memo, createDTM) 
                VALUES(@users_id, @token, @loginIP, @memo, @createDTM)";

            string sessionHistoryHousekeepingSql =
                @"DELETE 
                  FROM SSO.SessionHistory
                  WHERE users_id = @users_id
                  AND id IN(SELECT m2.id
                         FROM SSO.SessionHistory m2
                         WHERE m2.users_id = @users_id
                         ORDER BY m2.createDTM DESC
                         OFFSET 6 ROWS)";

            _cityAdminDbContextDapper.Connection.Execute(insertSessionSql,
                       new {
                           session.users_id,
                           session.token,
                           session.loginIP,
                           session.memo,
                           session.createDTM
                       });

            _cityAdminDbContextDapper.Connection.Execute(insertSessionHistorySql,
                new {
                    session.users_id,
                    session.token,
                    session.loginIP,
                    session.memo,
                    session.createDTM
                });

            _cityAdminDbContextDapper.Connection.Execute(sessionHistoryHousekeepingSql,
                new {
                    session.users_id
                });

            //using (var connection = _cityAdminDapper.Connection) {
            //    connection.Open();
            //    using (var transaction = connection.BeginTransaction()) {
            //        connection.Execute(insertSessionSql,
            //            new {
            //                session.users_id,
            //                session.token,
            //                session.loginIP,
            //                session.memo,
            //                session.createDTM
            //            },
            //            transaction: transaction);

            //        connection.Execute(insertSessionHistorySql,
            //            new {
            //                session.users_id,
            //                session.token,
            //                session.loginIP,
            //                session.memo,
            //                session.createDTM
            //            },
            //            transaction: transaction);

            //        connection.Execute(sessionHistoryHousekeepingSql,
            //            new {
            //                session.users_id
            //            },
            //            transaction: transaction);

            //        transaction.Commit();
            //    }
            //}

            //using (var dbContextTransaction = _cityAdminDb.Database.BeginTransaction()) {
            //    //1. Add Session
            //    //_cityAdminDb.Session.Add(session);
            //    //_cityAdminDb.SaveChanges();

            //    //2. backup session to history
            //    _cityAdminDb.SessionHistory.Add(new SessionHistory() {
            //        user_id = session.users_id,
            //        token = session.token,
            //        loginIP = session.loginIP,
            //        memo = session.memo,
            //        createDTM = session.createDTM
            //    });

            //    _cityAdminDb.SaveChanges();

            //    //3. Hourskeeping for sessionHistory 保留最後六筆 
            //    _cityAdminDbContextDapper.Connection.Execute(sessionHistoryHousekeepingSql, new { user_id = session.users_id });

            //    dbContextTransaction.Commit();
            //}

            return _cityAdminDb.Session.Where(x => x.users_id == session.users_id)
                .OrderBy(x => x.createDTM)
                .Select(m => new UserSessionModel() {
                    Token = m.token,
                    LoginIP = m.loginIP,
                    Memo = m.memo,
                    CreateDTM = m.createDTM
                }).ToList(); ;
        }

        /// <summary>
        /// Login
        /// TODO: add PBKDF2 for password moonfeng@2021/9/8 https://www.meziantou.net/how-to-store-a-password-in-a-web-application.htm
        /// </summary>
        /// <param name="userLoginRequest"></param>
        /// <returns></returns>
        [HttpPost("V1/Login")]
        public ResultModel<UserBasicInfo> Login(UserLoginRequest userLoginRequest) {
            #region -- check parameters
            if (userLoginRequest == null) return FormatResultModel<UserBasicInfo>.Failure(null);

            //Check password
            //** EF Core and Linq
            var queryUser = from User in _cityAdminDb.Users
                            join Identity in _cityAdminDb.Identities
                              on User.id equals Identity.users_id
                            where User.company_id.ToUpper() == userLoginRequest.CompanyId.ToUpper()
                                && User.user_name.ToUpper() == userLoginRequest.UserId.ToUpper()
                                && User.status >= (int)UsersStatusType.Enabled
                                && Identity.method == IdentitiesMethodType.PASSWORD.ToString()
                                && Identity.data == userLoginRequest.Password
                            select new { User.id };

            #endregion

            //** TOPT
            if (queryUser.FirstOrDefault() == null) {
                if (int.TryParse(userLoginRequest.Password, out _) && userLoginRequest.Password.Length == 6) {
                    if (_toptHelper.ValidatePIN(userLoginRequest.CompanyId, userLoginRequest.UserId, userLoginRequest.Password)) {
                        queryUser = from User in _cityAdminDb.Users
                                    join Identity in _cityAdminDb.Identities
                                      on User.id equals Identity.users_id
                                    where User.company_id.ToUpper() == userLoginRequest.CompanyId.ToUpper()
                                            && User.user_name.ToUpper() == userLoginRequest.UserId.ToUpper()
                                            && User.status >= (int)UsersStatusType.Enabled
                                            && Identity.method == IdentitiesMethodType.TOPT.ToString()
                                    select new { User.id };
                    }
                }
            }

            if (queryUser.FirstOrDefault() == null) return FormatResultModel<UserBasicInfo>.Failure(null);

            #region -- initial variables
            var userId = queryUser.FirstOrDefault().id;
            UserBasicInfo RET;
            #endregion

            #region -- biz
            RET = _cityAdminDb.Users.Where(m => m.id == userId)
                .Select(m => new UserBasicInfo() {
                    Id = m.id,
                    CompanyId = m.company_id,
                    StoreId = m.store_id,
                    UserId = m.user_name,
                    FullName = m.name,
                    Email = m.email,
                    Role = m.role,
                    TerminalID = m.terminal_id
                }).FirstOrDefault();

            RET.Ability = _cityAdminDb.ACL.Where(x => x.users_id == userId)
                .OrderBy(x => x.subject)
                .Select(m => new UserACLModel() {
                    Subject = m.subject,
                    Action = m.action
                }).ToList();

            var SessionList = _cityAdminDb.Session.Where(x => x.users_id == userId)
                .OrderBy(x => x.createDTM)
                .Select(m => new UserSessionModel() {
                    Token = m.token,
                    LoginIP = m.loginIP,
                    Memo = m.memo,
                    CreateDTM = m.createDTM
                }).ToList();

            RET.LoginDuplication = false;
            if (SessionList.FirstOrDefault() == null) {
                // 建立Session資料
                RET.SessionList = CreateSession(userId, userLoginRequest);
            } else {
                if (userLoginRequest.Force) {
                    //強制登入
                    //1. 清除Session
                    string sql =
                        @"DELETE FROM SSO.Session
                        WHERE users_id = @User_id";
                    _cityAdminDbContextDapper.Connection.Execute(sql, new { User_id = userId });
                    //2. 建立Session資料 & 備份Session to SessionHistory
                    RET.SessionList = CreateSession(userId, userLoginRequest);
                } else {
                    //重覆登入，詢問是否強制登入
                    RET.SessionList = SessionList;
                    RET.LoginDuplication = true;
                }
            }

            #endregion

            #region result
            return FormatResultModel<UserBasicInfo>.Success(RET);
            #endregion
        }

        [HttpPost("V1/Logout")]
        public ResultModel<string> Logout(UserLogoutRequest userLoginRequest) {
            var queryUser = from User in _cityAdminDb.Users
                            where User.company_id.ToUpper() == userLoginRequest.CompanyId.ToUpper()
                                    && User.user_name.ToUpper() == userLoginRequest.UserId.ToUpper()
                            select new { User.id };
            if (queryUser.FirstOrDefault() == null) return FormatResultModel<string>.Failure("User not found");
            var userId = queryUser.FirstOrDefault().id;

            string sql =
                        @"DELETE FROM SSO.Session
                        WHERE users_id = @User_id";
            _cityAdminDbContextDapper.Connection.Execute(sql, new { User_id = userId });

            return FormatResultModel<string>.Success("Logout success");
        }

        [HttpPost("V1/ClearSession")]
        public ResultModel<string> ClearSession(SessionUserInfo sessionUserInfo) {
            var queryUser = from User in _cityAdminDb.Users
                            where User.company_id.ToUpper() == sessionUserInfo.CompanyId.ToUpper()
                                    && User.user_name.ToUpper() == sessionUserInfo.UserId.ToUpper()
                            select new { User.id };
            if (queryUser.FirstOrDefault() == null) return FormatResultModel<string>.Failure("User not found");
            var userId = queryUser.FirstOrDefault().id;

            string sql =
                        @"DELETE FROM SSO.Session
                        WHERE users_id = @User_id AND token = @token";
            int affectedRows = _cityAdminDbContextDapper.Connection.Execute(sql, new { User_id = userId, token = sessionUserInfo.SID });

            return FormatResultModel<string>.Success("Logout success");
        }

        [HttpPost("V1/CheckSessionID")]
        public ResultModel<string> CheckSessionID(SessionUserInfo sessionUserInfo) {
            var queryUser = from User in _cityAdminDb.Users
                            where User.company_id.ToUpper() == sessionUserInfo.CompanyId.ToUpper()
                                    && User.user_name.ToUpper() == sessionUserInfo.UserId.ToUpper()
                            select new { User.id };
            if (queryUser.FirstOrDefault() == null) return FormatResultModel<string>.Failure("User not found");
            var userId = queryUser.FirstOrDefault().id;

            var querySessionID = from Session in _cityAdminDb.Session
                                 where Session.users_id == userId
                                         && Session.token.ToUpper() == sessionUserInfo.SID.ToUpper()
                                 select new { Session.token };

            if (querySessionID.FirstOrDefault() == null) return FormatResultModel<string>.Failure("SessionID not found");

            return FormatResultModel<string>.Success("SessionID found");
        }

        //先Mark
//        [HttpPost("V1/GetById")]
//        public async Task<ResultModel<UserBasicInfo>> GetById(UserGetByIdRequest data) {
//            #region -- check parameters
//            if (data is null) return FormatResultModel<UserBasicInfo>.Failure(null);
//            #endregion

//            #region -- initial variables
//            UserBasicInfo RET = default;
//            #endregion

//            #region -- biz
//            string sql =
//                /* By Stored Procedure*/
//                @"EXEC [SSO].[uspGetUsersByCompanyId_UserName] 
//                @COMPANY_ID, @USER_NAME";
//            /* By Select statement*/
//            //    @"SELECT *
//            //        FROM SSO.Users AS A JOIN SSO.ACL AS B ON A.id = B.users_id
//            //       WHERE company_id = @COMPANY_ID AND user_name = @USER_NAME";
//            var users = await _cityAdminDapper.Connection.QueryAsync(
//                sql,
//                new { COMPANY_ID = data.CompanyId, USER_NAME = data.UserId }
//                );

//            // return null if user not found
//            if (users == null) return FormatResultModel<UserBasicInfo>.Failure(null);

//            var user = users.First();

//            RET = new UserBasicInfo() {
//                Id = user.id,
//                CompanyId = user.company_id,
//                UserId = user.user_name,
//                FullName = user.name,
//                Email = user.email,
//                Role = user.role,
//                Ability = (from USER in users
//                           select new UserACLModel() {
//                               Subject = USER.subject,
//                               Action = USER.action
//                           }).ToList()
//            };

//            #endregion

//            #region result
//            return FormatResultModel<UserBasicInfo>.Success(RET);
//            #endregion
//        }

//        [HttpGet("V1/GetAll")]
//        public ResultModel<IEnumerable<UserBasicInfo>> GetAll() {
//            #region -- check parameters
//            #endregion

//            #region -- initial variables

//            #endregion

//            #region -- biz
//            // SQL 欄位要與 model相同，才會自動mapping. MoonFeng @ 2021/9/9 23:26
//            string sql =
//@"SELECT A.[id] AS Id
//      ,A.[company_id] AS CompanyId
//      ,A.[user_name] AS UserId
//      ,A.[name] AS FullName
//      ,A.[email] AS Email
//      ,A.[role] AS Role
//      ,B.[users_id] AS splitOnKey
//	  ,B.[action] AS Action
//      ,B.[subject] AS Subject
//  FROM [SSO].[Users] AS A JOIN [SSO].[ACL] AS B ON A.id = B.users_id ";
//            var userDictionary = new Dictionary<long, UserBasicInfo>();
//            var users = _cityAdminDapper.Connection.Query<UserBasicInfo, UserACLModel, UserBasicInfo>(
//                    sql,
//                    (user, userACL) => {
//                        UserBasicInfo userEntry;
//                        if (!userDictionary.TryGetValue(user.Id, out userEntry)) {
//                            userEntry = user;
//                            userEntry.Ability = new List<UserACLModel>();
//                            userDictionary.Add(userEntry.Id, userEntry);
//                        }
//                        userEntry.Ability.Add(userACL);
//                        return userEntry;
//                    }, splitOn: "splitOnKey"
//                    ).Distinct()
//                    .ToList();

//            // return null if user not found
//            if (users == null) return FormatResultModel<IEnumerable<UserBasicInfo>>.Failure(null);
//            #endregion

//            #region result
//            return FormatResultModel<IEnumerable<UserBasicInfo>>.Success(users);
//            #endregion
//        }
    
    }
}
