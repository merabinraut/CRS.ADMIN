using CRS.ADMIN.REPOSITORY.PlanManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PlanManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.PlanManagement
{
    public class PlanManagementBusiness : IPlanManagementBusiness
    {
        private readonly IPlanManagementRepository _repository;

        public PlanManagementBusiness(PlanManagementRepository repository) => this._repository = repository;

        public CommonDbResponse EnableDisablePlans(PlanManagementCommon planManagement)
        {
            return _repository.EnableDisablePlans(planManagement);
        }

        public PlanManagementCommon GetPlan(PlanManagementCommon planManagementCommon)
        {
            return _repository.GetPlan(planManagementCommon);
        }

        public PlanManagementCommon GetPlanDetail(PlanManagementCommon planManagementCommon)
        {
            return _repository.GetPlanDetail(planManagementCommon);
        }

        public List<PlanManagementCommon> GetPlanList(PaginationFilterCommon Request)
        {
            return _repository.GetPlanList(Request);
        }

        public CommonDbResponse ManagePlan(PlanManagementCommon planManagementCommon)
        {
            return _repository.ManagePlan(planManagementCommon);
        }
        #region DDL
        public List<StaticDataCommon> GetDDL(string StaticType)
        {
            return _repository.GetDDL(StaticType);
        }
        #endregion
    }
}