using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.AffiliateManagement;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CRS.ADMIN.REPOSITORY.AffiliateManagement
{
    public class AffiliateManagementRepository : IAffiliateManagementRepository
    {
        private readonly RepositoryDao _dao;
        public AffiliateManagementRepository(RepositoryDao dao)
        {
            _dao = dao;
        }

        public List<AffiliateManagementCommon> GetAffiliateList(PaginationFilterCommon Request)
        {
            var Response = new List<AffiliateManagementCommon>();
            var SQL = "EXEC sproc_admin_affiliate_management @Flag = 'g_al'";
            SQL += !string.IsNullOrEmpty(Request.SearchFilter) ? ",@SearchFilter=N" + _dao.FilterString(Request.SearchFilter) : null;
            SQL += ",@Skip=" + Request.Skip;
            SQL += ",@Take=" + Request.Take;
            var dbResponse = _dao.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0) Response = _dao.DataTableToListObject<AffiliateManagementCommon>(dbResponse).ToList();
            return Response;
        }

        public CommonDbResponse ApproveRejectAffiliateRequest(ApproveRejectAffiliateCommon Request)
        {
            string SQL = "EXEC sproc_admin_affiliate_management ";
            if (Request.ApprovedStatus.Trim().ToUpper() == "A") SQL += "@Flag = 'aarr'";
            else SQL += "@Flag = 'rarr'";
            SQL += ",@HoldAgentId=" + _dao.FilterString(Request.HoldAgentId);
            SQL += ",@ActionUser=" + _dao.FilterString(Request.ActionUser);
            SQL += ",@ActionIP=" + _dao.FilterString(Request.ActionIP);
            SQL += ",@ActionPlatform=" + _dao.FilterString(Request.ActionPlatform);
            return _dao.ParseCommonDbResponse(SQL);
        }

        public CommonDbResponse ManageAffiliateStatus(ManageAffiliateStatusCommon Request)
        {
            string SQL = "EXEC sproc_admin_affiliate_management @Flag = 'mas'";
            SQL += ",@AgentId=" + _dao.FilterString(Request.AgentId);
            SQL += ",@Status=" + _dao.FilterString(Request.Status);
            SQL += ",@ActionUser=" + _dao.FilterString(Request.ActionUser);
            SQL += ",@ActionIP=" + _dao.FilterString(Request.ActionIP);
            SQL += ",@ActionPlatform=" + _dao.FilterString(Request.ActionPlatform);
            return _dao.ParseCommonDbResponse(SQL);
        }

        public List<ReferralConvertedCustomerListModelCommon> GetReferalConvertedCustomerList(string affiliateId, string filterDate, PaginationFilterCommon Request)
        {
            List<ReferralConvertedCustomerListModelCommon> responseInfo = new List<ReferralConvertedCustomerListModelCommon>();
            string sp_name = "EXEC sproc_admin_affiliate_management @Flag = 'grccl'";
            sp_name += !string.IsNullOrEmpty(Request.SearchFilter) ? ",@SearchFilter=N" + _dao.FilterString(Request.SearchFilter) : string.Empty;
            sp_name += !string.IsNullOrEmpty(affiliateId) ? ",@AffiliateId=" + _dao.FilterString(affiliateId) : string.Empty;
            sp_name += !string.IsNullOrEmpty(filterDate) ? ",@FilterDate=" + _dao.FilterString(filterDate) : string.Empty;
            sp_name += ",@Skip=" + Request.Skip;
            sp_name += ",@Take=" + Request.Take;
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new ReferralConvertedCustomerListModelCommon()
                    {
                        AffiliateFullName = row["AffiliateFullName"].ToString(),
                        AffiliateAmount =Convert.ToInt64( row["AffiliateAmount"]).ToString("N0"),
                        AffiliateId = row["AffiliateCode"].ToString(),
                        AffiliateImaeg = row["AffiliateImage"].ToString(),
                        CustomerConvertedDate = row["CustomerConvertedDate"].ToString(),
                        CustomerFullName = row["CustomerFullName"].ToString(),
                        //CustomerId = row["CustomerId"].ToString(),
                        CustomerImage = row["CustomerImage"].ToString(),
                        CustomerUserName = row["NickName"].ToString(),
                        //ReferCode = row["ReferCode"].ToString(),
                        TotalRecords = Convert.ToInt32(_dao.ParseColumnValue(row, "TotalRecords").ToString()),
                        SNO = Convert.ToInt32(_dao.ParseColumnValue(row, "SNO").ToString())
                    });
                }
            }
            return responseInfo;
        }

        public AffiliatePageAnalyticCommon GetAffiliateAnalytic()
        {
            string SQL = "EXEC dbo.sproc_admin_affiliate_management @Flag = 'gapa'";
            var dbResponse = _dao.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0) return _dao.DataTableToListObject<AffiliatePageAnalyticCommon>(dbResponse).First();
            return new AffiliatePageAnalyticCommon();
        }
        public CommonDbResponse ResetPassword(ManageAffiliateStatusCommon Request, SqlConnection connection = null, SqlTransaction transaction = null )
        {
            string SQL = "EXEC sproc_admin_affiliate_management @Flag = 'res_ap'";
            SQL += ",@AgentId=" + _dao.FilterString(Request.AgentId);
            SQL += ",@ActionUser=" + _dao.FilterString(Request.ActionUser);
            SQL += ",@ActionIP=" + _dao.FilterString(Request.ActionIP);
            SQL += ",@ActionPlatform=" + _dao.FilterString(Request.ActionPlatform);
            var _sqlTransactionHandler = new RepositoryDaoWithTransaction(connection, transaction);
            return _sqlTransactionHandler.ParseCommonDbResponse(SQL);
            //return _dao.ParseCommonDbResponse(SQL);
        }
        public ManageAffiliateCommon GetAffiliateDetails(string AffiliateId, String culture = "")
        {

            ManageAffiliateCommon ClubDetail = new ManageAffiliateCommon();
            string SQL = "EXEC sproc_admin_affiliate_management @Flag='g_ad'";
            SQL += ",@AffiliateId=" + _dao.FilterString(AffiliateId);
            var dbResponse = _dao.ExecuteDataRow(SQL);
            if (dbResponse != null)
            {
                return new ManageAffiliateCommon()
                {
                    AffiliateId = _dao.ParseColumnValue(dbResponse, "AgentId").ToString(),
                    LoginId = _dao.ParseColumnValue(dbResponse, "LoginId").ToString(),
                    UserName = _dao.ParseColumnValue(dbResponse, "AffiliateCode").ToString(),
                    FullName = _dao.ParseColumnValue(dbResponse, "FullName").ToString(),
                    MobileNumber = _dao.ParseColumnValue(dbResponse, "MobileNumber").ToString(),
                    EmailAddress = _dao.ParseColumnValue(dbResponse, "EmailAddress").ToString(),
                    BirthDateYear = _dao.ParseColumnValue(dbResponse, "BirthDateYear").ToString(),
                    BirthDateMonth = _dao.ParseColumnValue(dbResponse, "BirthDateMonth").ToString(),
                    BirthDateDay = _dao.ParseColumnValue(dbResponse, "BirthDateDay").ToString(),
                    Gender = _dao.ParseColumnValue(dbResponse, "Gender").ToString(),
                    PostalCode = _dao.ParseColumnValue(dbResponse, "PostalCode").ToString(),
                    FullNameFurigana = _dao.ParseColumnValue(dbResponse, "FullNameFurigana").ToString(),
                    Prefecture = _dao.ParseColumnValue(dbResponse, "Prefecture").ToString(),
                    City = _dao.ParseColumnValue(dbResponse, "City").ToString(),
                    Street = _dao.ParseColumnValue(dbResponse, "Street").ToString(),
                    BuildingRoomNo = _dao.ParseColumnValue(dbResponse, "BuildingNo").ToString(),
                    BusinessType = _dao.ParseColumnValue(dbResponse, "BusinessType").ToString(),
                    CEOName = _dao.ParseColumnValue(dbResponse, "CEOName").ToString(),
                    CEONameFurigana = _dao.ParseColumnValue(dbResponse, "CEONameFurigana").ToString(),
                    CompanyName = _dao.ParseColumnValue(dbResponse, "CompanyName").ToString(),
                    CompanyAddress = _dao.ParseColumnValue(dbResponse, "CompanyAddress").ToString(),
                };
            }
            return new ManageAffiliateCommon();
        }

        public CommonDbResponse ManageAffiliate(ManageAffiliateCommon Request)
        {
            var Response = new CommonDbResponse();
            string SQL = "EXEC sproc_admin_affiliate_management @Flag='u_am'";
            SQL += ",@LoginId=" + (!string.IsNullOrEmpty(Request.LoginId) ? "N" + _dao.FilterString(Request.LoginId) : _dao.FilterString(Request.LoginId));
            SQL += ",@Email=" + _dao.FilterString(Request.EmailAddress);
            SQL += ",@MobileNumber=" + _dao.FilterString(Request.MobileNumber);
            SQL += ",@Fullname=" + (!string.IsNullOrEmpty(Request.FullName) ? "N" + _dao.FilterString(Request.FullName) : _dao.FilterString(Request.FullName));   
            SQL += ",@BirthDateYear=" + _dao.FilterString(Request.BirthDateYear);
            SQL += ",@BirthDateMonth=" + _dao.FilterString(Request.BirthDateMonth);
            SQL += ",@BirthDateDay=" + _dao.FilterString(Request.BirthDateDay);
            SQL += ",@Gender=" + _dao.FilterString(Request.Gender);
            SQL += ",@PostalCode=" + _dao.FilterString(Request.PostalCode);
            SQL += ",@FullNameFurigana=" + (!string.IsNullOrEmpty(Request.FullNameFurigana) ? "N" + _dao.FilterString(Request.FullNameFurigana) : _dao.FilterString(Request.FullNameFurigana)); 
            SQL += ",@Prefecture=" + (!string.IsNullOrEmpty(Request.Prefecture) ? "N" + _dao.FilterString(Request.Prefecture) : _dao.FilterString(Request.Prefecture));  
            SQL += ",@City=" + (!string.IsNullOrEmpty(Request.City) ? "N" + _dao.FilterString(Request.City) : _dao.FilterString(Request.City)); 
            SQL += ",@Street=" + (!string.IsNullOrEmpty(Request.Street) ? "N" + _dao.FilterString(Request.Street) : _dao.FilterString(Request.Street));   
            SQL += ",@BuildingNo=" + (!string.IsNullOrEmpty(Request.BuildingRoomNo) ? "N" + _dao.FilterString(Request.BuildingRoomNo) : _dao.FilterString(Request.BuildingRoomNo));   
            SQL += ",@BusinessType=" + (!string.IsNullOrEmpty(Request.BusinessType) ? "N" + _dao.FilterString(Request.BusinessType) : _dao.FilterString(Request.BusinessType));
            if (Request.BusinessType=="1")
            {
                SQL += ",@CEOName=" + _dao.FilterString(Request.CEOName);
                SQL += ",@CEONameFurigana=" + (!string.IsNullOrEmpty(Request.CEONameFurigana) ? "N" + _dao.FilterString(Request.CEONameFurigana) : _dao.FilterString(Request.CEONameFurigana));
                SQL += ",@CompanyName=" + (!string.IsNullOrEmpty(Request.CompanyName) ? "N" + _dao.FilterString(Request.CompanyName) : _dao.FilterString(Request.CompanyName));
                SQL += ",@CompanyAddress=" + (!string.IsNullOrEmpty(Request.CompanyAddress) ? "N" + _dao.FilterString(Request.CompanyAddress) : _dao.FilterString(Request.CompanyAddress));
            }
            SQL += ",@AffiliateId=" + _dao.FilterString(Request.AffiliateId);
            Response = _dao.ParseCommonDbResponse(SQL);
            return Response;

        }
    }
}
