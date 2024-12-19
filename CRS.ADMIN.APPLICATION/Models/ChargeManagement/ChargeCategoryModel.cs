using CRS.ADMIN.APPLICATION.Resources;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.APPLICATION.Models.ChargeManagement
{
    public class ChargeTypeModel
    {
        public string agentTypeValue { get; set; }
        public string SearchFilter { get; set; }
        public ManageChargeCategoryModel ManageCategory { get; set; }
        public List<ChargeCategoryManagementModel> CategoryTypeList { get; set; }
        public List<ChargeCategoryManagementModel> ChargeTypeList { get; set; }
    }
    public class ChargeCategoryManagementModel
    {
        public string categoryId { get; set; }
        public string categoryName { get; set; }
        public string description { get; set; }
        public string isDefault { get; set; }
        public string agentType { get; set; }
        public string agentTypeValue { get; set; }
        public string isUserCandeleteCategory { get; set; }
        public string status { get; set; }
        public string SNO { get; set; }
        public string RowsTotal { get; set; }
    }
    public class ManageChargeCategoryModel
    {
        public string categoryId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string categoryName { get; set; }
        public string description { get; set; }
        public string isDefault { get; set; }
        public string agentType { get; set; }
        public string agentTypeValue { get; set; }
  
    }

    public class ChargeCategoryStatusManagementModel
    {
        public string categoryId { get; set; }
        public string status { get; set; }
    }

    public class ChargeCategoryDetailModel
    {
        public string categoryId { get; set; }
        public string categoryName { get; set; }
        public string description { get; set; }
        public string isDefault { get; set; }
        public string agentType { get; set; }
        public string status { get; set; }
        public string creatorUsername { get; set; }
        public string creatorFullname { get; set; }
        public string creatorProfileImage { get; set; }
        public string createdDate { get; set; }
        public string createdDateUTC { get; set; }
        public string createdPlatform { get; set; }
        public string createdIP { get; set; }
    }
}