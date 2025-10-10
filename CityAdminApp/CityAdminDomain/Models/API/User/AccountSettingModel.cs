using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityAdminDomain.Models.API.User.AccountSetting
{
    public class GetUserListRequest
    {
        public string company_id { get; set; }
      
    }
    public class GetGroupListRequest
    {
        public string company_id { get; set; }

    }
    public class GetGroupProgramRequest
    {
        public string company_id { get; set; }
    }
    public class GetGroupProgramDetailRequest
    {
        public string GroupName { get; set; }
        public string company_id { get; set; }
    }

    public class User
    {
        public string id { get; set; }
        public string company_id { get; set; }
        public string user_name { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string role { get; set; }
        public string status { get; set; }
        public string updateDTM { get; set; }
        public string createDTM { get; set; }
        public string role_detail { get; set; }
        public string store_id { get; set; }
        public string terminal_id { get; set; }
        // SSO.Identities
        public string method = "PASSWORD";
        /// <summary>
        /// 密碼
        /// </summary>
        public string data { get; set; }
        public string is_enable { get; set; }
        public string Group_Name { get; set; }
        public string Description { get; set; }

    }
    public class UserGroup
    {
        public string Group_Name { get; set; }
        public string description { get; set; }
        public string company_id { get; set; }
    }

    public class AddUserRequest
    {
        /// <summary>
        /// 登入帳號
        /// </summary>
        public string user_name { get; set; }
        /// <summary>
        /// 通常使用公司統編        
        /// </summary>
        public string company_id { get; set; }
        public string name { get; set; }

        private string _password;
        /// <summary>
        /// 不分大小寫
        /// </summary>
        public string Password { get { return _password; } set { _password = value?.Trim();} }

        public string Email { get; set; }
        public string Description { get; set; }
        public string GroupName { get; set; }
        //public string createDTM { get; set; }
        //public string store_id { get; set; }
        //public string terminal_id { get; set; }
    }

    public class DelUserRequest
    {
        public string user_name { get; set; }
        public string company_id { get; set; }
    }

    public class UpdUserRequest
    { /// <summary>
        /// 原始帳號
        /// </summary>
        public string user_name { get; set; }
        /// <summary>
        /// 新帳號
        /// </summary>
        public string new_username { get; set; }
        /// <summary>
        /// 通常使用公司統編        
        /// </summary>
        public string company_id { get; set; }
        public string name { get; set; }
        /// <summary>
        /// 群組權限 對應到 UserGroup表 GroupName欄位
        /// </summary>
        public string role { get; set; }
        public string email { get; set; }
       
        /// <summary>
        /// 不確定何用 先填10
        /// </summary>
        public string status { get; set; }
        public string store_id { get; set; }
        public string terminal_id { get; set; }

        private string _password;
        public string Password { get { return _password; } set { _password = value?.Trim(); } }
        public string is_enable { get; set; }

        private string _groupName;
        public string GroupName { get { return _groupName; } set { _groupName = value?.Trim(); } }
        public string Description { get; set; }
    }

    public class AddUserGroupRequest
    {
        private string _groupName;
        public string GroupName { get { return _groupName; } set { _groupName = value?.Trim(); } }
        /// <summary>
        /// 通常使用公司統編        
        /// </summary>
        public string company_id { get; set; }
        public string Description { get; set; }
        public List<GroupProgramDetail> GroupProgramDetail { get; set; }
        public string ChangePerson { get; set; }
        public string ChangeDate { get; set; }
    }

    public class DelUserGroupRequest
    {
        public string GroupName { get; set; }
        /// <summary>
        /// 通常使用公司統編        
        /// </summary>
        public string company_id { get; set; }
    }

    public class UpdUserGroupRequest
    {
        public string OriginalGroupName { get; set; }
        private string _groupName;
        public string GroupName { get { return _groupName; } set { _groupName = value?.Trim(); } }
        /// <summary>
        /// 通常使用公司統編        
        /// </summary>
        public string company_id { get; set; }
        public string Description { get; set; }
        public List<GroupProgramDetail> GroupProgramDetail { get; set; }
        public string ChangePerson { get; set; }
        public string ChangeDate { get; set; }
    }

    public class GroupProgramDetail
    {
        private string _groupName;
        public string GroupName { get { return _groupName; } set { _groupName = value?.Trim(); } }
        public string Program_ID { get; set; }
        public string Program_Name { get; set; }
        /// <summary>
        /// 通常使用公司統編        
        /// </summary>
        public string company_id { get; set; }
        public string Insert_Flag { get; set; }
        public string View_Flag { get; set; }
        public string Edit_Flag { get; set; }
        public string Delete_Flag { get; set; }
        public string Print_Flag { get; set; }
        public string Money_Flag { get; set; }
        public string Cost_Flag { get; set; }
        public string ChangePerson { get; set; }
        public string ChangeDate { get; set; }
    }
}
