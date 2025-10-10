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
    /// 在StoreSrv與HqSrv連線狀態下，採用的UserAuth實作. 
    /// </summary>
    public class UserAuthOnlineService : IUserAuth{
        private readonly ILogger<UserAuthOnlineService> _logger;
        private readonly HqSrvClient _hQSrvClient;

        public string ImplementationName => "UserAuthOnlineService";

        public UserAuthOnlineService(
            ILogger<UserAuthOnlineService> logger
            , HqSrvClient srvClient
            ) {
            _logger = logger;
            _hQSrvClient = srvClient;
        }

        public async Task<UserBasicInfo> Login(UserLoginRequest model) {
            var result = await _hQSrvClient.HttpPostAsync<ResultModel<UserBasicInfo>>("api/auth/v1/login", model);
            if (result is object) {
                if (result.Succeeded is false || result.Data is null) return null;
            } else {
                return null;
            }

            return result.Data;
        }

        public Task<RefreshTokenResponse> RefreshToken(string token, string ipAddress) {
            throw new NotImplementedException();
        }

        public void RevokeToken(string token, string ipAddress) {
            throw new NotImplementedException();
        }

        public async Task Logout(UserLogoutRequest request) {
            await _hQSrvClient.HttpPostAsync<ResultModel<string>>("api/auth/v1/logout", request);
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

        public async Task<bool> ClearSession(SessionUserInfo request) {
            var result = await _hQSrvClient.HttpPostAsync<ResultModel<bool>>("api/auth/v1/ClearSession", request);
            return result.Succeeded;
        }

        public async Task<bool> IsSessionIDVaild(SessionUserInfo request) {
            var result = await _hQSrvClient.HttpPostAsync<ResultModel<bool>>("api/auth/v1/CheckSessionID", request);
            return result.Succeeded;
        }

        public async Task<CheckIsFront> CheckIsFront(UserLoginRequest request)
        {
            var result = await _hQSrvClient.HttpPostAsync<ResultModel<CheckIsFront>>("api/auth/v1/CheckIsFront", request);
            if (result is object)
            {
                if (result.Succeeded is false || result.Data is null) return null;
            }
            else
            {
                return null;
            }
            return result.Data;
        }

        public async Task<UpdMacModel> UpdMacAddress(UserLoginRequest request)
        {
            var result = await _hQSrvClient.HttpPostAsync<ResultModel<UpdMacModel>>("api/auth/v1/UpdMacAddress", request);
            return result.Data;
        }
    }
}