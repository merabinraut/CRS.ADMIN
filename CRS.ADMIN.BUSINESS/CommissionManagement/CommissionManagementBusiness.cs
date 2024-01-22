using CRS.ADMIN.REPOSITORY.CommissionManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.CommissionManagement;
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

        public List<CommissionCategoryCommon> GetCategoryList()
        {
            return _REPO.GetCategoryList();
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
        public List<CommissionDetailCommon> GetCommissionDetailList(string CategoryId)
        {
            return _REPO.GetCommissionDetailList(CategoryId);
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

        #endregion
    }
}