
using CRS.ADMIN.APPLICATION.Models.ClubManagement;
using CRS.ADMIN.APPLICATION.Resources;
using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace CRS.ADMIN.APPLICATION.Models.PointSetup
{
    public class PointSetupModel
    {
        public string SearchFilter { get; set; }
        public string UserTypeId { get; set; }
        public string AgentId { get; set; }
        public string CurrentCategoryId { get; set; }
        public string NewCategoryId { get; set; }
        public List<CategoryModel> CategoryList = new List<CategoryModel>();
        public CategoryModel ManageCategory { get; set; }
        public List<UserTypeModel>  UserTypeList = new List<UserTypeModel>();
        public List<CategorySlabModel> CategorySlabList = new List<CategorySlabModel>();
        public CategorySlabModel ManageCategorySlab { get; set; }
        

    }

    public class UserTypeModel
    {
        public string SNO { get; set; }
        public string RoleTypeId { get; set; }       
        public string RoleTypeName { get; set; }
    }
    public class CategoryModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string CategoryName { get; set; }
        public string CategoryId { get; set; }
        public string CategoryDescription { get; set; }
        public string RoleTypeId { get; set; }
        public string RoleName { get; set; }         
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string Status { get; set; }
    }
    public class CategorySlabModel
    {
        public string CategoryName { get; set; }
        public string CategoryId { get; set; }
        public string CategorySlabId { get; set; }
        public string RoleTypeId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_number")]
        public string FromAmount { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_number")]
        public string ToAmount { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string PointType { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"^(?:[0-9]|[1-9][0-9]|1[0-4][0-9]|150)$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_number_between_0_and_150")]
        public string PointValue { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string Status { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_number")]
        public string MinValue { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_number")]
        public string MaxValue { get; set; }
        public string PointType2 { get; set; }
        [RegularExpression(@"^(?:[0-9]|[1-9][0-9]|1[0-4][0-9]|150)$", ErrorMessage = "The_field_must_be_a_number_between_0_and_150")]
        public string PointValue2 { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_number")]
        public string MinValue2 { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_number")]
        public string MaxValue2 { get; set; }
        public string PointType3 { get; set; }
        [RegularExpression(@"^(?:[0-9]|[1-9][0-9]|1[0-4][0-9]|150)$", ErrorMessage = "The_field_must_be_a_number_between_0_and_150")]
        public string PointValue3 { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_number")]
        public string MinValue3 { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "The_field_must_be_a_number")]
        public string MaxValue3 { get; set; }
    }
}