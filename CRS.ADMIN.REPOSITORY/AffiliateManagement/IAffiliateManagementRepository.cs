using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.AffiliateManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.AffiliateManagement
{
    public interface IAffiliateManagementRepository
    {
        List<AffiliateManagementCommon> GetAffiliateList(string SearchField = "");
        CommonDbResponse ApproveRejectAffiliateRequest(ApproveRejectAffiliateCommon Request);
        CommonDbResponse ManageAffiliateStatus(ManageAffiliateStatusCommon Request);
        List<ReferralConvertedCustomerListModelCommon> GetReferalConvertedCustomerList(string searchField, string affiliateId, string filterDate);
        AffiliatePageAnalyticCommon GetAffiliateAnalytic();
    }
}
