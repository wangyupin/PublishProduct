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
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSrv.Application.Services {
    /// <summary>
    /// 在StoreSrv與HqSrv離線狀態下，採用的UserAuth實作. 
    /// Todo: StoreSrv與HqSrv離線狀態下，採用的UserAuth實作。如要連線POVSqlF，檢查使用者驗證....
    /// </summary>
    public class UserAuthOfflineService : IUserAuth{
        private readonly ILogger<UserAuthOfflineService> _logger;

        public string ImplementationName => "UserAuthOfflineService";

        public UserAuthOfflineService(
            ILogger<UserAuthOfflineService> logger
            ) {
            _logger = logger;
        }

        /// StoreWeb會用到，必要實作
        public Task<UserBasicInfo> Login(UserLoginRequest model) {
            throw new NotImplementedException();
        }

        /// StoreWeb會用到，必要實作
        public Task<RefreshTokenResponse> RefreshToken(string token, string ipAddress) {
            throw new NotImplementedException();
        }

        /// StoreWeb會用到，必要實作
        public Task<bool> ClearSession(SessionUserInfo request) {
            throw new NotImplementedException();
        }

        /// StoreWeb會用到，必要實作
        public Task<bool> IsSessionIDVaild(SessionUserInfo request) {
            throw new NotImplementedException();
        }

        /// StoreWeb會用到，必要實作
        public Task Logout(UserLogoutRequest request) {
            throw new NotImplementedException();
        }

        public void RevokeToken(string token, string ipAddress) {
            throw new NotImplementedException();
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