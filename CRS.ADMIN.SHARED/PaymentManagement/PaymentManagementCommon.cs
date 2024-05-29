using CRS.ADMIN.SHARED.PaginationManagement;

namespace CRS.ADMIN.SHARED.PaymentManagement
{
    public class PaymentOverviewCommon
    {
        public string ReceivedPayments { get; set; }
        public string DuePayments { get; set; }
        public string AffiliatePayments { get; set; }
    }

    public class PaymentLogsCommon : PaginationResponseCommon
    {
        public string ClubId { get; set; }
        public string ClubName { get; set; }
        public string ClubLogo { get; set; }
        public string ClubCategory { get; set; }
        public string ClubMobileNumber { get; set; }
        public string Location { get; set; }
        public string Date { get; set; }
        public string PaymentStatus { get; set; }
        public string TotalPlanAmount { get; set; }
        public string TotalAdminPlanCommissionAmount { get; set; }
        public string TotalAdminCommissionAmount { get; set; }
        public string GrandTotal { get; set; }
        public string TransactionFormattedDate { get; set; }
    }

    public class PaymentLedgerCommon : PaginationResponseCommon
    {
        public string CustomerName { get; set; }
        public string CustomerNickName { get; set; }
        public string CustomerImage { get; set; }
        public string PlanName { get; set; }
        public string NoOfPeople { get; set; }
        public string VisitDate { get; set; }
        public string VisitTime { get; set; }
        public string PaymentType { get; set; }     
        public string ReservationType { get; set; }
        public string PlanAmount { get; set; }
        public string TotalPlanAmount { get; set; }
        public string TotalClubPlanAmount { get; set; }
        public string AdminPlanCommissionAmount { get; set; }
        public string TotalAdminPlanCommissionAmount { get; set; }
        public string AdminCommissionAmount { get; set; }
        public string TotalAdminCommissionAmount { get; set; }
        public string TotalAdminPayableAmount { get; set; }
    }
}