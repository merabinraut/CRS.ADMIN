using CRS.ADMIN.REPOSITORY.ChargeManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ChargeManagement;
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
        public List<ChargeCategoryDetailCommon> GetChargeCategory(string agentType, string freeText)
        {
            return _repo.GetChargeCategory(agentType, freeText);
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
        public List<ChargeDetailCommon> GetCharge(string categoryId, string categoryDetailId)
        {
            return _repo.GetCharge(categoryId, categoryDetailId);
        }
        public CommonDbResponse ManageChargeStatus(ChargeStatusManagementCommon request)
        {
            return _repo.ManageChargeStatus(request);
        }
        public CommonDbResponse UpdateCharge(ChargeManagementCommon request)
        {
            return _repo.UpdateCharge(request);
        }
        #endregion
    }
}
