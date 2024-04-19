using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.CommissionManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.CommissionManagement
{
    public interface ICommissionManagementBusiness
    {
        #region Commission Category
        List<CommissionCategoryCommon> GetCategoryList();
        CommissionCategoryCommon GetCategoryById(string categoryId);
        List<CommissionAssignedClubsCommon> GetCategoryAssignedList(string CategoryId);
        CommonDbResponse ManageCommissionCategory(ManageCommissionCategoryCommon Request);
        CommonDbResponse ManageCommissionStatus(string Status, string CategoryId, Common Request);
        #endregion

        #region Commission Setup
        List<CommissionDetailCommon> GetCommissionDetailList(string CategoryId,string AdminCmsTypeId);
        CommissionDetailCommon GetCommissionDetailById(string CategoryDetailId);
        CommonDbResponse ManageCommissionDetail(ManageCommissionDetailCommon Request);
        CommonDbResponse DeleteCommissionDetail(string CategoryId, string CategoryDetailId, Common Request);
        #endregion

        #region Assign Commission
        CommonDbResponse AssignCommission(AssignCommissionCommon Request);
        List<AdminCommissionCommon> GetAdminCommissionList();
        #endregion
    }
}
