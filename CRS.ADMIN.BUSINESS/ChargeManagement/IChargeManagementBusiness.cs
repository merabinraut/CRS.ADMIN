using CRS.ADMIN.SHARED.ChargeManagement;
using CRS.ADMIN.SHARED;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.ChargeManagement
{
    public interface IChargeManagementBusiness
    {
        #region Charge Category Management
        CommonDbResponse CreateChargeCategory(ChargeCategoryManagementCommon request);
        CommonDbResponse ManageChargeCategoryStatus(ChargeCategoryStatusManagementCommon request);
        List<ChargeCategoryDetailCommon> GetChargeCategory(string agentType, string freeText);
        #endregion
        #region Charge Management
        CommonDbResponse CreateCharge(ChargeManagementCommon request);
        CommonDbResponse UpdateCharge(ChargeManagementCommon request);
        CommonDbResponse ManageChargeStatus(ChargeStatusManagementCommon request);
        List<ChargeDetailCommon> GetCharge(string categoryId, string categoryDetailId);
        #endregion
    }
}
