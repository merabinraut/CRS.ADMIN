using CRS.ADMIN.APPLICATION.Resources;
using CRS.ADMIN.SHARED;
using System.ComponentModel.DataAnnotations;

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
    public class ManageClubPointRequestModel
    {
        public string Id { get; set; }
        public string AgentId { get; set; }
        public string UserId { get; set; }
        public string TxnId { get; set; }
        public string Status { get; set; }
        public string AdminRemark { get; set; }
        public string ImageURL { get; set; }
    }
}