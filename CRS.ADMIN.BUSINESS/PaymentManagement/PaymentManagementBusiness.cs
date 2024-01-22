using CRS.ADMIN.REPOSITORY.PaymentManagement;
using CRS.ADMIN.SHARED.PaymentManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.PaymentManagement
{
    public class PaymentManagementBusiness : IPaymentManagementBusiness
    {
        private readonly IPaymentManagementRepository _repository;

        public PaymentManagementBusiness(PaymentManagementRepository repository) => this._repository = repository;

        public List<PaymentLedgerCommon> GetPaymentLedgerDetail(string searchText, string clubId, string date)
        {
            return _repository.GetPaymentLedgerDetail(searchText, clubId, date);
        }
        public List<PaymentLogsCommon> GetPaymentLogs(string searchText, string clubId, string date)
        {
            return _repository.GetPaymentLogs(searchText, clubId, date);
        }

        public PaymentOverviewCommon GetPaymentOverview()
        {
            return _repository.GetPaymentOverview();
        }
    }
}
