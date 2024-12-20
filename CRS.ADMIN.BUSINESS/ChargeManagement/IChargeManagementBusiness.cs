using CRS.ADMIN.SHARED.ChargeManagement;
using CRS.ADMIN.SHARED;
using System.Collections.Generic;
using CRS.ADMIN.SHARED.PaginationManagement;

namespace CRS.ADMIN.BUSINESS.ChargeManagement
{
    public interface IChargeManagementBusiness
    {
        #region Charge Category Management
        CommonDbResponse CreateChargeCategory(ChargeCategoryManagementCommon request);
        CommonDbResponse ManageChargeCategoryStatus(ChargeCategoryStatusManagementCommon request);
        List<ChargeCategoryDetailCommon> GetChargeCategory(string agentTypestring,string categoryId,PaginationFilterCommon dbRequest );
        ChargeCategoryDetailCommon GetChargeCategoryDetails(string agentTypestring, string categoryId, PaginationFilterCommon dbRequest);
        #endregion
        #region Charge Management
        CommonDbResponse CreateCharge(ChargeManagementCommon request);
        //CommonDbResponse UpdateCharge(ChargeManagementCommon request);
        CommonDbResponse ManageChargeStatus(ChargeStatusManagementCommon request);
        List<ChargeDetailCommon> GetCharge(string categoryId, string categoryDetailId, PaginationFilterCommon dbRequest);
        ChargeDetailCommon GetChargeDetails(string categoryId, string categoryDetailId);
        #endregion
        #region Assign Category
        CommonDbResponse GetCurrentCategory(string agentTypeValue, string agentType);
        CommonDbResponse AssignCategory(string agentTypeValue, string agentType, string categoryId);
        #endregion
    }
}
