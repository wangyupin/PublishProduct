using CityAdminDomain.Models.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoreWeb.ViewModels.User
{
    public class LoginRequest
    {
        [Required]
        public string CompanyId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public bool Force { get; set; }
        public string MacAddress { get; set; } //¾÷½X


    }

    public class LoginResponse
    {
        public LoginUserData UserData { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public bool LoginDuplication { get; set; }
    }

    public class LoginUserData
    {
        //Require
        public int Id { get; set; }
        public string CompanyId { get; set; }
        public string StoreId { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }

        //Extension
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }

        //Role Control
        public string Role { get; set; }
        public List<UserACLModel> Ability { get; set; }
    }
}