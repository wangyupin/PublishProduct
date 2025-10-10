using System;

namespace CityAdminDomain.Models.Common {
    public class UserSessionModel {
        public string Token { get; set; }
        public string LoginIP { get; set; }
        public string Memo { get; set; }
        public DateTime CreateDTM { get; set; }
    }
}
