using CRS.ADMIN.REPOSITORY.AffiliateManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.AffiliateManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.AffiliateManagement
{
    public class AffiliateManagementBusiness : IAffiliateManagementBusiness
    {
        private readonly IAffiliateManagementRepository _repo;
        public AffiliateManagementBusiness(AffiliateManagementRepository repo)
        {
            _repo = repo;
        }

        public CommonDbResponse ApproveRejectAffiliateRequest(ApproveRejectAffiliateCommon Request)
        {
            return _repo.ApproveRejectAffiliateRequest(Request);
        }

        public List<AffiliateManagementCommon> GetAffiliateList(string SearchField = "")
        {
            return _repo.GetAffiliateList(SearchField);
        }

        public List<ReferralConvertedCustomerListModelCommon> GetReferalConvertedCustomerList(string searchField, string affiliateId, string filterDate)
        {
            return _repo.GetReferalConvertedCustomerList(searchField, affiliateId, filterDate);
        }

        public CommonDbResponse ManageAffiliateStatus(ManageAffiliateStatusCommon Request)
        {
            return _repo.ManageAffiliateStatus(Request);
        }

        public AffiliatePageAnalyticCommon GetAffiliateAnalytic()
        {
            return _repo.GetAffiliateAnalytic();
        }
    }
}
