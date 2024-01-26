using CRS.ADMIN.REPOSITORY.PaymentManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PaymentManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.PaymentManagement
{
    public class PaymentManagementBusiness : IPaymentManagementBusiness
    {
        private readonly IPaymentManagementRepository _repository;

        public PaymentManagementBusiness(PaymentManagementRepository repository) => this._repository = repository;

        public List<PaymentLedgerCommon> GetPaymentLedgerDetail(string clubId, string date, PaginationFilterCommon Request, string FromDate = "", string ToDate = "")
        {
            return _repository.GetPaymentLedgerDetail(clubId, date, Request, FromDate, ToDate);
        }
        public List<PaymentLogsCommon> GetPaymentLogs(string clubId, string date, PaginationFilterCommon Request, string LocationId = "", string FromDate = "", string ToDate = "")
        {
            return _repository.GetPaymentLogs(clubId, date, Request, LocationId, FromDate, ToDate);
        }

        public PaymentOverviewCommon GetPaymentOverview()
        {
            return _repository.GetPaymentOverview();
        }
    }
}
