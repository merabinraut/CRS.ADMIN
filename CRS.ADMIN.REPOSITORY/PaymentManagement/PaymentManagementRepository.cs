using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PaymentManagement;
using System;
using System.Collections.Generic;
using System.Data;

namespace CRS.ADMIN.REPOSITORY.PaymentManagement
{
    public class PaymentManagementRepository : IPaymentManagementRepository
    {
        private readonly RepositoryDao _dao;
        public PaymentManagementRepository() => _dao = new RepositoryDao();

        public List<PaymentLedgerCommon> GetPaymentLedgerDetail(string clubId, string date, PaginationFilterCommon Request, string FromDate = "", string ToDate = "")
        {
            var paymentLogs = new List<PaymentLedgerCommon>();

            string sql = "sproc_admin_payment_management @Flag='gpld'";
            sql += " ,@ClubId =" + _dao.FilterString(clubId);
            if (!string.IsNullOrEmpty(date))
                sql += " ,@Date=" + _dao.FilterString(date);
            sql += !string.IsNullOrEmpty(Request.SearchFilter) ? ",@SearchFilter=N" + _dao.FilterString(Request.SearchFilter) : null;
            sql += !string.IsNullOrEmpty(FromDate) ? ",@FromDate=" + _dao.FilterString(FromDate) : null;
            sql += !string.IsNullOrEmpty(ToDate) ? ",@ToDate=" + _dao.FilterString(ToDate) : null;
            sql += ",@Skip=" + Request.Skip;
            sql += ",@Take=" + Request.Take;
            var dbResp = _dao.ExecuteDataTable(sql);
            if (dbResp != null && dbResp.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dbResp.Rows)
                {
                    if (dataRow["Code"].ToString() == "0")
                    {
                        paymentLogs.Add(new PaymentLedgerCommon()
                        {
                            CustomerName = dataRow["CustomerName"].ToString(),
                            CustomerNickName = dataRow["CustomerNickName"].ToString(),
                            CustomerImage = dataRow["CustomerImage"].ToString(),
                            PlanName = dataRow["PlanName"].ToString(),
                            NoOfPeople = dataRow["NoOfPeople"].ToString(),
                            VisitDate = dataRow["VisitDate"].ToString(),
                            VisitTime = dataRow["VisitTime"].ToString(),
                            PaymentType = dataRow["PaymentType"].ToString(),
                            PlanAmount = dataRow["PlanAmount"].ToString(),
                            TotalAmount = dataRow["TotalAmount"].ToString(),
                            CommissionAmount = dataRow["CommissionAmount"].ToString(),
                            TotalCommissionAmount = dataRow["TotalCommissionAmount"].ToString(),
                            AdminPaymentAmount = dataRow["AdminPaymentAmount"].ToString(),
                            ReservationType = dataRow["ReservationType"].ToString(),
                            TotalRecords = Convert.ToInt32(_dao.ParseColumnValue(dataRow, "TotalRecords").ToString()),
                            SNO = Convert.ToInt32(_dao.ParseColumnValue(dataRow, "SNO").ToString())
                        });
                    }
                }
            }

            return paymentLogs;
        }

        public List<PaymentLogsCommon> GetPaymentLogs(string clubId, string date, PaginationFilterCommon Request, string LocationId = "", string FromDate = "", string ToDate = "")
        {
            var paymentLogs = new List<PaymentLogsCommon>();
            string sql = "sproc_admin_payment_management @Flag='gpl'";
            if (!string.IsNullOrEmpty(clubId))
                sql += " ,@ClubId=" + _dao.FilterString(clubId);
            if (!string.IsNullOrEmpty(date))
                sql += " ,@Date=" + _dao.FilterString(date);
            sql += !string.IsNullOrEmpty(Request.SearchFilter) ? ",@SearchFilter=N" + _dao.FilterString(Request.SearchFilter) : null;
            sql += !string.IsNullOrEmpty(LocationId) ? ",@LocationId=" + _dao.FilterString(LocationId) : null;
            sql += !string.IsNullOrEmpty(FromDate) ? ",@FromDate=" + _dao.FilterString(FromDate) : null;
            sql += !string.IsNullOrEmpty(ToDate) ? ",@ToDate=" + _dao.FilterString(ToDate) : null;
            sql += ",@Skip=" + Request.Skip;
            sql += ",@Take=" + Request.Take;
            var dbResp = _dao.ExecuteDataTable(sql);
            if (dbResp != null && dbResp.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dbResp.Rows)
                {
                    paymentLogs.Add(new PaymentLogsCommon()
                    {
                        ClubId = dataRow["ClubId"].ToString(),
                        ClubName = dataRow["ClubName"].ToString(),
                        ClubLogo = dataRow["ClubLogo"].ToString(),
                        ClubCategory = dataRow["ClubCategory"].ToString(),
                        //ClubMobileNumber = dataRow["ClubMobileNumber"].ToString(),
                        Location = dataRow["ClubLocation"].ToString(),
                        Date = dataRow["TransactionDate"].ToString(),
                        TransactionFormattedDate = dataRow["TransactionFormattedDate"].ToString(),
                        //PaymentStatus = dataRow["PaymentStatus"].ToString(),
                        TotalAmount = dataRow["TotalAmount"].ToString(),
                        TotalCommission = dataRow["TotalCommissionAmount"].ToString(),
                        GrandTotal = dataRow["GrandTotal"].ToString(),
                        TotalRecords = Convert.ToInt32(_dao.ParseColumnValue(dataRow, "TotalRecords").ToString()),
                        SNO = Convert.ToInt32(_dao.ParseColumnValue(dataRow, "SNO").ToString())
                    });
                }
            }

            return paymentLogs;
        }

        public PaymentOverviewCommon GetPaymentOverview()
        {
            var paymentOverview = new PaymentOverviewCommon();
            string sql = "sproc_admin_payment_management @Flag='gpmo'";
            var dbResp = _dao.ExecuteDataTable(sql);
            if (dbResp != null && dbResp.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dbResp.Rows)
                {
                    paymentOverview.ReceivedPayments = dataRow["ReceivedPayments"].ToString();
                    paymentOverview.DuePayments = dataRow["DuePayments"].ToString();
                    paymentOverview.AffiliatePayments = dataRow["AffiliatePayment"].ToString();
                }
            }
            return paymentOverview;
        }
    }
}
