using CRS.ADMIN.SHARED.NotificationManagement;
using System.Collections.Generic;
using System.Linq;

namespace CRS.ADMIN.REPOSITORY.NotificationManagement
{
    public class NotificationManagementRepository : INotificationManagementRepository
    {
        private readonly RepositoryDao _dao;
        public NotificationManagementRepository()
        {
            _dao = new RepositoryDao();
        }

        public List<NotificationDetailCommon> GetNotification(string AdminId)
        {
            string SQL = "EXEC sproc_admin_notification_management @Flag='s'";
            SQL += ",@AdminId=" + _dao.FilterString(AdminId);
            var dbResponse = _dao.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0) return _dao.DataTableToListObject<NotificationDetailCommon>(dbResponse).ToList();
            return new List<NotificationDetailCommon>();
        }

        public List<NotificationDetailCommon> GetAllNotification(ManageNotificationCommon Request)
        {
            string SQL = "EXEC sproc_admin_notification_management @Flag='gan'";
            SQL += ",@AdminId=" + _dao.FilterString(Request.AdminId);
            SQL += !string.IsNullOrEmpty(Request.NotificationId) ? ",@AdminId=" + _dao.FilterString(Request.AdminId) : "";
            var dbResponse = _dao.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0) 
                return _dao.DataTableToListObject<NotificationDetailCommon>(dbResponse).ToList();
            return new List<NotificationDetailCommon>();
        }
    }
}
