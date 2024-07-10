using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.CommissionManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CRS.ADMIN.REPOSITORY.CommissionManagement
{
    public class CommissionManagementRepository : ICommissionManagementRepository
    {
        private readonly RepositoryDao _DAO;
        public CommissionManagementRepository() => _DAO = new RepositoryDao();

        #region Commission Category
        public List<CommissionCategoryCommon> GetCategoryList()
        {
            var response = new List<CommissionCategoryCommon>();
            string SQL = "EXEC sproc_commission_category_management @Flag='gccl'";
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new CommissionCategoryCommon()
                    {
                        CategoryId = _DAO.ParseColumnValue(item, "CategoryId").ToString(),
                        CategoryName = _DAO.ParseColumnValue(item, "CategoryName").ToString(),
                        Status = _DAO.ParseColumnValue(item, "Status").ToString(),
                        CreatedDate = !string.IsNullOrEmpty(_DAO.ParseColumnValue(item, "CreatedDate").ToString()) ? DateTime.Parse(_DAO.ParseColumnValue(item, "CreatedDate").ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : _DAO.ParseColumnValue(item, "CreatedDate").ToString(),
                        CreatedByFullName = _DAO.ParseColumnValue(item, "CreatedByFullname").ToString(),
                        CreatedByUsername = _DAO.ParseColumnValue(item, "CreatedByUsername").ToString(),
                        CreatedByImage = _DAO.ParseColumnValue(item, "CreatedByImage").ToString(),
                    });
                }
            }
            return response;
        }


        public CommissionCategoryCommon GetCategoryById(string categoryId)
        {
            var SQL = "sproc_commission_category_management @Flag='gccvid'";
            SQL += ",@CategoryId=" + _DAO.FilterString(categoryId);

            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0)
            {
                return new CommissionCategoryCommon()
                {
                    CategoryId = dbResponse.Rows[0]["CategoryId"]?.ToString(),
                    CategoryName = dbResponse.Rows[0]["CategoryName"]?.ToString(),
                    Description = dbResponse.Rows[0]["Description"]?.ToString(),
                    Status = dbResponse.Rows[0]["Status"]?.ToString(),
                    CreatedDate =  dbResponse.Rows[0]["CreatedDate"]?.ToString(),
                    CreatedByFullName = dbResponse.Rows[0]["CreatedByFullname"]?.ToString(),
                    CreatedByUsername = dbResponse.Rows[0]["CreatedByUsername"]?.ToString(),
                    CreatedByImage = dbResponse.Rows[0]["CreatedByImage"]?.ToString(),
                };
            }
            return new CommissionCategoryCommon();
        }

        public List<CommissionAssignedClubsCommon> GetCategoryAssignedList(string CategoryId)
        {
            var response = new List<CommissionAssignedClubsCommon>();
            string SQL = "EXEC sproc_commission_category_management @Flag='gcac'";
            SQL += ",@CategoryId=" + _DAO.FilterString(CategoryId);
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new CommissionAssignedClubsCommon()
                    {
                        ClubName = _DAO.ParseColumnValue(item, "ClubName").ToString(),
                        Logo = _DAO.ParseColumnValue(item, "Logo").ToString(),
                        Status = _DAO.ParseColumnValue(item, "Status").ToString(),
                        EmailAddress = _DAO.ParseColumnValue(item, "EmailAddress").ToString(),
                        MobileNumber = _DAO.ParseColumnValue(item, "MobileNumber").ToString(),
                        CreatedDate = _DAO.ParseColumnValue(item, "CreatedDate").ToString(),
                        UpdatedDate = _DAO.ParseColumnValue(item, "UpdatedDate").ToString()
                    });
                }
            }
            return response;
        }

        public CommonDbResponse ManageCommissionCategory(ManageCommissionCategoryCommon Request)
        {
            var SQL = "EXEC sproc_commission_category_management ";
            SQL += !string.IsNullOrEmpty(Request.CategoryId) ? "@Flag='ucc'" : "@Flag='icc'";
            SQL += ",@CategoryName=N" + _DAO.FilterString(Request.CategoryName);
            SQL += ",@Description=N" + _DAO.FilterString(Request.Description);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            if (!string.IsNullOrEmpty(Request.CategoryId))
                SQL += ",@CategoryId=" + _DAO.FilterString(Request.CategoryId);

            return _DAO.ParseCommonDbResponse(SQL);
        }

        public CommonDbResponse ManageCommissionStatus(string Status, string CategoryId, Common Request)
        {
            var SQL = "EXEC sproc_commission_category_management @Flag='mccs'";
            SQL += ",@CategoryId=" + _DAO.FilterString(CategoryId);
            SQL += ",@Status=" + _DAO.FilterString(Status);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            return _DAO.ParseCommonDbResponse(SQL);
        }
        #endregion

        #region Commission Setup
        public List<CommissionDetailCommon> GetCommissionDetailList(string CategoryId, string AdminCmsTypeId)
        {
            var response = new List<CommissionDetailCommon>();
            string SQL = "EXEC sproc_commission_detail_management @Flag='gcdl'";
            SQL += ",@CategoryId=" + _DAO.FilterString(CategoryId);
            SQL += ",@AdminCommissionTypeId=" + _DAO.FilterString(AdminCmsTypeId);
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new CommissionDetailCommon()
                    {
                        CategoryId = _DAO.ParseColumnValue(item, "CategoryId").ToString(),
                        CategoryDetailId = _DAO.ParseColumnValue(item, "CategoryDetailId").ToString(),
                        FromAmount = _DAO.ParseColumnValue(item, "FromAmount").ToString(),
                        ToAmount = _DAO.ParseColumnValue(item, "ToAmount").ToString(),
                        CommissionType = _DAO.ParseColumnValue(item, "CommissionType").ToString(),
                        CommissionValue = _DAO.ParseColumnValue(item, "CommissionValue").ToString(),
                        CommissionPercentageType = _DAO.ParseColumnValue(item, "CommissionPercentageType").ToString(),
                        MinCommissionValue = _DAO.ParseColumnValue(item, "MinCommissionValue").ToString(),
                        MaxCommissionValue = _DAO.ParseColumnValue(item, "MaxCommissionValue").ToString(),
                        CategoryName = _DAO.ParseColumnValue(item, "CategoryName").ToString(),
                        AdminCommissionTypeId = _DAO.ParseColumnValue(item, "AdminCommissionTypeId").ToString(),
                    });
                }
            }
            return response;
        }

        public CommissionDetailCommon GetCommissionDetailById(string CategoryDetailId)
        {
            var SQL = "sproc_commission_detail_management @Flag='gcdid'";
            SQL += ",@CategoryDetailId=" + _DAO.FilterString(CategoryDetailId);

            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0)
            {
                return new CommissionDetailCommon()
                {
                    CategoryId = dbResponse.Rows[0]["CategoryId"]?.ToString(),
                    CategoryDetailId = dbResponse.Rows[0]["CategoryDetailId"]?.ToString(),
                    FromAmount = dbResponse.Rows[0]["FromAmount"]?.ToString(),
                    ToAmount = dbResponse.Rows[0]["ToAmount"]?.ToString(),
                    CommissionType = dbResponse.Rows[0]["CommissionType"]?.ToString(),
                    CommissionValue = dbResponse.Rows[0]["CommissionValue"]?.ToString(),
                    CommissionPercentageType = dbResponse.Rows[0]["CommissionPercentageType"]?.ToString(),
                    MinCommissionValue = dbResponse.Rows[0]["MinCommissionValue"]?.ToString(),
                    MaxCommissionValue = dbResponse.Rows[0]["MaxCommissionValue"]?.ToString(),
                };
            }
            return new CommissionDetailCommon();
        }

        public CommonDbResponse ManageCommissionDetail(ManageCommissionDetailCommon Request)
        {
            var SQL = "EXEC sproc_commission_detail_management ";
            SQL += !string.IsNullOrEmpty(Request.CategoryDetailId) ? "@Flag='ucd'" : "@Flag='icd'";
            SQL += ",@CategoryId=" + _DAO.FilterString(Request.CategoryId);
            SQL += ",@AdminCommissionTypeId=" + _DAO.FilterString(Request.AdminCommissionTypeId);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            SQL += ",@FromAmount=" + _DAO.FilterString(Request.FromAmount);
            SQL += ",@ToAmount=" + _DAO.FilterString(Request.ToAmount);
            SQL += ",@CommissionValue=" + _DAO.FilterString(Request.CommissionValue);
            SQL += ",@CommissionPercentageType=" + _DAO.FilterString(Request.CommissionPercentageType);
            SQL += ",@MinCommissionValue=" + _DAO.FilterString(Request.MinCommissionValue);
            SQL += ",@MaxCommissionValue=" + _DAO.FilterString(Request.MaxCommissionValue);
            SQL += ",@CommissionType=" + _DAO.FilterString(Request.CommissionType);

            if (!string.IsNullOrEmpty(Request.CategoryDetailId))
                SQL += ",@CategoryDetailId=" + _DAO.FilterString(Request.CategoryDetailId);

            return _DAO.ParseCommonDbResponse(SQL);
        }

        public CommonDbResponse DeleteCommissionDetail(string CategoryId, string CategoryDetailId, Common Request)
        {
            var SQL = "EXEC sproc_commission_detail_management @Flag='dcd'";
            SQL += ",@CategoryId=" + _DAO.FilterString(CategoryId);
            SQL += ",@CategoryDetailId=" + _DAO.FilterString(CategoryDetailId);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            return _DAO.ParseCommonDbResponse(SQL);
        }
        #endregion

        #region Assign Commission
        public CommonDbResponse AssignCommission(AssignCommissionCommon Request)
        {
            string SQL = "EXEC sproc_commission_detail_management @Flag='acc'";
            SQL += ",@CategoryId=" + _DAO.FilterString(Request.CategoryId);
            SQL += ",@AgentId=" + _DAO.FilterString(Request.AgentId);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            return _DAO.ParseCommonDbResponse(SQL);
        }

        public List<AdminCommissionCommon> GetAdminCommissionList()
        {
            string sp_name = "EXEC sproc_admin_admincommissiontype";
            var dbResponseInfo = _DAO.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null && dbResponseInfo.Rows.Count > 0) return _DAO.DataTableToListObject<AdminCommissionCommon>(dbResponseInfo).ToList();
            return new List<AdminCommissionCommon>();
        }

        #endregion
    }
}