using CRS.ADMIN.APPLICATION.Models.TagManagement;
using CRS.ADMIN.APPLICATION.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace CRS.ADMIN.APPLICATION.Models.Withdraw
{
    public class WithdrawSetupModel
    {
        public List<WithdrawSetupList> WithdrawSetupList { get; set; }=new List<WithdrawSetupList>();
        public ManageWithdrawSetupModel ManageWithdrawSetup { get; set; } = new ManageWithdrawSetupModel();
    }
    public class WithdrawSetupList
    {
        public string id { get; set; }
        public string minAmount { get; set; }
        public string maxAmount { get; set; }
        public string fromDate { get; set; }
        public string toDate{ get; set; }
        public string withdrawDate { get; set; }
        
    }
    public class ManageWithdrawSetupModel
    {
        public string id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [RegularExpression("^[0-9]+$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Invalid_amount")]
        public string minAmount { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [RegularExpression("^[0-9]+$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Invalid_amount")]
        public string maxAmount { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string fromDate { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string toDate { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string withdrawDate { get; set; } 
        public string InsertType { get; set; }

    }
}
