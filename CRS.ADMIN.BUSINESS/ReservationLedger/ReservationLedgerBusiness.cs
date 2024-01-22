using CRS.ADMIN.REPOSITORY.ReservationLedger;
using CRS.ADMIN.SHARED.ReservationLedger;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.ReservationLedger
{
    public class ReservationLedgerBusiness : IReservationLedgerBusiness
    {
        private readonly IReservationLedgerRepository _repo;
        public ReservationLedgerBusiness(ReservationLedgerRepository repo) => _repo = repo;

        public List<ReservationLedgerDetailCommon> GetReservationLedgerDetail(string ClubId, string Date, string SearchText = "")
        {
            return _repo.GetReservationLedgerDetail(ClubId, Date, SearchText);  
        }

        public List<ReservationLedgerCommon> GetReservationLedgerList(string SearchText = "", string ClubId = "", string Date = "")
        {
           return _repo.GetReservationLedgerList(SearchText, ClubId, Date);
        }
    }
}
