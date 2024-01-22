namespace CRS.ADMIN.SHARED.ProfileManagement
{
    public class PasswordCommon
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string UserId { get; set; }
        public string Session { get; set; }
        public string IPAddress { get; set; }
        public string BrowserInfo { get; set; }
        public string ActionUser { get; set; }
    }
}