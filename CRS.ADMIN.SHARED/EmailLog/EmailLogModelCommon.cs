namespace CRS.ADMIN.SHARED.EmailLog
{
    public class EmailLogModelCommon
    {
        public string EmailRequestId { get; set; }
        public string EmailText { get; set; }
        public string EmailSendBy { get; set; }
        public string EmailSendTo { get; set; }
        public string EmailSendToCC { get; set; }
        public string EmailSendStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
    }
}
