using AutoMapper;
using CityAdminDomain.Models.API.User;
using CityAdminDomain.Models.Common;
using CityAdminDomain.Services.User;
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

namespace StoreWeb.Application.Services
{
    public class UserAuthService : IUserAuth
    {
        private readonly ILogger<UserAuthService> _logger;
        private readonly JwtConfig _jwtConfig;
        private readonly StoreSrvClient _srvClient;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        public string ImplementationName => "UserAuthService";
        public UserAuthService(
            ILogger<UserAuthService> logger
            , IOptions<JwtConfig> jwtConfig
            , StoreSrvClient srvClient
            , IMapper mapper
            , IDistributedCache cache)
        {
            _logger = logger;
            _jwtConfig = jwtConfig.Value;
            _srvClient = srvClient;
            _mapper = mapper;
            _cache = cache;
        }
        public async Task<UserBasicInfo> Login(UserLoginRequest model)
        {
            #region -- check parameters

            #endregion

            #region -- initial variables
            //UserBasicInfo user = result.Data;

            //不做cityAdmin的登入
            UserBasicInfo user = new UserBasicInfo
            {
                CompanyId = model.CompanyId,
                UserId = model.UserId
            };
            JwtUserInfo jwtUserInfo = new()
            {
                CompanyId = user.CompanyId,
                UserId = user.UserId,
                SID = Guid.NewGuid().ToString("N").ToUpper()
            };
            #endregion

            //** biz for JWT
            // authentication successful so generate jwt token
            user.AccessToken = JwtHelper.GenerateJwtToken(_jwtConfig, jwtUserInfo);
            // generate RefreshToken
            user.RefreshToken = JwtHelper.GenerateRefreshToken(_jwtConfig, jwtUserInfo, model.IpAddress);
            //AddRefreshTokenToCache(jwtUserInfo, user.RefreshToken);

            //** biz for Session
            SessionHelper.AddSessionIDToCache(_cache, _jwtConfig, jwtUserInfo);

            #region result

            return user;

            #endregion
        }
        public async Task<RefreshTokenResponse> RefreshToken(string token, string ipAddress)
        {
            #region -- Validate JWT Token
            //Format token
            token = JwtHelper.FormatToken(token);
            string newRefreshToken = null;
            JwtUserInfo jwtUserInfo;

            try
            {
                jwtUserInfo = JwtHelper.ValidateJwtToken(_jwtConfig.Issuer, _jwtConfig.Audience, _jwtConfig.RefreshTokenSecret, token);
            }
            catch (Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException)
            {
                jwtUserInfo = JwtHelper.ValidateJwtToken(_jwtConfig.Issuer, _jwtConfig.Audience, _jwtConfig.RefreshTokenSecret, token, false);

                //newRefreshToken = JwtHelper.GenerateRefreshToken(_jwtConfig, jwtUserInfo, ipAddress);
                //token = newRefreshToken;

                //Check Session from server
                var result = await _srvClient.HttpPostAsync<ResultModel<string>>("api/auth/v1/CheckSessionID", new UserLogoutRequest()
                {
                    CompanyId = jwtUserInfo.CompanyId,
                    UserId = jwtUserInfo.UserId,
                    SID = jwtUserInfo.SID,
                    IpAddress = ipAddress,
                    UserAgent = "RefreshToken"
                });
                if (result.Succeeded)
                {
                    //��sRefreshToken
                    newRefreshToken = JwtHelper.GenerateRefreshToken(_jwtConfig, jwtUserInfo, ipAddress);
                    token = newRefreshToken;
                }
                else
                {
                    // 441 ���b���w�q�䥦�˸m�n�J�C
                    throw new SessionNotAllowException();
                }
            }

            #endregion

            #region -- Check in Redis or not
            //// 1. Read from Redis
            //string cacheKey = $"RefreshToken-{jwtUserInfo.CompanyId}-{jwtUserInfo.UserId}";
            //string cacheData = _cache.GetString(cacheKey);
            ////_ = cacheData ?? throw new JwtRefreshTokenNotFoundException("RefreshToken not in cache");
            //if (cacheData == null) {
            //    newRefreshToken = JwtHelper.GenerateRefreshToken(_jwtConfig, jwtUserInfo, ipAddress);
            //    AddRefreshTokenToCache(jwtUserInfo, newRefreshToken);
            //    token = newRefreshToken;
            //    cacheData = _cache.GetString(cacheKey);
            //}

            //Queue<string> tokenQueue = JsonSerializer.Deserialize<Queue<string>>(cacheData);
            //if (!tokenQueue.Contains(token))
            //    throw new JwtRefreshTokenNotFoundException("RefreshToken not found");
            #endregion

            return new RefreshTokenResponse()
            {
                AccessToken = JwtHelper.GenerateJwtToken(_jwtConfig, jwtUserInfo),
                //RefreshToken = newRefreshToken ?? JwtHelper.GenerateRefreshToken(_jwtConfig, jwtUserInfo, ipAddress)
                RefreshToken = newRefreshToken ?? token
            };
        }
        public async Task Logout(UserLogoutRequest request)
        {
            SessionHelper.DelSessionIDCache(_cache, new JwtUserInfo()
            {
                CompanyId = request.CompanyId,
                UserId = request.UserId,
                SID = request.SID
            });

            await _srvClient.HttpPostAsync<ResultModel<string>>("api/auth/v1/logout", request);
        }
        public async Task<bool> ClearSession(SessionUserInfo request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<bool>>("api/auth/v1/ClearSession", request);

            return result.Succeeded;
        }
        public async Task<bool> IsSessionIDVaild(SessionUserInfo request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<bool>>("api/auth/v1/ClearSession", request);

            return result.Succeeded;
        }
        public void RevokeToken(string token, string ipAddress)
        {
            throw new NotImplementedException();
        }
        public void Register(UserBasicInfo model)
        {
            throw new NotImplementedException();
        }
        public void ForgotPassword(UserBasicInfo model)
        {
            throw new NotImplementedException();
        }
        public void ResetPassword(UserBasicInfo model)
        {
            throw new NotImplementedException();
        }
        public void VerifyEmail(UserBasicInfo model)
        {
            throw new NotImplementedException();
        }
        public void TwoStepsVerify(UserBasicInfo model)
        {
            throw new NotImplementedException();
        }

        public async Task<CheckIsFront> CheckIsFront(UserLoginRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<CheckIsFront>>("api/auth/v1/CheckIsFront", request);

            return result.Data;
        }

        public async Task<UpdMacModel> UpdMacAddress(UserLoginRequest request)
        {
            var result = await _srvClient.HttpPostAsync<ResultModel<UpdMacModel>>("api/auth/v1/UpdMacAddress", request);
            return result.Data;
        }
    }
}