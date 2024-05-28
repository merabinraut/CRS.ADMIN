using CRS.ADMIN.REPOSITORY.ClubManagement;
using CRS.ADMIN.REPOSITORY.ClubPlanManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ClubManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.BUSINESS.ClubPlanManagement
{
    public interface IClubPlanManagementBusiness
    {
        List<PlanListCommon> GetClubPlanIdentityList(string culture, string clubid);
        CommonDbResponse ManageClubPlan(ManageClubPlan Request);
        List<ClubplanListCommon> GetClubPlanList(string culture, string clubid);
        List<PlanListCommon> EditClubPlanIdentityList(string culture, string clubid, string planlistid);
        CommonDbResponse BlockUnblockPlan(ClubplanListCommon objClubplanListCommon = null, string ActionUser = null, string ActionIp = null);
    }
}
