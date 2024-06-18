using CRS.ADMIN.APPLICATION.Resources;
using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.APPLICATION.Models.UserProfileManagement
{
    public class UserProfileModel
    {
        public string UpdatedDate { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string RoleType { get; set; }
        public string RoleName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Full_name_is_required")]
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string ProfileImage { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Password_is_required")]
        [Display(Name = "Current Password")]
        [MaxLength(16, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Password_length_should_not_exceed_16_digit"), MinLength(8, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Password_minimum_length_must_be_8")]
        public string OldPassword { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "New_password_is_required")]
        [Display(Name = "New Password")]
        [MaxLength(16, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Password_length_should_not_exceed_16_digit"), MinLength(8, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Password_minimum_length_must_be_8")]
        //[RegularExpression(@"^.*(?=.{8,16})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=]).*$", ErrorMessage = "Must be 8 to 16 Length and must contain a-z,A-Z,0-9,@#$%^&+=")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm Password")]
        [MaxLength(16, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Password_length_should_not_exceed_16_digit"), MinLength(8, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Password_minimum_length_must_be_8")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Confirm_password_is_required")]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Password_Mismatch")]
        public string ConfirmPassword { get; set; }
    }

}