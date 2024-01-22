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
        [Required(AllowEmptyStrings = false, ErrorMessage = "Full name is required")]
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string ProfileImage { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [Display(Name = "Current Password")]
        [MaxLength(16, ErrorMessage = "Password length should not exceed 16 digit"), MinLength(8, ErrorMessage = "Password minimum length must be 8")]
        public string OldPassword { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "New password is required")]
        [Display(Name = "New Password")]
        [MaxLength(16, ErrorMessage = "Password length should not exceed 16 digit"), MinLength(8, ErrorMessage = "Password minimum length must be 8")]
        //[RegularExpression(@"^.*(?=.{8,16})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=]).*$", ErrorMessage = "Must be 8 to 16 Length and must contain a-z,A-Z,0-9,@#$%^&+=")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm Password")]
        [MaxLength(16, ErrorMessage = "Password length should not exceed 16 digit"), MinLength(8, ErrorMessage = "Password minimum length must be 8")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "Password  Mismatch")]
        public string ConfirmPassword { get; set; }
    }

}