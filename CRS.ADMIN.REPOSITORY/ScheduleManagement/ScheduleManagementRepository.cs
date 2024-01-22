using CRS.ADMIN.SHARED.ScheduleManagement;
using CRS.ADMIN.SHARED;
using System.Collections.Generic;
using System.Linq;

namespace CRS.ADMIN.REPOSITORY.ScheduleManagement
{
    public class ScheduleManagementRepository : IScheduleManagementRepository
    {
        private readonly RepositoryDao _dao;
        public ScheduleManagementRepository()
        {
            _dao = new RepositoryDao();
        }
        public CommonDbResponse ManageSchedule(ManageScheduleCommon Request)
        {
            var SQL = "EXEC sproc_club_schedule_management ";
            if (!string.IsNullOrEmpty(Request.ScheduleId))
            {
                SQL += "@Flag = 'mcs'";
                SQL += ",@ScheduleId=" + _dao.FilterString(Request.ScheduleId);
            }
            else SQL += "@Flag = 'ccs'";
            SQL += ",@ClubId=" + _dao.FilterString(Request.ClubId);
            SQL += ",@DateValue=" + _dao.FilterString(Request.DateValue);
            SQL += ",@ClubSchedule=" + _dao.FilterString(Request.ClubSchedule);
            SQL += ",@ActionUser=" + _dao.FilterString(Request.ActionUser);
            SQL += ",@ActionIP=" + _dao.FilterString(Request.ActionIP);
            SQL += ",@ActionPlatform=" + _dao.FilterString(Request.ActionPlatform);
            return _dao.ParseCommonDbResponse(SQL);
        }

        public List<ClubScheduleCommon> GetClubSchedule(string ClubId)
        {
            var SQL = "EXEC sproc_club_schedule_management @Flag = 'gcsl'";
            SQL += ",@ClubId=" + _dao.FilterString(ClubId);
            var dbResponse = _dao.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0) return _dao.DataTableToListObject<ClubScheduleCommon>(dbResponse).ToList();
            return new List<ClubScheduleCommon>();
        }
    }
}
