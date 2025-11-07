using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PlanManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.PlanManagement
{
    public interface IPlanManagementBusiness
    {
        List<PlanManagementCommon> GetPlanList(PaginationFilterCommon Request);
        PlanManagementCommon GetPlan(PlanManagementCommon planManagementCommon);
        PlanManagementCommon GetPlanDetail(PlanManagementCommon planManagementCommon);
        CommonDbResponse ManagePlan(PlanManagementCommon planManagementCommon);
        CommonDbResponse EnableDisablePlans(PlanManagementCommon planManagement);
        #region DDL
        List<StaticDataCommon> GetDDL(string StaticType);
        List<PlanRequesResponseListCommon> GetPlanRequestList(PaginationFilterCommon dbRequest);
        CommonDbResponse ApprovePlanRequest(string sno, string type, string planId);
        PlanRequesResponseListCommon GetPlanRequestDetails(string clubId, string planId);
        #endregion
    }
}