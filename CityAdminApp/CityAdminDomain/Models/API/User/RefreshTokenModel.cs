using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityAdminDomain.Models.API.User {
    public class RefreshTokenRequest {
        public string RefreshToken { get; set; }
    }

    public class RefreshTokenResponse {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
