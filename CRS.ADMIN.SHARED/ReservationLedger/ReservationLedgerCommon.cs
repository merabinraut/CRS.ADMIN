using CRS.ADMIN.SHARED.PaginationManagement;

namespace CRS.ADMIN.SHARED.ReservationLedger
{
    public class ReservationLedgerCommon : PaginationResponseCommon
    {
        public string ClubId { get; set; }
        public string ClubName { get; set; }
        public string ClubLogo { get; set; }
        public string ClubCategory { get; set; }
        public string AdminPayment { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionFormattedDate { get; set; }
        public string TotalVisitors { get; set; }
    }
    public class ReservationLedgerDetailCommon : PaginationResponseCommon
    {
        public string ClubId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNickName { get; set; }
        public string PlanName { get; set; }
        public string NoOfPeople { get; set; }
        public string VisitTime { get; set; }
        public string VisitDate { get; set; }
        public string PaymentType { get; set; }
        public string CustomerImage { get; set; }
        public string ReservationType { get; set; }
        public string ClubVerification { get; set; }
        public string TransactionStatus { get; set; }
        public string AdminRemarks { get; set; }
        public string PlanAmount { get; set; }
        public string TotalPlanAmount { get; set; }
        public string TotalClubPlanAmount { get; set; }
        public string AdminPlanCommissionAmount { get; set; }
        public string TotalAdminPlanCommissionAmount { get; set; }
        public string AdminCommissionAmount { get; set; }
        public string TotalAdminCommissionAmount { get; set; }
        public string TotalAdminPayableAmount { get; set; }
        public string CreatedDate { get; set; }
        public string Id { get; set; }
        public string InvoiceId { get; set; }
    }
}
