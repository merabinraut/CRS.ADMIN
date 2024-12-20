using CRS.ADMIN.REPOSITORY.ChargeManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ChargeManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.ChargeManagement
{
    public class ChargeManagementBusiness : IChargeManagementBusiness
    {
        public readonly IChargeManagementRepository _repo;
        public ChargeManagementBusiness(ChargeManagementRepository repo)
        {
            _repo = repo;
        }
        #region Charge Category Management
        public CommonDbResponse CreateChargeCategory(ChargeCategoryManagementCommon request)
        {
            return _repo.CreateChargeCategory(request);
        }
        public List<ChargeCategoryDetailCommon> GetChargeCategory(string  agentType , string categoryId, PaginationFilterCommon dbRequest )
        {
            return _repo.GetChargeCategory(agentType, categoryId, dbRequest);
        }
        public ChargeCategoryDetailCommon GetChargeCategoryDetails(string agentTypestring, string categoryId, PaginationFilterCommon dbRequest)
        {
            return _repo.GetChargeCategoryDetails(agentTypestring, categoryId, dbRequest);
        }
        public CommonDbResponse ManageChargeCategoryStatus(ChargeCategoryStatusManagementCommon request)
        {
            return _repo.ManageChargeCategoryStatus(request);
        }
        #endregion
        #region Charge Management
        public CommonDbResponse CreateCharge(ChargeManagementCommon request)
        {
            return _repo.CreateCharge(request);
        }
        public List<ChargeDetailCommon> GetCharge(string categoryId, string categoryDetailId, PaginationFilterCommon dbRequest)
        {
            return _repo.GetCharge(categoryId, categoryDetailId,dbRequest);
        }
        public CommonDbResponse ManageChargeStatus(ChargeStatusManagementCommon request)
        {
            return _repo.ManageChargeStatus(request);
        }
        //public CommonDbResponse UpdateCharge(ChargeManagementCommon request)
        //{
        //    return _repo.UpdateCharge(request);
        //}
        public ChargeDetailCommon GetChargeDetails(string categoryId, string categoryDetailId)
        {
            return _repo.GetChargeDetails(categoryId, categoryDetailId);
        }
        #endregion
        #region Assign Category
        public CommonDbResponse GetCurrentCategory(string agentTypeValue, string agentType)
        {
            return _repo.GetCurrentCategory(agentTypeValue, agentType);
        }
        public CommonDbResponse AssignCategory(string agentTypeValue, string agentType, string categoryId)
        {
            return _repo.AssignCategory(agentTypeValue, agentType, categoryId);
        }
        #endregion
    }
}
