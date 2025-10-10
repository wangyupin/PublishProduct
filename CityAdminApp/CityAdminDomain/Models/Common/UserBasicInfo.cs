using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CityAdminDomain.Models.Common {
    public class UserBasicInfo {
        //Require
        public long Id { get; set; }
        public string CompanyId { get; set; }
        public string UserId { get; set; }
        public string StoreId { get; set; }
        public string TerminalID { get; set; }

        //Extension
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }

        //Role Control
        public string Role { get; set; }
        public string Role_Detail { get; set; }

        public List<UserACLModel> Ability { get; set; }

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public bool LoginDuplication { get; set; }
        public List<UserSessionModel> SessionList { get; set; }
        public bool? IsFront {  get; set; }

        public static implicit operator Task<object>(UserBasicInfo v) {
            throw new NotImplementedException();
        }
    }
}