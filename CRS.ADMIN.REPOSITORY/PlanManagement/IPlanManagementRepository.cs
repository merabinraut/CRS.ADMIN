using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PlanManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.PlanManagement
{
    public interface IPlanManagementRepository
    {
        List<PlanManagementCommon> GetPlanList(string SearchFilter = "");
        PlanManagementCommon GetPlan(PlanManagementCommon planManagementCommon);
        PlanManagementCommon GetPlanDetail(PlanManagementCommon planManagementCommon);
        CommonDbResponse ManagePlan(PlanManagementCommon planManagementCommon);
        CommonDbResponse EnableDisablePlans(PlanManagementCommon planManagement);
        #region DDL
        List<StaticDataCommon> GetDDL(string StaticType);
        #endregion
    }
}