using AutoMapper;
using CityAdminDomain.Models.API.User;
using CityAdminDomain.Models.Common;
using CityAdminDomain.Services.User;
using CityHubCore.Application;
using CityHubCore.Application.Attributies;
using CityHubCore.Application.Jwt;
using CityHubCore.Infrastructure.API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using HqSrv.Application.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityHubCore.Infrastructure.ServiceClient;
using POVWebDomain.Models.DB.POVWeb;
using CityHubCore.Application.Session;

namespace HqSrv.Controllers
{
    public class AuthController : ApiControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUserAuth _userAuthCityAdminService;
        private readonly IUserAuth _userAuthPOVWebService;

        public AuthController(
            ILogger<AuthController> logger,
            IEnumerable<IUserAuth> userAuthList)
        {
            _logger = logger;
            _userAuthCityAdminService = userAuthList.SingleOrDefault(m => m.ImplementationName == "UserAuthCityAdminService");
            _userAuthPOVWebService = userAuthList.SingleOrDefault(m => m.ImplementationName == "UserAuthPOVWebService");
        }

        [HttpPost("V1/Login")]
        public async Task<ResultModel<UserBasicInfo>> Login(UserLoginRequest request)
        {

            //執行CityAdminService Login Flow
            UserBasicInfo user = await _userAuthCityAdminService.Login(request);

            if (user is null)
            {
                return FormatResultModel<UserBasicInfo>.Failure(user);
            }

            //POVWeb Login Flow
            await _userAuthPOVWebService.Login(request);

            return FormatResultModel<UserBasicInfo>.Success(user);
        }

        [HttpPost("V1/Logout")]
        public async Task Logout(UserLogoutRequest request)
        {

            await _userAuthCityAdminService.Logout(request);

            await _userAuthPOVWebService.Logout(request);

        }

        [HttpPost("V1/ClearSession")]
        public async Task<ResultModel<string>> ClearSession(SessionUserInfo request)
        {

            return new ResultModel<string>() { Succeeded = await _userAuthCityAdminService.ClearSession(request) };
        }

        [HttpPost("V1/IsSessionIDVaild")]
        public async Task<ResultModel<string>> IsSessionIDVaild(SessionUserInfo request)
        {

            return new ResultModel<string>() { Succeeded = await _userAuthCityAdminService.IsSessionIDVaild(request) };
        }

        [HttpPost("V1/CheckIsFront")]
        public async Task<ResultModel<CheckIsFront>> CheckIsFront(UserLoginRequest request)
        {

            //執行CityAdminService Login Flow
            var result = await _userAuthCityAdminService.CheckIsFront(request);

            if (result is not null)
            {
                return FormatResultModel<CheckIsFront>.Success(result);
            }
            else
            {
                return FormatResultModel<CheckIsFront>.Failure(result);
            }
        }

        [HttpPost("V1/UpdMacAddress")]
        public async Task<ResultModel<UpdMacModel>> UpdMacAddress(UserLoginRequest request)
        {

            var result = await _userAuthCityAdminService.UpdMacAddress(request);
            if (result is not null)
            {
                return FormatResultModel<UpdMacModel>.Success(result);
            }
            else
            {
                return FormatResultModel<UpdMacModel>.Failure(result);
            }


        }

    }

}
