using CRS.ADMIN.APPLICATION.Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.APPLICATION.Models.ChargeManagement
{
    public class ChargeCategoryDetailsModel
    {
        public string categoryId { get; set; }
        public string SearchFilter { get; set; }
        public string categoryName { get; set; }
        public string agentTypeValue { get; set; }
        public ManageChargeDetailsModel ManageChargeDetails { get; set; }
        public  List<ChargeManagementModel>  ChargeCategoryDetailsList{ get; set; }
    }
    public class ChargeManagementModel
    {
        public string categoryId { get; set; }
        public string categoryDetailId { get; set; }
        public int fromAmount { get; set; }
        public int toAmount { get; set; }
        public string chargeType { get; set; }
        public int chargeValue { get; set; }
        public int minChargeValue { get; set; }
        public int maxChargeValue { get; set; }
        public string agentType { get; set; } 
        public string status { get; set; }
        public string SNO { get; set; }
        public string RowsTotal { get; set; }
    }
    public class ManageChargeDetailsModel
    {
        public string categoryId { get; set; }
        public string agentTypeValue { get; set; }
        public string categoryDetailId { get; set; }
        public string categoryName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_number")]
        public int fromAmount { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_number")]
        public int toAmount { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string chargeType { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_number")]
        public int chargeValue { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_number")]
        public int minChargeValue { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_number")]
        public int maxChargeValue { get; set; }
        public string agentType { get; set; }
       
    }
    public class ChargeStatusManagementModel
    {
        public string categoryId { get; set; }
        public string categoryDetailId { get; set; }
        public string status { get; set; }
    }

    public class ChargeDetailModel
    {
        public string categoryDetailId { get; set; }
        public string categoryId { get; set; }
        public int fromAmount { get; set; }
        public int toAmount { get; set; }
        public string chargeType { get; set; }
        public int chargeValue { get; set; }
        public int minChargeValue { get; set; }
        public int maxChargeValue { get; set; }
        public string status { get; set; }
        public string createdBy { get; set; }
        public string createdDate { get; set; }
        public string createdDateUTC { get; set; }
        public string createdPlatform { get; set; }
        public string createdIP { get; set; }
        public string updatedBy { get; set; }
        public string updatedDate { get; set; }
        public string updatedDateUTC { get; set; }
        public string updatedPlatform { get; set; }
        public string updatedIP { get; set; }
    }
}