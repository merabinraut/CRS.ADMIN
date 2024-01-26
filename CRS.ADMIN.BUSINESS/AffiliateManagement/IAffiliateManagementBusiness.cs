using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.AffiliateManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.AffiliateManagement
{
    public interface IAffiliateManagementBusiness
    {
        List<AffiliateManagementCommon> GetAffiliateList(PaginationFilterCommon Request);
        CommonDbResponse ApproveRejectAffiliateRequest(ApproveRejectAffiliateCommon Request);
        CommonDbResponse ManageAffiliateStatus(ManageAffiliateStatusCommon Request);
        List<ReferralConvertedCustomerListModelCommon> GetReferalConvertedCustomerList(string affiliateId, string filterDate, PaginationFilterCommon Request);
        AffiliatePageAnalyticCommon GetAffiliateAnalytic();
    }
}
