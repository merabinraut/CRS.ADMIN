namespace CRS.ADMIN.APPLICATION.Models.ErrorLog
{
    public class ErrorLogModel
    {
        public string ErrorId { get; set; }
        public string ErrorDescription { get; set; }
        public string ErrorCategory { get; set; }
        public string ErrorSource { get; set; }
        public string ActionDate { get; set; }
    }
}