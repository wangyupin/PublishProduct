using CityAdminDomain.Models.API.User;
using CityAdminDomain.Models.Common;
using CityHubCore.Application.Base;
using CityHubCore.Application.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityAdminDomain.Services.User {
    /// <summary>
    /// 處理使用者驗證(Authentication)與授權(Authorization)等資安類事件。
    /// </summary>
    public interface IUserAuth : InterfaceBase {       
        Task<UserBasicInfo> Login(UserLoginRequest model);
        /// <summary>
        /// 若RefreshToken過期，回應前端重新登入。
        /// 此功能只能用於同步處理
        /// </summary>
        /// <param name="token"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        Task<RefreshTokenResponse> RefreshToken(string token, string ipAddress);

        /// 使用時機。
        /// 1.登入時，移除之前取得的。
        /// 2.登出時，移除全部。
        /// </summary>
        /// <param name="token"></param>
        /// <param name="ipAddress"></param>
        void RevokeToken(string token, string ipAddress);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task Logout(UserLogoutRequest request);
        void Register(UserBasicInfo model);
        void ForgotPassword(UserBasicInfo model);
        void ResetPassword(UserBasicInfo model);
        void VerifyEmail(UserBasicInfo model);
        void TwoStepsVerify(UserBasicInfo model);

        Task<bool> ClearSession(SessionUserInfo request);

        Task<bool> IsSessionIDVaild(SessionUserInfo request);

        Task<CheckIsFront> CheckIsFront(UserLoginRequest request);
        Task<UpdMacModel> UpdMacAddress(UserLoginRequest request);

    }
}
