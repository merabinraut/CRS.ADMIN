using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PaymentManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.PaymentManagement
{
    public interface IPaymentManagementBusiness
    {
        PaymentOverviewCommon GetPaymentOverview();
        List<PaymentLogsCommon> GetPaymentLogs(string clubId, string date, PaginationFilterCommon Request, string LocationId = "", string FromDate = "", string ToDate = "");
        List<PaymentLedgerCommon> GetPaymentLedgerDetail(string clubId, string date, PaginationFilterCommon Request, string FromDate = "", string ToDate = "");
    }
}