using CRS.ADMIN.SHARED.NotificationManagement;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.Collections.Generic;

namespace CRS.ADMIN.SHARED.Home
{
    #region Login
    public class LoginRequestCommon
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string SessionId { get; set; }
    }
    public class LoginResponseCommon
    {
        public string RoleId { get; set; }
        public string UserId { get; set; }
        public string RoleName { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string ProfileImage { get; set; }
        public string Session { get; set; }
        public string IsPasswordForceful { get; set; }
        public string LastPasswordChangedDate { get; set; }
        public List<MenuCommon> Menus { get; set; }
        public List<string> Functions { get; set; }
        public List<NotificationDetailCommon> Notifications { get; set; }
    }

    public class MenuCommon
    {
        public string MenuName { get; set; }
        public string MenuUrl { get; set; }
        public string MenuGroup { get; set; }
        public string ParentGroup { get; set; }
        public string CssClass { get; set; }
        public string GroupOrderPosition { get; set; }
        public string MenuOrderPosition { get; set; }
    }
    #endregion

    #region "Dashboard Info"
    public class DashboardInfoModelCommon
    {
        public int TotalClubs { get; set; }
        public int TotalBasicClubs { get; set; }
        public int TotalVisitors { get; set; }
        public decimal TotalSales { get; set; }
    }
    public class HostListModelCommon
    {
        public string HostName { get; set; }
        public string EmailAddress { get; set; }
        public string HostAddress { get; set; }
        public decimal HostCharge { get; set; }
        public string HostImage { get; set; }
    }
    public class ChartInfoCommon
    {
        public string Month { get; set; }
        public string TotalSales { get; set; }
    }
    public class TopBookedHostRankingModelCommon
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
    public class ReceivedAmountModelCommon
    {
        public string PaymentMethod { get; set; }
        public string TotalAmount { get; set; }
        public string TodayDate { get; set; }
    }
    #endregion
}
