using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ScheduleManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.ScheduleManagement
{
    public interface IScheduleManagementRepository
    {
        CommonDbResponse ManageSchedule(ManageScheduleCommon Request);
        List<ClubScheduleCommon> GetClubSchedule(string ClubId);
    }
}
