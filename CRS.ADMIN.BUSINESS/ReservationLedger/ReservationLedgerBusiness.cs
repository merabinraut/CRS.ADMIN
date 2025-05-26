using CRS.ADMIN.REPOSITORY.ReservationLedger;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.ReservationLedger;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.ReservationLedger
{
    public class ReservationLedgerBusiness : IReservationLedgerBusiness
    {
        private readonly IReservationLedgerRepository _repo;
        public ReservationLedgerBusiness(ReservationLedgerRepository repo) => _repo = repo;

        public List<ReservationLedgerDetailCommon> GetReservationLedgerDetail(PaginationFilterCommon Request, string ClubId, string Date)
        {
            return _repo.GetReservationLedgerDetail(Request, ClubId, Date);  
        }

        public List<ReservationLedgerCommon> GetReservationLedgerList(PaginationFilterCommon Request, string ClubId = "", string Date = "")
        {
           return _repo.GetReservationLedgerList(Request, ClubId, Date);
        }
        public CommonDbResponse VerifyCode(string reservationId, string agentId, string code, Common requestCommon)
        {
           return _repo.VerifyCode(reservationId,  agentId,  code, requestCommon);
        }
    }
}
