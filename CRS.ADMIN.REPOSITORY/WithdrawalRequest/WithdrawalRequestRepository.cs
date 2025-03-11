using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.ScheduleManagement;
using CRS.ADMIN.SHARED.WithdrawalRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.REPOSITORY.WithdrawalRequest
{
    public class WithdrawalRequestRepository: IWithdrawalRequestRepository
    {
        private readonly RepositoryDao _dao;
        public WithdrawalRequestRepository(RepositoryDao dao)
        {
            _dao = dao;
        }
        public List<WithdrawalCommon> GetWithdrawal(PaginationFilterCommon request)
        {
            var SQL = "EXEC sproc_admin_withdrawal_monthly_list ";
            SQL += " @FromDate=" + _dao.FilterString(request.FromDate);
            SQL += ",@ToDate=" + _dao.FilterString(request.ToDate);
            SQL += " ,@Skip=" + request.Skip;
            SQL += " ,@Take =" + request.Take;
            var dbResponse = _dao.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0) return _dao.DataTableToListObject<WithdrawalCommon>(dbResponse).ToList();
            return new List<WithdrawalCommon>();
        }
        public List<WithdrawalMonthlyDetailsCommon> GetWithdrawalDetails(WithdrawalFilterCommon request)
        {
            var SQL = "EXEC sproc_admin_withdrawal_monthly_detail ";
            SQL += " @fromDate=" + _dao.FilterString(request.FromDate);
            SQL += ",@toDate=" + _dao.FilterString(request.ToDate);
            SQL += " ,@id=" + _dao.FilterString(request.id);
            SQL += !string.IsNullOrEmpty(request.SearchFilter) ? " ,@search=N" + _dao.FilterString(request.SearchFilter) : " ,@search=null ";
            //SQL += " ,@bankType=" + _dao.FilterString(request.bankType);
            //SQL += ",@bankName=" + _dao.FilterString(request.bankName);
            //SQL += " ,@branchName =" + _dao.FilterString(request.branchName);
            //SQL += ",@affiliateName=" + _dao.FilterString(request.affiliateInfo);
            SQL += " ,@Skip=" + request.Skip;
            SQL += " ,@Take =" + request.Take;
            var dbResponse = _dao.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0) return _dao.DataTableToListObject<WithdrawalMonthlyDetailsCommon>(dbResponse).ToList();
            return new List<WithdrawalMonthlyDetailsCommon>();
        }
        public CommonDbResponse ManageRequestStatus(string id, string status, Common request)
        {
            string SQL = "EXEC sproc_admin_withdrawal_acceptreject";
            SQL += " @id=" + _dao.FilterString(id);
            SQL += ",@status=" + _dao.FilterString(status);
            SQL += ",@actionUser=" + _dao.FilterString(request.ActionUser);
            
            return _dao.ParseCommonDbResponse(SQL);
        }
    }
}
