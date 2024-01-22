using CRS.ADMIN.SHARED.NotificationManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.NotificationManagement
{
    public interface INotificationManagementRepository
    {
        List<NotificationDetailCommon> GetNotification(string AdminId);
        List<NotificationDetailCommon> GetAllNotification(ManageNotificationCommon Request);
    }
}
