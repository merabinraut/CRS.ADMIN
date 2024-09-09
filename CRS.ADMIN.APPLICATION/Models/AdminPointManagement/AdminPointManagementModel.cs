using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace CRS.ADMIN.APPLICATION.Models.AdminPointManagement
{
    public class AdminPointManagementModel
    {
        public string searchFilter { get; set; }
        public string txnId { get; set; }
        public string point { get; set; }
        public string remarks { get; set; }
        public string createdby { get; set; }
        public ManagePointRequestModel ManagePointRequest{ get; set; }
    }
    public class ManagePointRequestModel
    {
        public string point { get; set; }
        public string remarks { get; set; }

    }
}