using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace CRS.ADMIN.APPLICATION.Models.PlanManagement
{
    public class PlansManagementModel
    {
        public List<PlanListsModel> PlanListModel { get; set; } = new List<PlanListsModel>();
        public List<PlanManagementModel> PlanManagementModel { get; set; } = new List<PlanManagementModel>();
        public PlanDetailModel PlanDetailModel { get; set; } = new PlanDetailModel();
        public PlanManagementModel PlanMgmt { get; set; } = new PlanManagementModel();
    }
    public class PlanListsModel
    {
        public string PlanTitle { get; set; }
        public string Type { get; set; }
        public string Time { get; set; }
        public string Price { get; set; }
        public string Nomination { get; set; }
        public string UpdatedDate { get; set; }
    }
    public class PlanManagementModel    
    {
        public string PlanId { get; set; }
        [DisplayName("Plan Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string PlanName { get; set; }
        [DisplayName("Plan Type")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string PlanType { get; set; }
        [DisplayName("Time")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string PlanTime { get; set; }
        [DisplayName("Price")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string Price { get; set; }
        [DisplayName("Liquor")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string Liquor { get; set; }
        [DisplayName("Nomination")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string Nomination { get; set; }
        [DisplayName("Plan Image")]
        public string PlanImage { get; set; }
        [DisplayName("Plan Image 2")]
        public string PlanImage2 { get; set; }
        public string Remarks { get; set; }
        public string PlanStatus { get; set; }
        public string ExtraField1 { get; set; }
        public string ExtraField2 { get; set; }
        public string ExtraField3 { get; set; }
        public string ActionDate { get; set; }
    }

    public class PlanDetailModel
    {
        [DisplayName("Plan Name")]
        public string PlanName { get; set; }
        [DisplayName("Plan Type")]
        public string PlanType { get; set; }
        [DisplayName("Time")]
        public string PlanTime { get; set; }
        [DisplayName("Price")]
        public string Price { get; set; }
        [DisplayName("Liquor")]
        public string Liquor { get; set; }
        [DisplayName("Nomination")]
        public string Nomination { get; set; }
        [DisplayName("Remarks")]
        public string Remarks { get; set; }
        [DisplayName("Status")]
        public string PlanStatus { get; set; }
    }
}