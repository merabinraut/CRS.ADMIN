using CRS.ADMIN.SHARED.NotificationManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.NotificationManagement
{
    public interface INotificationManagementBusiness
    {
        List<NotificationDetailCommon> GetNotification(string AdminId);
        List<NotificationDetailCommon> GetAllNotification(ManageNotificationCommon Request);
    }
}
