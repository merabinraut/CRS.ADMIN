using CRS.ADMIN.SHARED.AffiliateManagement;
using CRS.ADMIN.SHARED;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.AffiliateManagement
{
    public interface IAffiliateManagementBusiness
    {
        List<AffiliateManagementCommon> GetAffiliateList(string SearchField = "");
        CommonDbResponse ApproveRejectAffiliateRequest(ApproveRejectAffiliateCommon Request);
        CommonDbResponse ManageAffiliateStatus(ManageAffiliateStatusCommon Request);
        List<ReferralConvertedCustomerListModelCommon> GetReferalConvertedCustomerList(string searchField, string affiliateId, string filterDate);
        AffiliatePageAnalyticCommon GetAffiliateAnalytic();
    }
}
