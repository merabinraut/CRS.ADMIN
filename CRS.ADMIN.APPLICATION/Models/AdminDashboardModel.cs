using CRS.ADMIN.SHARED;

namespace CRS.ADMIN.APPLICATION.Models
{
    public class AdminDashboardModel : Common
    {
        public string UserType { get; set; }
        public string TotalUser { get; set; }
        public string TotalActiveUser { get; set; }
        public string ActiveUser { get; set; }
        public string InactiveUser { get; set; }
        public string SuspendedUser { get; set; }
        public string RedirectTo { get; set; }
        public string EODBalance { get; set; }
    }
    public class txnChartDataModel : Common
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Day1 { get; set; }
        public int CountDay1 { get; set; }
        public string Day2 { get; set; }
        public int CountDay2 { get; set; }
        public string Day3 { get; set; }
        public int CountDay3 { get; set; }
        public string Today { get; set; }
        public int CountToday { get; set; }
    }
    public class txnSuccessFailChart : Common
    {
        public int TotalSuccess { get; set; }
        public int TotalFailed { get; set; }
    }

    public class AccountDetailModel
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountLabel { get; set; }
    }

    public class SettlementBankBalanceModel
    {
        public string AccountNumber { get; set; }
        public string AvailableBalance { get; set; }
        public string LedgerBalance { get; set; }
        public string AccountName { get; set; }
        public string AccountLabel { get; set; }
    }
}