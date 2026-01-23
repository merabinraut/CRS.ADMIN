using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.CommissionManagement;
using CRS.ADMIN.SHARED.DiscountManagementCommon;
using CRS.ADMIN.SHARED.PaginationManagement;
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
        public List<CommissionCategoryCommon> GetCategoryList(string search)
        {
            var response = new List<CommissionCategoryCommon>();
            string SQL = "EXEC sproc_commission_category_management @Flag='gccl'";
            SQL += ",@searchText=" + _DAO.FilterString(search);
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
                        IsDefault = Convert.ToBoolean(_DAO.ParseColumnValue(item, "IsDelete")),
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
                    CreatedDate = dbResponse.Rows[0]["CreatedDate"]?.ToString(),
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

        #region Discount management 

        public List<DiscountManagementCommon> GetDiscountCategoryList(PaginationFilterCommon discountRequest)
        {
            var response = new List<DiscountManagementCommon>();
            string sql = "EXEC sproc_discount_category_management ";
            sql += !string.IsNullOrEmpty(discountRequest.SearchFilter) ? " @SearchFilter=N" + _DAO.FilterString(discountRequest.SearchFilter) : null;
            sql += !string.IsNullOrEmpty(discountRequest.SearchFilter) ? ",@Skip=" + discountRequest.Skip : "@Skip = " + discountRequest.Skip;
            sql += ",@Take=" + discountRequest.Take;
            var dbResponse = _DAO.ExecuteDataTable(sql);
            if (dbResponse != null && dbResponse.Rows.Count > 0)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new DiscountManagementCommon()
                    {
                        CategoryId = _DAO.ParseColumnValue(item, "CategoryId").ToString(),
                        CategoryName = _DAO.ParseColumnValue(item, "CategoryName").ToString(),
                        Description = _DAO.ParseColumnValue(item, "Description").ToString(),
                        Status = _DAO.ParseColumnValue(item, "Status").ToString(),
                        CreatedDate = _DAO.ParseColumnValue(item, "CreatedDate").ToString(),
                        UpdatedDate = _DAO.ParseColumnValue(item, "UpdatedDate").ToString(),
                        TotalRecords = Convert.ToInt32(_DAO.ParseColumnValue(item, "TotalRecords").ToString()),
                        SNO = Convert.ToInt32(_DAO.ParseColumnValue(item, "SNO").ToString())
                    });
                }
            }
            return response;
        }

        public CommonDbResponse ManageDiscountCategory(DiscountManagementRequestCommon mappedRequest)
        {
            string SQL = string.IsNullOrEmpty(mappedRequest.categoryId) ? "EXEC sproc_manage_discount_category" : "sproc_update_discount_category";
            string categoryName = string.IsNullOrEmpty(mappedRequest.categoryName) ? "''" : "N" + _DAO.FilterString(mappedRequest.categoryName);
            string description = string.IsNullOrEmpty(mappedRequest.description) ? "''" : "N" + _DAO.FilterString(mappedRequest.description);

            SQL += " @categoryId=" + _DAO.FilterString(mappedRequest.categoryId);
            SQL += ",@categoryName=" + categoryName;
            SQL += ",@categoryDescription=" + description;
            SQL += ",@actionUser=" + _DAO.FilterString(mappedRequest.ActionUser);
            SQL += ",@actionIP=" + _DAO.FilterString(mappedRequest.ActionIP);
            SQL += ",@actionPlatform=" + _DAO.FilterString("web");
            return _DAO.ParseCommonDbResponse(SQL);
        }

        public DiscountManagementCommon GetDiscountDetails(DiscountManagementRequestCommon common)
        {
            var SQL = "sproc_discount_detail_management ";
            SQL += " @categoryId=" + _DAO.FilterString(common.categoryId);
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0)
            {
                return new DiscountManagementCommon()
                {
                    CategoryId = dbResponse.Rows[0]["CategoryId"]?.ToString(),
                    CategoryName = dbResponse.Rows[0]["CategoryName"]?.ToString(),
                    Description = dbResponse.Rows[0]["Description"]?.ToString()
                };
            }
            return new DiscountManagementCommon();
        }

        public CommonDbResponse ChangeDiscountCategoryStatus(string categoryId, string subCategoryId, string status, string actionUser, string actionIP)
        {
            var SQL = string.IsNullOrEmpty(subCategoryId) ? "EXEC sproc_update_discount_category_status" : "EXEC sproc_update_discount_sub_category_status";
            SQL += " @CategoryId=" + _DAO.FilterString(categoryId);
            SQL += ",@SubCategoryId=" + _DAO.FilterString(subCategoryId);
            SQL += ",@status=" + _DAO.FilterString(status);
            SQL += ",@actionUser=" + _DAO.FilterString(actionUser);
            SQL += ",@actionIP=" + _DAO.FilterString(actionIP);
            SQL += ",@actionPlatform=" + _DAO.FilterString("web");
            return _DAO.ParseCommonDbResponse(SQL);
        }

        public List<DiscountCategoryDetailsResponseCommon> GetDiscountSubCategoryList(string categoryId, PaginationFilterCommon discountRequest)
        {
            var response = new List<DiscountCategoryDetailsResponseCommon>();
            string sql = "EXEC sproc_discount_sub_category_list ";
            sql += !string.IsNullOrEmpty(discountRequest.SearchFilter) ? " @SearchFilter=N" + _DAO.FilterString(discountRequest.SearchFilter) : null;
            sql += !string.IsNullOrEmpty(discountRequest.SearchFilter) ? ",@Skip=" + discountRequest.Skip : "@Skip=" + discountRequest.Skip;
            sql += ",@Take=" + discountRequest.Take;
            sql += !string.IsNullOrEmpty(categoryId) ? ",@categoryId=" + categoryId : ",@categoryId=N''";
            var dbResponse = _DAO.ExecuteDataTable(sql);
            if (dbResponse != null && dbResponse.Rows.Count > 0)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new DiscountCategoryDetailsResponseCommon()
                    {
                        CategoryId = _DAO.ParseColumnValue(item, "CategoryId").ToString(),
                        SubCategoryId = _DAO.ParseColumnValue(item, "SubCategoryId").ToString(),
                        categoryName = _DAO.ParseColumnValue(item, "CategoryName").ToString(),
                        FromAmount = Convert.ToInt64(_DAO.ParseColumnValue(item, "FromAmount").ToString()),
                        ToAmount = Convert.ToInt64(_DAO.ParseColumnValue(item, "ToAmount").ToString()),
                        Status = _DAO.ParseColumnValue(item, "Status").ToString(),
                        DiscountType = _DAO.ParseColumnValue(item, "DiscountType").ToString(),
                        Value = Convert.ToInt32(_DAO.ParseColumnValue(item, "Value").ToString()),
                        MinValue = Convert.ToInt64(_DAO.ParseColumnValue(item, "MinValue").ToString()),
                        MaxValue = Convert.ToInt64(_DAO.ParseColumnValue(item, "MaxValue").ToString()),
                        TotalRecords = Convert.ToInt32(_DAO.ParseColumnValue(item, "TotalRecords").ToString()),
                        createdDate = _DAO.ParseColumnValue(item, "createdDate").ToString(),
                        updatedDate = _DAO.ParseColumnValue(item, "updatedDate").ToString(),
                        SNO = Convert.ToInt32(_DAO.ParseColumnValue(item, "SNO").ToString())
                    });
                }
            }
            return response;
        }

        public CommonDbResponse ManageDiscountSubCategory(DiscountSubCategoryManagementRequestCommon mappedRequest)
        {
            var SQL = "EXEC sproc_manage_discount_sub_category";
            SQL += " @CategoryId=" + _DAO.FilterString(mappedRequest.categoryId);
            SQL += ",@SubCategoryId=" + _DAO.FilterString(mappedRequest.subCategoryId);
            SQL += ",@FromAmount=" + mappedRequest.FromAmount;
            SQL += ",@ToAmount=" + mappedRequest.ToAmount;
            SQL += ",@DiscountType=" + mappedRequest.DiscountType;
            SQL += ",@Value=" + mappedRequest.Value;
            SQL += ",@MinValue=" + (mappedRequest.MinValue ?? 0); ;
            SQL += ",@MaxValue=" + (mappedRequest.MaxValue ?? 0);
            SQL += ",@ActionIp=" + _DAO.FilterString(mappedRequest.ActionIP);
            SQL += ",@CreatedBy=" + _DAO.FilterString(mappedRequest.ActionUser);
            SQL += ",@actionPlatform=" + _DAO.FilterString(mappedRequest.ActionPlatform);
            return _DAO.ParseCommonDbResponse(SQL);
        }

        public DiscountCategoryDetailsResponseCommon GetSubCategoryDiscountDetails(DiscountSubCategoryRequestCommon common)
        {
            var SQL = "sproc_discount_sub_category_detail_management ";
            SQL += " @categoryId=" + _DAO.FilterString(common.categoryId);
            SQL += ",@subCategoryId=" + _DAO.FilterString(common.subCategoryId);
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0)
            {
                return new DiscountCategoryDetailsResponseCommon()
                {
                    CategoryId = dbResponse.Rows[0]["CategoryId"]?.ToString(),
                    SubCategoryId = dbResponse.Rows[0]["SubCategoryId"]?.ToString(),
                    categoryName = dbResponse.Rows[0]["categoryName"]?.ToString(),
                    FromAmount = Convert.ToInt64(dbResponse.Rows[0]["FromAmount"]?.ToString()),
                    ToAmount = Convert.ToInt64(dbResponse.Rows[0]["ToAmount"]?.ToString()),
                    DiscountType = dbResponse.Rows[0]["DiscountType"]?.ToString(),
                    Value = Convert.ToInt64(dbResponse.Rows[0]["Value"]?.ToString()),
                    MinValue = Convert.ToInt64(dbResponse.Rows[0]["MinValue"]?.ToString()),
                    MaxValue = Convert.ToInt64(dbResponse.Rows[0]["MaxValue"]?.ToString())
                };
            }
            return new DiscountCategoryDetailsResponseCommon();
        }

        public AssignDiscountCommon GetSubCategoryDetails(string agentId)
        {
            var SQL = "sproc_dropdown_management ";
            SQL += " @flag=" + _DAO.FilterString("CAT");
            SQL += ",@SearchField1=" + _DAO.FilterString(agentId);
            SQL += ",@SearchField2=" + _DAO.FilterString("");
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0)
            {
                return new AssignDiscountCommon()
                {
                    CategoryId = dbResponse.Rows[0]["Value"]?.ToString(),
                    CategoryName = dbResponse.Rows[0]["TEXT"]?.ToString(),
                };
            }
            return new AssignDiscountCommon();
        }

        public CommonDbResponse AssignDiscount(AssignDiscountCommon requestMapped)
        {
            var SQL = "EXEC sproc_assign_discount_category_to_club";
            SQL += " @CategoryId=" + _DAO.FilterString(requestMapped.CategoryId);
            SQL += ",@clubId=" + _DAO.FilterString(requestMapped.AgentId);
            SQL += ",@ActionIp=" + _DAO.FilterString(requestMapped.ActionIP);
            SQL += ",@CreatedBy=" + _DAO.FilterString(requestMapped.ActionUser);
            SQL += ",@ActionPlatform=" + _DAO.FilterString(requestMapped.ActionPlatform);
            return _DAO.ParseCommonDbResponse(SQL);
        }
        #endregion
    }
}