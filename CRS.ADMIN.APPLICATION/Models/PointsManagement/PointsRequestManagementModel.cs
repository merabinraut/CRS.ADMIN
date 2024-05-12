namespace CRS.ADMIN.APPLICATION.Models.PointsManagement
{
    public class PointsRequestListModel
    {
        public string RequestId { get; set; }
        public string RequestDate { get; set; }
        public string ClubName { get; set; }
        public string PaymentMethod { get; set; }
        public string AmountTransfered { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
    }
}