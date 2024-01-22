using System.Collections.Generic;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Models.CommissionManagement
{
    public class CommissionDetailModel
    {
        public string CategoryId { get; set; }
        public string CategoryDetailId { get; set; }
        public string FromAmount { get; set; }
        public string ToAmount { get; set; }
        public string CommissionType { get; set; }
        public string CommissionValue { get; set; }
        public string CommissionPercentageType { get; set; }
        public string MinCommissionValue { get; set; }
        public string MaxCommissionValue { get; set; }
    }

    public class ManageCommissionDetailModel
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDetailId { get; set; }
        public string FromAmount { get; set; }
        public string ToAmount { get; set; }
        public string CommissionType { get; set; }
        public string CommissionValue { get; set; }
        public string CommissionPercentageType { get; set; }
        public string MinCommissionValue { get; set; }
        public string MaxCommissionValue { get; set; }
        public List<SelectListItem> CommissionTypeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> CommissionPercentageTypeList { get; set; } = new List<SelectListItem>();
    }

    public class CommissionDetailRazorViewModel
    {
        public List<ManageCommissionDetailModel> ManageCommissionDetailGrid { get; set; } = new List<ManageCommissionDetailModel>();
        public ManageCommissionDetailModel ManageCommissionDetailAddEdit { get; set; } = new ManageCommissionDetailModel();
    }
}