using CRS.ADMIN.SHARED.PaymentManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.PaymentManagement
{
    public interface IPaymentManagementBusiness
    {
        PaymentOverviewCommon GetPaymentOverview();
        List<PaymentLogsCommon> GetPaymentLogs(string searchText, string clubId, string date);
        List<PaymentLedgerCommon> GetPaymentLedgerDetail(string searchText, string clubId, string date);
    }
}