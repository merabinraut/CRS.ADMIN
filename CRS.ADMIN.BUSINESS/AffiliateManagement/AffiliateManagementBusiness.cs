using CRS.ADMIN.REPOSITORY.AffiliateManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.AffiliateManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
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

        public List<AffiliateManagementCommon> GetAffiliateList(PaginationFilterCommon Request)
        {
            return _repo.GetAffiliateList(Request);
        }

        public List<ReferralConvertedCustomerListModelCommon> GetReferalConvertedCustomerList(string affiliateId, string filterDate, PaginationFilterCommon Request)
        {
            return _repo.GetReferalConvertedCustomerList(affiliateId, filterDate, Request);
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
