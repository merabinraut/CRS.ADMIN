using CRS.ADMIN.REPOSITORY.ScheduleManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ScheduleManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.ScheduleManagement
{
    public class ScheduleManagementBusiness : IScheduleManagementBusiness
    {
        private readonly IScheduleManagementRepository _repo;
        public ScheduleManagementBusiness(ScheduleManagementRepository repo)
        {
            _repo = repo;
        }

        public List<ClubScheduleCommon> GetClubSchedule(string ClubId)
        {
            return _repo.GetClubSchedule(ClubId);
        }

        public CommonDbResponse ManageSchedule(ManageScheduleCommon Request)
        {
            return _repo.ManageSchedule(Request);
        }
    }
}
