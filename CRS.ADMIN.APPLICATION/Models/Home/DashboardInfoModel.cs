namespace CRS.ADMIN.APPLICATION.Models.Home
{
    public class DashboardInfoModel
    {
        public int TotalClubs { get; set; }
        public int TotalVisitors { get; set; }
        public decimal TotalSales { get; set; }
        public string Username { get; set; }
    }
    public class HostListModel
    {
        public string HostName { get; set; }
        public string EmailAddress { get; set; }
        public string HostAddress { get; set; }
        public decimal HostCharge { get; set; }
        public string HostImage { get; set; }
    }
    public class ChartInfo
    {
        public string Month { get; set; }
        public string TotalSales { get; set; }
    }
    public class TopBookedHostRankingModel
    {
        public string HostName { get; set; }
        public string HostImage { get; set; }
        public string HostUsername { get; set; }
        public string HostCount { get; set; }
        public string ClubName { get; set; }
        public string ClubImage { get; set; }
        public string Price { get; set; }
        public string PlanName { get; set; }
    }
    public class ReceivedAmountModel
    {
        public string PaymentMethod { get; set; }
        public string TotalAmount { get; set; }
        public string TodayDate { get; set; }
    }
}