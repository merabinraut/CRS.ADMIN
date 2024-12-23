using CRS.ADMIN.REPOSITORY.WithdrawalRequest;
using CRS.ADMIN.REPOSITORY.WithdrawSetup;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PointsManagement;
using CRS.ADMIN.SHARED.WithdrawalRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.BUSINESS.WithdrawalRequest
{
    public class WithdrawalRequestBusiness: IWithdrawalRequestBusiness
    {
        IWithdrawalRequestRepository _REPO;
        public WithdrawalRequestBusiness(WithdrawalRequestRepository REPO)
        {
            _REPO = REPO;
        }
        public List<WithdrawalCommon> GetWithdrawal(PaginationFilterCommon request)
        {
            return _REPO.GetWithdrawal(request);
        }
        public List<WithdrawalMonthlyDetailsCommon> GetWithdrawalDetails(WithdrawalFilterCommon request)
        {
            return _REPO.GetWithdrawalDetails(request);
        }
        public CommonDbResponse ManageRequestStatus(string id,  string status, Common request)
        {
            return _REPO.ManageRequestStatus(id,status,request);
        }
    }
}
