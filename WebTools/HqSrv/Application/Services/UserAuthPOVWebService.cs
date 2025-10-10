using AutoMapper;
using CityAdminDomain.Models.API.User;
using CityAdminDomain.Models.Common;
using CityAdminDomain.Services.User;
using CityHubCore.Application.Base;
using CityHubCore.Application.Exceptions;
using CityHubCore.Application.Jwt;
using CityHubCore.Application.Session;
using CityHubCore.Infrastructure.API;
using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using POVWebDomain.Models.DB.POVWeb;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HqSrv.Application.Services {
    /// <summary>
    /// ToDo: 實作原本POVSQL登入登出的程序，如檢查DB中的使用者帳號
    /// </summary>
    public class UserAuthPOVWebService : IUserAuth {
        private readonly ILogger<UserAuthPOVWebService> _logger;
        private readonly POVWebDbContext _POVWebDb;
        private readonly POVWebDbContextDapper _POVWebDbContextDapper;

        public string ImplementationName => "UserAuthPOVWebService";

        public UserAuthPOVWebService(
            ILogger<UserAuthPOVWebService> logger
            , POVWebDbContext pOVWebDbContext
            , POVWebDbContextDapper pOVWebDbContextDapper
            ) {
            _logger = logger;
            _POVWebDb = pOVWebDbContext;
            _POVWebDbContextDapper = pOVWebDbContextDapper;
        }

        public Task<UserBasicInfo> Login(UserLoginRequest model) {
            #region -- check parameters

            #endregion

            #region -- initial variables

            #endregion

            #region -- Biz

            #endregion

            #region -- result
            var result = new UserBasicInfo();
            return Task.FromResult(result);

            #endregion
        }

        public Task<RefreshTokenResponse> RefreshToken(string token, string ipAddress) {
            throw new NotImplementedException();
        }

        public void RevokeToken(string token, string ipAddress) {
            throw new NotImplementedException();
        }

        public Task Logout(UserLogoutRequest request) {

            #region -- check parameters

            #endregion

            #region -- initial variables

            #endregion

            #region -- Biz

            #endregion


            #region -- result
            return Task.FromResult("NotImplemented");
            #endregion
        }

        public void Register(UserBasicInfo model) {
            throw new NotImplementedException();
        }

        public void ForgotPassword(UserBasicInfo model) {
            throw new NotImplementedException();
        }

        public void ResetPassword(UserBasicInfo model) {
            throw new NotImplementedException();
        }

        public void VerifyEmail(UserBasicInfo model) {
            throw new NotImplementedException();
        }

        public void TwoStepsVerify(UserBasicInfo model) {
            throw new NotImplementedException();
        }

        public Task<bool> ClearSession(SessionUserInfo request) {
            throw new NotImplementedException();
        }

        public Task<bool> IsSessionIDVaild(SessionUserInfo request) {
            throw new NotImplementedException();
        }

        public Task<CheckIsFront> CheckIsFront(UserLoginRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<UpdMacModel> UpdMacAddress(UserLoginRequest request)
        {
            throw new NotImplementedException();
        }
    }
}