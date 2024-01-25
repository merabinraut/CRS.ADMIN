using CRS.ADMIN.SHARED.PaginationManagement;

namespace CRS.ADMIN.SHARED.AffiliateManagement
{
    public class AffiliateManagementCommon : PaginationResponseCommon
    {
        public string AffiliateId { get; set; }
        public string HoldAffiliateId { get; set; }
        public string AffiliateImage { get; set; }
        public string AffiliateFullName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public string RequestedDate { get; set; }
        public string Status { get; set; }
        public string ApprovedStatus { get; set; }
        public string SnsURLLink { get; set; }
        public string AffiliateType { get; set; }
    }

    public class ApproveRejectAffiliateCommon : Common
    {
        public string HoldAgentId { get; set; }
        public string ApprovedStatus { get; set; }
    }

    public class ManageAffiliateStatusCommon : Common
    {
        public string AgentId { get; set; }
        public string Status { get; set; }
    }
    public class ReferralConvertedCustomerListModelCommon
    {
        public string CustomerId { get; set; }
        public string ReferCode { get; set; }
        public string CustomerImage { get; set; }
        public string CustomerFullName { get; set; }
        public string CustomerUserName { get; set; }
        public string CustomerConvertedDate { get; set; }
        public string AffiliateId { get; set; }
        public string AffiliateFullName { get; set; }
        public string AffiliateAmount { get; set; }
        public string AffiliateImaeg { get; set; }
    }

    public class AffiliatePageAnalyticCommon
    {
        public string TotalClick { get; set; }
        public string TotalAffiliates { get; set; }
        public string TotalConvertedCustomer { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Facebook { get; set; }
        public string Tiktok { get; set; }
        public string Line { get; set; }
        public string TwitterPercentage { get; set; }
        public string InstagramPercentage { get; set; }
        public string FacebookPercentage { get; set; }
        public string TiktokPercentage { get; set; }
        public string LinePercentage { get; set; }
    }
}
