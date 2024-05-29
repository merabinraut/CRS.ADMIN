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
    public class ClubPlanManagementBusiness: IClubPlanManagementBusiness
    {
        IClubPlanManagementRepository _REPO;
        public ClubPlanManagementBusiness(ClubPlanManagementRepository REPO)
        {
            _REPO = REPO;
        }
        public List<PlanListCommon> GetClubPlanIdentityList(string culture, string clubid)
        {
            return _REPO.GetClubPlanIdentityList( culture, clubid);
        }
        public CommonDbResponse ManageClubPlan(ManageClubPlan Request)
        {
            return _REPO.ManageClubPlan(Request);
        }
        public List<ClubplanListCommon> GetClubPlanList(string culture, string clubid)
        {
            return _REPO.GetClubPlanList(culture,clubid);
        }
        public List<PlanListCommon> EditClubPlanIdentityList(string culture, string clubid, string planlistid)
        {
            return _REPO.EditClubPlanIdentityList(culture,clubid, planlistid);
        }
        public CommonDbResponse BlockUnblockPlan(ClubplanListCommon objClubplanListCommon = null, string ActionUser = null, string ActionIp = null)
        {
            return _REPO.BlockUnblockPlan(objClubplanListCommon,ActionUser,ActionIp);
        }
    }
}
