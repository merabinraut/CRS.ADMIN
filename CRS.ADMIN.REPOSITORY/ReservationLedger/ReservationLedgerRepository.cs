using CRS.ADMIN.SHARED.ReservationLedger;
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

        public List<ReservationLedgerCommon> GetReservationLedgerList(string SearchText = "", string ClubId = "", string Date = "")
        {
            string sp_name = "EXEC sproc_superadmin_getreservationledgerlist @Flag='grll'";
            sp_name += !string.IsNullOrEmpty(SearchText) ? ",@SearchField=" + _dao.FilterString(SearchText) : "";
            sp_name += !string.IsNullOrEmpty(ClubId) ? ",@ClubId=" + _dao.FilterString(ClubId) : "";
            sp_name += !string.IsNullOrEmpty(Date) ? ",@Date=" + _dao.FilterString(Date) : "";
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null) return _dao.DataTableToListObject<ReservationLedgerCommon>(dbResponseInfo).ToList();
            return new List<ReservationLedgerCommon>();
        }
        public List<ReservationLedgerDetailCommon> GetReservationLedgerDetail(string ClubId, string Date, string SearchText = "")
        {
            string sp_name = "EXEC sproc_superadmin_getreservationledgerlist @Flag='grld'";
            sp_name += !string.IsNullOrEmpty(SearchText) ? ",@SearchField=" + _dao.FilterString(SearchText) : "";
            sp_name += !string.IsNullOrEmpty(ClubId) ? ",@ClubId=" + _dao.FilterString(ClubId) : "";
            sp_name += !string.IsNullOrEmpty(Date) ? ",@Date=" + _dao.FilterString(Date) : "";
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
