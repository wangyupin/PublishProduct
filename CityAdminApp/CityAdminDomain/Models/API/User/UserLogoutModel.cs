using CityHubCore.Infrastructure.API;

namespace CityAdminDomain.Models.API.User {
    public class UserLogoutRequest : RequestBase {
        public string CompanyId { get; set; }
        public string UserId { get; set; }
        public string SID { get; set; }
    }
}