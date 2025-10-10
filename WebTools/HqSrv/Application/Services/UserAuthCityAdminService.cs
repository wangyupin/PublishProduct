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

namespace HqSrv.Application.Services {
    /// <summary>
    /// °õ¦æ CityAdmin ªº UserAuth Flow
    /// </summary>
    public class UserAuthCityAdminService : IUserAuth {
        private readonly ILogger<UserAuthCityAdminService> _logger;
        private readonly CityAdminSrvClient _cityAdminSrvClient;

        public string ImplementationName => "UserAuthCityAdminService";

        public UserAuthCityAdminService(
            ILogger<UserAuthCityAdminService> logger
            , CityAdminSrvClient cityAdminSrvClient
            ) {
            _logger = logger;
            _cityAdminSrvClient = cityAdminSrvClient;
        }

        public async Task<UserBasicInfo> Login(UserLoginRequest request) {
            var result = await _cityAdminSrvClient.HttpPostAsync<ResultModel<UserBasicInfo>>("api/auth/v1/Login", request);

            return result.Data;
        }

        public async Task Logout(UserLogoutRequest request) {

            await _cityAdminSrvClient.HttpPostAsync<ResultModel<string>>("api/auth/v1/Logout", request);
        }

        public async Task<bool> ClearSession(SessionUserInfo request) {
            var result = await _cityAdminSrvClient.HttpPostAsync<ResultModel<string>>("api/auth/v1/ClearSession", request);

            return result.Succeeded;
        }

        public async Task<bool> IsSessionIDVaild(SessionUserInfo request) {
            var result = await _cityAdminSrvClient.HttpPostAsync<ResultModel<string>>("api/auth/v1/IsSessionIDVaild", request);

            return result.Succeeded;
        }

        public async Task<CheckIsFront> CheckIsFront(UserLoginRequest request)
        {
            var result = await _cityAdminSrvClient.HttpPostAsync<ResultModel<CheckIsFront>>("api/auth/v1/CheckIsFront", request);

            return result.Data;
        }

        public Task<RefreshTokenResponse> RefreshToken(string token, string ipAddress) {
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

        public async Task<UpdMacModel> UpdMacAddress(UserLoginRequest request)
        {
            var result = await _cityAdminSrvClient.HttpPostAsync<ResultModel<UpdMacModel>>("api/auth/v1/UpdMacAddress", request);

            return result.Data;
        }
    }
}