using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ChargeManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.ChargeManagement
{
    public interface IChargeManagementRepository
    {
        #region Charge Category Management
        CommonDbResponse CreateChargeCategory(ChargeCategoryManagementCommon request);
        CommonDbResponse ManageChargeCategoryStatus(ChargeCategoryStatusManagementCommon request);
        List<ChargeCategoryDetailCommon> GetChargeCategory(string agentType ,string categoryId, PaginationFilterCommon dbRequest );
        ChargeCategoryDetailCommon GetChargeCategoryDetails(string agentTypestring, string categoryId, PaginationFilterCommon dbRequest);
        #endregion
        #region Charge Management
        CommonDbResponse CreateCharge(ChargeManagementCommon request);
        CommonDbResponse UpdateCharge(ChargeManagementCommon request);
        CommonDbResponse ManageChargeStatus(ChargeStatusManagementCommon request);
        List<ChargeDetailCommon> GetCharge(string categoryId, string categoryDetailId);
        #endregion
    }
}
