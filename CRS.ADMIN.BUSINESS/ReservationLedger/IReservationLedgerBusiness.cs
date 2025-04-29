using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.ReservationLedger;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.ReservationLedger
{
    public interface IReservationLedgerBusiness
    {
        List<ReservationLedgerCommon> GetReservationLedgerList(PaginationFilterCommon Request, string ClubId = "", string Date = "");
        List<ReservationLedgerDetailCommon> GetReservationLedgerDetail(PaginationFilterCommon Request, string ClubId, string Date);
        CommonDbResponse VerifyCode(string reservationId, string agentId, string code, Common requestCommon);
    }
}
