using CityHubCore.Infrastructure.API;

namespace CityAdminDomain.Models.API.User {
    public class UserLoginRequest : RequestBase {
        public string CompanyId { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// �j��n�J
        /// </summary>
        public bool Force { get; set; }
        public string MacAddress {  get; set; } //���X
    }

    public class UserLoginWithModeRequest : UserLoginRequest {
        public string UserName { get; set; }
        public string Token { get; set; }
    }

    public class CheckIsFront
    {
        public bool IsFront { get; set; }
    }
}