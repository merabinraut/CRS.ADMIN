using CRS.ADMIN.REPOSITORY.NotificationManagement;
using CRS.ADMIN.SHARED.NotificationManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.NotificationManagement
{
    public class NotificationManagementBusiness : INotificationManagementBusiness
    {
        private readonly INotificationManagementRepository _repo;
        public NotificationManagementBusiness()
        {
            _repo = new NotificationManagementRepository();
        }

        public List<NotificationDetailCommon> GetAllNotification(ManageNotificationCommon Request)
        {
            return _repo.GetAllNotification(Request);
        }

        public List<NotificationDetailCommon> GetNotification(string AdminId)
        {
            return _repo.GetNotification(AdminId);
        }
    }
}
