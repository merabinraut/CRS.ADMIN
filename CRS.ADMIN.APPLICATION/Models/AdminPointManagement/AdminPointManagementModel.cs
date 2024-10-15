using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace CRS.ADMIN.APPLICATION.Models.AdminPointManagement
{
    public class AdminPointManagementModel
    {
        public string searchFilter { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public List<PointRequestDetail> PointRequestDetailList { get; set; }
        public ManagePointRequestModel ManagePointRequest{ get; set; }
    }
    public class ManagePointRequestModel
    {
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "The point field must be a number.")]
        public string point { get; set; }
        public string remarks { get; set; }

    }

    public class PointRequestDetail
    {
        public string txnId { get; set; }
        public string point { get; set; }
        public string remarks { get; set; }
        public string createdby { get; set; }
        public string txnDate { get; set; }

    }
}