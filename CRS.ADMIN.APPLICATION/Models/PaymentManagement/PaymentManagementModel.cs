using System.Collections.Generic;

namespace CRS.ADMIN.APPLICATION.Models.PaymentManagement
{
    public class PaymentManagementModel
    {
        public PaymentOverviewModel PaymentOverview { get; set; }
        public List<PaymentLogsModel> PaymentLogs { get; set; }
       
    }
    public class PaymentOverviewModel
    {
        public string ReceivedPayments { get; set; }
        public string DuePayments { get; set; }
        public string AffiliatePayments { get; set; }
    }
    public class PaymentLogsModel
    {
        public string SNO { get; set; }
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
        public string Value { get; set; }
    }

    public class PaymentLedgerModel
    {
        public string SNO { get; set; }
        public string ClubId { get; set; }
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