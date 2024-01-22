using CRS.ADMIN.SHARED.PaymentManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.PaymentManagement
{
    public interface IPaymentManagementRepository
    {
        PaymentOverviewCommon GetPaymentOverview();
        List<PaymentLogsCommon> GetPaymentLogs(string searchText, string clubId, string date);
        List<PaymentLedgerCommon> GetPaymentLedgerDetail(string searchText, string clubId, string date);
    }
}