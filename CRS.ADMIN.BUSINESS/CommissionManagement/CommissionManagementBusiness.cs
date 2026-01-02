using CRS.ADMIN.REPOSITORY.CommissionManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.CommissionManagement;
using CRS.ADMIN.SHARED.DiscountManagementCommon;
using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.CommissionManagement
{
    public class CommissionManagementBusiness : ICommissionManagementBusiness
    {
        private readonly ICommissionManagementRepository _REPO;
        public CommissionManagementBusiness(CommissionManagementRepository REPO) => _REPO = REPO;

        #region Commission Category
        public List<CommissionAssignedClubsCommon> GetCategoryAssignedList(string CategoryId)
        {
            return _REPO.GetCategoryAssignedList(CategoryId);
        }

        public List<CommissionCategoryCommon> GetCategoryList(string searchText)
        {
            return _REPO.GetCategoryList(searchText);
        }

        public CommissionCategoryCommon GetCategoryById(string categoryId)
        {
            return _REPO.GetCategoryById(categoryId);
        }

        public CommonDbResponse ManageCommissionCategory(ManageCommissionCategoryCommon Request)
        {
            return _REPO.ManageCommissionCategory(Request);
        }

        public CommonDbResponse ManageCommissionStatus(string Status, string CategoryId, Common Request)
        {
            return _REPO.ManageCommissionStatus(Status, CategoryId, Request);
        }
        #endregion

        #region Commission Setup
        public List<CommissionDetailCommon> GetCommissionDetailList(string CategoryId, string AdminCmsTypeId)
        {
            return _REPO.GetCommissionDetailList(CategoryId, AdminCmsTypeId);
        }

        public CommissionDetailCommon GetCommissionDetailById(string CategoryDetailId)
        {
            return _REPO.GetCommissionDetailById(CategoryDetailId);
        }

        public CommonDbResponse ManageCommissionDetail(ManageCommissionDetailCommon Request)
        {
            return _REPO.ManageCommissionDetail(Request);
        }

        public CommonDbResponse DeleteCommissionDetail(string CategoryId, string CategoryDetailId, Common Request)
        {
            return _REPO.DeleteCommissionDetail(CategoryId, CategoryDetailId, Request);
        }
        #endregion

        #region Assign Commission
        public CommonDbResponse AssignCommission(AssignCommissionCommon Request)
        {
            return _REPO.AssignCommission(Request);
        }

        public List<AdminCommissionCommon> GetAdminCommissionList()
        {
            return _REPO.GetAdminCommissionList();
        }
        #endregion

        #region Discount Management
        public List<DiscountManagementCommon> GetDiscountCategoryList(PaginationFilterCommon discountRequest)
        {
            return _REPO.GetDiscountCategoryList(discountRequest);
        }

        public CommonDbResponse ManageDiscountCategory(DiscountManagementRequestCommon mappedRequest)
        {
            return _REPO.ManageDiscountCategory(mappedRequest);
        }

        public DiscountManagementCommon GetDiscountDetails(DiscountManagementRequestCommon common)
        {
            return _REPO.GetDiscountDetails(common);
        }

        public CommonDbResponse ChangeDiscountCategoryStatus(string categoryId,string subCategoryId, string status, string actionUser, string actionIP)
        {
            return _REPO.ChangeDiscountCategoryStatus(categoryId,subCategoryId, status, actionUser, actionIP);
        }

        public List<DiscountCategoryDetailsResponseCommon> GetDiscountSubCategoryList(PaginationFilterCommon discountRequest, string categoryId)
        {
            return _REPO.GetDiscountSubCategoryList(categoryId, discountRequest);
        }

        public CommonDbResponse ManageDiscountSubCategory(DiscountSubCategoryManagementRequestCommon mappedRequest)
        {
            return _REPO.ManageDiscountSubCategory(mappedRequest);
        }

        public DiscountCategoryDetailsResponseCommon GetSubCategoryDiscountDetails(DiscountSubCategoryRequestCommon common)
        {
            return _REPO.GetSubCategoryDiscountDetails(common);
        }

        public AssignDiscountCommon GetSubCategoryDetails(string agentId)
        {
            return _REPO.GetSubCategoryDetails(agentId);
        }

        public CommonDbResponse AssignDiscount(AssignDiscountCommon requestMapped)
        {
            return _REPO.AssignDiscount(requestMapped);
        }
        #endregion
    }
}