using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.ReservationLedger;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.ReservationLedger
{
    public interface IReservationLedgerRepository
    {
        List<ReservationLedgerCommon> GetReservationLedgerList(PaginationFilterCommon Request, string ClubId = "", string Date = "");
        List<ReservationLedgerDetailCommon> GetReservationLedgerDetail(PaginationFilterCommon Request, string ClubId, string Date);
        CommonDbResponse VerifyCode(string reservationId, string agentId, string code, Common requestCommon);
    }
}
