using AutoMapper;
using CityAdminDomain.Models.API.User;
using CityAdminDomain.Models.Common;
using CityAdminDomain.Services.User;
using CityHubCore.Application;
using CityHubCore.Application.Attributies;
using CityHubCore.Application.Jwt;
using CityHubCore.Application.Session;
using CityHubCore.Infrastructure.API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StoreSrv.Application.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSrv.Controllers {
    public class AuthController : ApiControllerBase {
        private readonly ILogger<AuthController> _logger;
        private readonly IUserAuth _userAuthService;
        private readonly IMapper _mapper;
        private readonly AppInfo _appInfo;

        public AuthController(ILogger<AuthController> logger,
            IEnumerable<IUserAuth> userAuthList,
            IMapper mapper, IOptions<AppInfo> appInfo) {
            _logger = logger;
            _mapper = mapper;
            _appInfo = appInfo.Value;

            //依據連線狀況，採用對應的實作
            _userAuthService = IsHqOnlne()
                ? userAuthList.SingleOrDefault(m => m.ImplementationName == "UserAuthOnlineService")
                : userAuthList.SingleOrDefault(m => m.ImplementationName == "UserAuthOfflineService");
        }

        /// <summary>
        /// Todo: 檢查目前連線狀況
        /// </summary>
        /// <returns></returns>
        private bool IsHqOnlne() {
            return true;
        }

        [HttpPost("V1/Login")]
        public async Task<ResultModel<UserBasicInfo>> Login(UserLoginRequest request) {

            UserBasicInfo user = await _userAuthService.Login(request);

            if (user is not null) {
                return FormatResultModel<UserBasicInfo>.Success(user);
            } else {
                return FormatResultModel<UserBasicInfo>.Failure(user);
            }
        }

        [HttpPost("V1/Logout")]
        public async Task Logout(UserLogoutRequest request) {
         
            await _userAuthService.Logout(request);

        }

        [HttpPost("V1/ClearSession")]
        public async Task<ResultModel<string>> ClearSession(SessionUserInfo request) {

            return new ResultModel<string>() { Succeeded = await _userAuthService.ClearSession(request) };
        }

        [HttpPost("V1/IsSessionIDVaild")]
        public async Task<ResultModel<string>> IsSessionIDVaild(SessionUserInfo request) {

            return new ResultModel<string>() { Succeeded = await _userAuthService.IsSessionIDVaild(request) };
        }

        [HttpPost("V1/CheckIsFront")]
        public async Task<ResultModel<CheckIsFront>> CheckIsFront(UserLoginRequest request)
        {

            CheckIsFront result = await _userAuthService.CheckIsFront(request);

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
            var result = await _userAuthService.UpdMacAddress(request);
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
