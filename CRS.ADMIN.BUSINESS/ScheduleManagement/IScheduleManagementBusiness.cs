using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ScheduleManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.ScheduleManagement
{
    public interface IScheduleManagementBusiness
    {
        CommonDbResponse ManageSchedule(ManageScheduleCommon Request);
        List<ClubScheduleCommon> GetClubSchedule(string ClubId);
    }
}
