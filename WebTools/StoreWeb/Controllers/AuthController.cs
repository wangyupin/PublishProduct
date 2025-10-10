using AutoMapper;
using CityAdminDomain.Models.API.User;
using CityAdminDomain.Models.Common;
using CityAdminDomain.Services.User;
using CityHubCore.Application;
using CityHubCore.Application.Attributies;
using CityHubCore.Application.Jwt;
using CityHubCore.Infrastructure.API;
using CityHubCore.Infrastructure.ServiceClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using POVWebDomain.Models.API.StoreSrv.MachineSet;
using StoreWeb.Application.Models;
using StoreWeb.Application.Services;
using StoreWeb.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;
using CityHubCore.Application.Common;
using CityAdminDomain.Models.DB.CityAdmin;
using POVWebDomain.Models.DB.POVWeb;

namespace StoreWeb.Controllers
{
    public class AuthController : ApiControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUserAuth _userAuthService;
        private readonly IMapper _mapper;
        private readonly AppInfo _appInfo;
        private readonly FrontendSettings _frontendSettings;
        private readonly StoreSrvClient _srvClient;

        public AuthController(ILogger<AuthController> logger, IUserAuth userAuthService, IMapper mapper, IOptions<AppInfo> appInfo,
            IOptions<FrontendSettings> frontendSettings, StoreSrvClient srvClient)
        {
            _logger = logger;
            _userAuthService = userAuthService;
            _mapper = mapper;
            _appInfo = appInfo.Value;
            _frontendSettings = frontendSettings.Value;
            _srvClient = srvClient;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest model)
        {
            var userLoginRequest = _mapper.Map<UserLoginRequest>(model);
            userLoginRequest.IpAddress = RemoteIpAddress;
            userLoginRequest.UserAgent = RemoteUserAgent;
            userLoginRequest.CompanyId = _appInfo.CompanyId;

            UserBasicInfo user = await _userAuthService.Login(_mapper.Map<UserLoginRequest>(userLoginRequest));
            if (user is null)
            {
                return BadRequest(new { message = "帳號錯密碼錯誤!" });
            }


            //Result
            LoginResponse RET = new LoginResponse()
            {
                UserData = _mapper.Map<LoginUserData>(user),
                AccessToken = user.AccessToken,
                RefreshToken = user.RefreshToken,
                LoginDuplication = user.LoginDuplication
            };

            return Ok(new
            {
                RET.UserData,
                RET.AccessToken,
                RET.RefreshToken,
                RET.LoginDuplication,
                machineSet = new { },
            });
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<RefreshTokenResponse>> RefreshToken(RefreshTokenRequest request)
        {

            var RET = await _userAuthService.RefreshToken(request.RefreshToken, RemoteIpAddress);

            if (RET is null)
                return BadRequest(new { message = "RefreshToken not found" });

            return Ok(RET);
        }

        [HttpPost("Logout")]
        public async Task<ActionResult> Logout()
        {
            JwtUserInfo jwtUserInfo = (JwtUserInfo)HttpContext.Items["UserJWTInfo"];

            await _userAuthService.Logout(new()
            {
                CompanyId = jwtUserInfo.CompanyId,
                UserId = jwtUserInfo.UserId,
                SID = jwtUserInfo.SID,
                IpAddress = RemoteIpAddress,
                UserAgent = RemoteUserAgent
            });

            return Ok();
        }

        [HttpPost("CheckIsFront")]
        public async Task<ActionResult> CheckIsFront(LoginRequest model)
        {
            var userLoginRequest = _mapper.Map<UserLoginRequest>(model);
            userLoginRequest.IpAddress = RemoteIpAddress;
            userLoginRequest.UserAgent = RemoteUserAgent;
            userLoginRequest.CompanyId = _appInfo.CompanyId;

            var result = await _userAuthService.CheckIsFront(userLoginRequest);
            return Ok(result);
        }

        [HttpPost("UpdMacAddress")]
        public async Task<ActionResult> UpdMacAddress(LoginRequest model)
        {
            var userLoginRequest = _mapper.Map<UserLoginRequest>(model);
            userLoginRequest.IpAddress = RemoteIpAddress;
            userLoginRequest.UserAgent = RemoteUserAgent;
            userLoginRequest.CompanyId = _appInfo.CompanyId;

            var result = await _userAuthService.UpdMacAddress(userLoginRequest);
            if (result.MacDuplication)
            {
                return BadRequest(new { message = "Mac機器號碼重複!" });
            }
            return Ok();
        }
    }

}
