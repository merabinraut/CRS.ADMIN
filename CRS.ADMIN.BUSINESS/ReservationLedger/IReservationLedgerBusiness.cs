using CRS.ADMIN.SHARED.ReservationLedger;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.ReservationLedger
{
    public interface IReservationLedgerBusiness
    {
        List<ReservationLedgerCommon> GetReservationLedgerList(string SearchText = "", string ClubId = "", string Date = "");
        List<ReservationLedgerDetailCommon> GetReservationLedgerDetail(string ClubId, string Date, string SearchText = "");
    }
}
