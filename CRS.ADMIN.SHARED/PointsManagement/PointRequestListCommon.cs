namespace CRS.ADMIN.SHARED.PointsManagement
{
    public class PointRequestListFilterCommon
    {
        public string SearchFilter { get; set; }
        public string LocationId { get; set; }
        public string ClubName { get; set; }
        public string PaymentMethodId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 10;
    }
    public class PointRequestListCommon
    {
        public string AgentId { get; set; }
        public string UserId { get; set; }
        public string RequestId { get; set; }
        public string RequestedDate { get; set; }
        public string ClubName { get; set; }
        public string PaymentMethod { get; set; }
        public string AmountTransferred { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string RowsTotal { get; set; }
    }

    public class ManageClubPointRequestCommon : Common
    {
        public string AgentId { get; set; }
        public string UserId { get; set; }
        public string TxnId { get; set; }
        public string Status { get; set; }
        public string AdminRemark { get; set; }
        public string ImageURL { get; set; }
    }
}
