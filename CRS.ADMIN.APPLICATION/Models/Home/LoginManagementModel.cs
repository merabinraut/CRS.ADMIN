using CRS.ADMIN.APPLICATION.Models.NotificationManagement;
using CRS.ADMIN.APPLICATION.Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.APPLICATION.Models.Home
{
    public class LoginRequestModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Display(Name = "UserName")]
        [MaxLength(30, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Maximum_length_is_30_characters")]
        public string Username { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Display(Name = "Password")]
        [MaxLength(16, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Maximum_length_is_16_characters")]
        public string Password { get; set; }
        public string SessionId { get; set; }
    }
    public class LoginResponseModel
    {
        public string RoleId { get; set; }
        public string UserId { get; set; }
        public string RoleName { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string ProfileImage { get; set; }
        public string IsPasswordForceful { get; set; }
        public string LastPasswordChangedDate { get; set; }
        public List<MenuModel> Menus { get; set; }
        public List<string> Functions { get; set; }
        public List<NotificationDetailModel> Notifications { get; set; } = new List<NotificationDetailModel>();
    }

    public class MenuModel
    {
        public string MenuName { get; set; }
        public string MenuUrl { get; set; }
        public string MenuGroup { get; set; }
        public string ParentGroup { get; set; }
        public string CssClass { get; set; }
        public string GroupOrderPosition { get; set; }
        public string MenuOrderPosition { get; set; }
    }
}