namespace CRS.ADMIN.SHARED.PaymentManagement
{
    public class PaymentOverviewCommon
    {
        public string ReceivedPayments { get; set; }
        public string DuePayments { get; set; }
        public string AffiliatePayments { get; set; }
    }

    public class PaymentLogsCommon
    {
        public string ClubId { get; set; }
        public string ClubName { get; set; }
        public string ClubLogo { get; set; }
        public string ClubCategory { get; set; }
        public string ClubMobileNumber { get; set; }
        public string Location { get; set; }
        public string Date { get; set; }
        public string PaymentStatus { get; set; }
        public string TotalAmount { get; set; }
        public string TotalCommission { get; set; }
        public string GrandTotal { get; set; }
        public string TransactionFormattedDate { get; set; }
    }

    public class PaymentLedgerCommon
    {
        public string CustomerName { get; set; }
        public string CustomerNickName { get; set; }
        public string CustomerImage { get; set; }
        public string PlanName { get; set; }
        public string NoOfPeople { get; set; }
        public string VisitDate { get; set; }
        public string VisitTime { get; set; }
        public string PaymentType { get; set; }
        public string PlanAmount { get; set; }
        public string TotalAmount { get; set; }
        public string CommissionAmount { get; set; }
        public string TotalCommissionAmount { get; set; }
        public string AdminPaymentAmount { get; set; }
        public string ReservationType { get; set; }
    }
}