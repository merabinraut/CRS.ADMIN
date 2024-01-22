namespace CRS.ADMIN.APPLICATION.Models.NotificationManagement
{
    public class NotificationDetailModel
    {
        public string NotificationId { get; set; }
        public string NotificationTo { get; set; }
        public string NotificationType { get; set; }
        public string NotificationSubject { get; set; }
        public string NotificationBody { get; set; }
        public string NotificationImageURL { get; set; }
        public string NotificationStatus { get; set; }
        public string NotificationReadStatus { get; set; }
        public string NotificationCount { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string Time { get; set; }
    }
}