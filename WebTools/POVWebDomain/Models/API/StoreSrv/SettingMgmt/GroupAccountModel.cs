using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POVWebDomain.Models.API.StoreSrv.SettingMgmt.GroupAccount
{
    public class UserResponse
    {
        public string GroupName { get; set; }
        public string Content { get; set; }
        public string Avatar { get; set; }
    }

    public class GroupResponse
    {
        public string GroupName { get; set;}
        public string Description { get; set;}
    }

    public class PermissionResponse
    {
        public int PermissionID { get; set;}
        public int ProgramID { get; set;}
        public string ParentName { get; set;}
        public string ProgramName { get; set;}
        public string PermissionName { get; set;}
        public string Description { get; set;}
    }

    public class User
    {
        public string Size { get; set; } = "sm";
        public string Content { get; set;}
        public string Img { get; set; }
        public User(string content, string avatar)
        {
            Content = content;
            Img = avatar;
        }
    }


    public class Group
    {
        public string GroupName { get; set; }
        public string Description { get; set;}
        public int TotalUsers { get; set; }
        public List<User> Users { get; set; } 

        public Group(string groupName, string description, List<UserResponse> userlist)
        {
            GroupName = groupName;
            Description = description;
            TotalUsers = userlist.Count;
            Users = userlist.Select(user => new User(user.Content, user.Avatar)).ToList();
        }
    }

    public class Permission
    {
        public int PermissionID { get; set; }
        public string PermissionName { get; set; }
        public string Description { get; set; }
        public Permission(int permissionID, string permissionName, string description)
        {
            PermissionID = permissionID;
            PermissionName = permissionName;
            Description = description;
        }
    }

    public class SingleProgram
    {
        public string ParentName { get; set; }
        public int ProgramID { get; set;}
        public string ProgramName { get; set;}
        public List<Permission> Permission { get; set; }  
    }

    public class GetGroupPermissionRequest
    {
        public string GroupName { get; set;}
    }

    public class GetGroupPermissionResponse
    {
        public int PermissionID { get; set;}
        public int ProgramID { get; set;}
    }

    public class AddGroupPermissionRequest
    {
        public string GroupName { get; set;}
        public string Description { get; set;}
        public List<int> Permission { get; set; }
        public string UserID { get; set;}
    }

    public class UpdGroupPermissionRequest
    {
        public string OriginalGroupName { get; set; }
        public string Description { get; set; }
        public string GroupName { get; set; }
        public List<int> Permission { get; set; }
        public string UserID { get; set; }
    }

    public class DelGroupPermissionRequest
    {
        public string GroupName { get; set;}
        public string UserID { get; set; }
    }
}
