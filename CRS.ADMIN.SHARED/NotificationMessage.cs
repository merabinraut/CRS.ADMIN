namespace CRS.ADMIN.SHARED
{
    public enum NotificationMessage
    {
        SUCCESS = 0,
        ERROR = 1,
        INFORMATION = 2,
        WARNING = 3,
    }

    public class NotificationModel
    {
        public string Message { get; set; }
        public string Title { get; set; }
        public NotificationMessage NotificationType { get; set; }
    }
}
