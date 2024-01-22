namespace CRS.ADMIN.SHARED.ProfileManagement
{
    public class UserProfileCommon : Common
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string RoleName { get; set; }
        public string RoleType { get; set; }
        public string UserId { get; set; }
        public string Session { get; set; }
        public string ProfileImage { get; set; }
        public string AdminLogoPath { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
    }
}