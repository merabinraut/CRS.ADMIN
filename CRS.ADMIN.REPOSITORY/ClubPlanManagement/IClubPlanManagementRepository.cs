using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ClubManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.REPOSITORY.ClubPlanManagement
{
    public interface IClubPlanManagementRepository
    {
        List<PlanListCommon> GetClubPlanIdentityList(string culture, string clubid);
        CommonDbResponse ManageClubPlan(ManageClubPlan Request);
        List<ClubplanListCommon> GetClubPlanList(string culture, string clubid);
    }
}
