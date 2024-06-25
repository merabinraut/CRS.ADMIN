using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.ReservationLedger;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.VariantTypes;
using System.Collections.Generic;
using System.Linq;

namespace CRS.ADMIN.REPOSITORY.ReservationLedger
{
    public class ReservationLedgerRepository : IReservationLedgerRepository
    {
        private readonly RepositoryDao _dao;
        public ReservationLedgerRepository()
        {
            _dao = new RepositoryDao();
        }

        public List<ReservationLedgerCommon> GetReservationLedgerList(PaginationFilterCommon Request, string ClubId = "", string Date = "")
        {
            string sp_name = "EXEC sproc_superadmin_getreservationledgerlist @Flag='grll'";
            sp_name += !string.IsNullOrEmpty(Request.SearchFilter) ? ",@SearchFilter=N" + _dao.FilterString(Request.SearchFilter) : "";
            sp_name += !string.IsNullOrEmpty(ClubId) ? ",@ClubId=" + _dao.FilterString(ClubId) : "";
            sp_name += !string.IsNullOrEmpty(Date) ? ",@Date=" + _dao.FilterString(Date) : "";
            sp_name += !string.IsNullOrEmpty(Request.FromDate) ? ",@FromDate=" + _dao.FilterString(Request.FromDate) : null;
            sp_name += !string.IsNullOrEmpty(Request.ToDate) ? ",@ToDate=" + _dao.FilterString(Request.ToDate) : null;
            sp_name += ",@Skip=" + Request.Skip;
            sp_name += ",@Take=" + Request.Take;
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null) return _dao.DataTableToListObject<ReservationLedgerCommon>(dbResponseInfo).ToList();
            return new List<ReservationLedgerCommon>();
        }
        public List<ReservationLedgerDetailCommon> GetReservationLedgerDetail(PaginationFilterCommon Request, string ClubId, string Date)
        {
            string sp_name = "EXEC sproc_superadmin_getreservationledgerlist @Flag='grld'";
            sp_name += !string.IsNullOrEmpty(Request.SearchFilter) ? ",@SearchFilter=N" + _dao.FilterString(Request.SearchFilter) : "";
            sp_name += !string.IsNullOrEmpty(ClubId) ? ",@ClubId=" + _dao.FilterString(ClubId) : "";
            sp_name += !string.IsNullOrEmpty(Date) ? ",@Date=" + _dao.FilterString(Date) : "";
            sp_name += !string.IsNullOrEmpty(Request.FromDate) ? ",@FromDate=" + _dao.FilterString(Request.FromDate) : null;
            sp_name += !string.IsNullOrEmpty(Request.ToDate) ? ",@ToDate=" + _dao.FilterString(Request.ToDate) : null;
            sp_name += ",@Skip=" + Request.Skip;
            sp_name += ",@Take=" + Request.Take;
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null)
            {
                var Code = _dao.ParseColumnValue(dbResponseInfo.Rows[0], "Code").ToString();
                if (Code.Trim() == "0") return _dao.DataTableToListObject<ReservationLedgerDetailCommon>(dbResponseInfo).ToList();
            }
            return new List<ReservationLedgerDetailCommon>();
        }
    }
}
