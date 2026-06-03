using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.CommissionManagement;
using CRS.ADMIN.SHARED.DiscountManagementCommon;
using CRS.ADMIN.SHARED.PaginationManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.CommissionManagement
{
    public interface ICommissionManagementRepository
    {
        #region Commission Category
        List<CommissionCategoryCommon> GetCategoryList(string searchText);
        CommissionCategoryCommon GetCategoryById(string categoryId);
        List<CommissionAssignedClubsCommon> GetCategoryAssignedList(string CategoryId);
        CommonDbResponse ManageCommissionCategory(ManageCommissionCategoryCommon Request);
        CommonDbResponse ManageCommissionStatus(string Status, string CategoryId, Common Request);
        #endregion

        #region Commission Setup
        List<CommissionDetailCommon> GetCommissionDetailList(string CategoryId, string AdminCmsTypeId);
        CommissionDetailCommon GetCommissionDetailById(string CategoryDetailId);
        CommonDbResponse ManageCommissionDetail(ManageCommissionDetailCommon Request);
        CommonDbResponse DeleteCommissionDetail(string CategoryId, string CategoryDetailId, Common Request);
        #endregion

        #region Assign Commission
        CommonDbResponse AssignCommission(AssignCommissionCommon Request);
        List<AdminCommissionCommon> GetAdminCommissionList();
        #endregion


        #region Discount management
        List<DiscountManagementCommon> GetDiscountCategoryList(PaginationFilterCommon discountRequest);
        CommonDbResponse ManageDiscountCategory(DiscountManagementRequestCommon mappedRequest);
        DiscountManagementCommon GetDiscountDetails(DiscountManagementRequestCommon common);
        CommonDbResponse ChangeDiscountCategoryStatus(string categoryId,string subCategoryId, string status, string actionUser, string actionIP);
        List<DiscountCategoryDetailsResponseCommon> GetDiscountSubCategoryList(string categoryId, PaginationFilterCommon discountRequest);
        CommonDbResponse ManageDiscountSubCategory(DiscountSubCategoryManagementRequestCommon mappedRequest);
        DiscountCategoryDetailsResponseCommon GetSubCategoryDiscountDetails(DiscountSubCategoryRequestCommon common);
        AssignDiscountCommon GetSubCategoryDetails(string agentId);
        CommonDbResponse AssignDiscount(AssignDiscountCommon requestMapped);
        #endregion
    }
}
