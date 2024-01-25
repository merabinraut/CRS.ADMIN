using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.AffiliateManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System.Collections.Generic;
using System.Data;
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
            var SQL = "EXEC sproc_admin_affiliate_management @Flag = 'gal'";
            SQL += !string.IsNullOrEmpty(Request.SearchFilter) ? ",@SearchField=N" + _dao.FilterString(Request.SearchFilter) : null;
            SQL += ",@Skip=" + Request.Skip;
            SQL += ",@Take=" + Request.Take;
            var dbResponse = _dao.ExecuteDataTable(SQL);
            if (dbResponse.Rows.Count > 0) Response = _dao.DataTableToListObject<AffiliateManagementCommon>(dbResponse).ToList();
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

        public List<ReferralConvertedCustomerListModelCommon> GetReferalConvertedCustomerList(string searchField, string affiliateId, string filterDate)
        {
            List<ReferralConvertedCustomerListModelCommon> responseInfo = new List<ReferralConvertedCustomerListModelCommon>();
            string sp_name = "EXEC sproc_admin_affiliate_management @Flag = 'grccl'";
            sp_name += !string.IsNullOrEmpty(searchField) ? ",@SearchField=N" + _dao.FilterString(searchField) : string.Empty;
            sp_name += ",@AffiliateId=" + _dao.FilterString(filterDate);
            sp_name += ",@FilterDate=" + _dao.FilterString(filterDate);
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                foreach (DataRow row in dbResponseInfo.Rows)
                {
                    responseInfo.Add(new ReferralConvertedCustomerListModelCommon()
                    {
                        AffiliateFullName = row["AffiliateFullName"].ToString(),
                        AffiliateAmount = row["AffiliateAmount"].ToString(),
                        AffiliateId = row["AffiliateId"].ToString(),
                        AffiliateImaeg = row["AffiliateImage"].ToString(),
                        CustomerConvertedDate = row["CustomerConvertedDate"].ToString(),
                        CustomerFullName = row["CustomerFullName"].ToString(),
                        CustomerId = row["CustomerId"].ToString(),
                        CustomerImage = row["CustomerImage"].ToString(),
                        CustomerUserName = row["CustomerUserName"].ToString(),
                        ReferCode = row["ReferCode"].ToString(),
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
    }
}
