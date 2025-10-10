using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using POVWebDomain.Models.API.StoreSrv.Common;

namespace POVWebDomain.Models.API.StoreSrv.SettingMgmt.UserAccount
{
    public class UserDataLength
    {
        public int Length { get; set; }
    }

    public class OptionWA : Option<string>
    {
        public string Avatar { get; set; }
        public OptionWA(string value, string label,string avatar):base(value,label)
        {
            this.Avatar = avatar;
        }
    }

    public class UserData
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string UserNumberStr { get; set; }
        public OptionWA UserNumber { get; set; }
        public string Password { get; set; }
        public string Description { get; set; }
        public string GroupNameStr { get; set; }
        public List<Option<string>> GroupName { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public List<Option<string>> ClientPermission { get; set; }
        public bool CostPermission { get; set; }
    }

    public class UserDataDB
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string UserNumber { get; set; }
        public string Password { get; set; }
        public string Description { get; set; }
        public string GroupName { get; set; }
        public string ClientPermission { get; set; }
        public bool CostPermission { get; set; }
    }

    public class GetDataRequest
    {
        public string Q { get; set; }
        public int Page { get; set; }
        public int PerPage { get; set; }
        public string Sort { get; set; }
        public string SortColumn { get; set; }
        public string Mode { get; set; }
        public AdvanceSearchRequest AdvanceRequest { get; set; }

    }
    public class GetSingleDataRequest
    {
        public string UserID { get; set; }
    }

    public class AddUsersDataRequest
    {
        private string _userID;
        public string UserID { get { return _userID; } set { _userID = value?.Trim(); } }
        public string UserNumber { get; set; }
        private string _password;
        public string Password { get { return _password; } set { _password = value?.Trim(); } }
        public string Description { get; set; }
        public string GroupName { get; set; }
        public bool CostPermission { get; set; }
        public string ClientPermission { get; set; }
    }

    public class ID
    {
        public string UserID { get; set; }
    }
    public class DelUsersDataRequest
    {
        public List<ID> DelList { get; set; }
    }

    public class UpdUsersDataRequest
    {
        public string OriginalUserID { get; set; }
        private string _userID;
        public string UserID { get { return _userID; } set { _userID = value?.Trim(); } }
        public string UserNumber { get; set; }
        private string _password;
        public string Password { get { return _password; } set { _password = value?.Trim(); } }
        public string Description { get; set; }
        public string GroupName { get; set; }
        public bool CostPermission { get; set; }
        public string ClientPermission { get; set; }
    }
}
