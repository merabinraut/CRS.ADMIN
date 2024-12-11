using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.WithdrawalRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.REPOSITORY.WithdrawalRequest
{
    public interface IWithdrawalRequestRepository
    {
        List<WithdrawalCommon> GetWithdrawal(PaginationFilterCommon request);
        List<WithdrawalMonthlyDetailsCommon> GetWithdrawalDetails(WithdrawalFilterCommon request);
        CommonDbResponse ManageRequestStatus(string id, string status, Common request);
    }
}
