
using CRS.ADMIN.APPLICATION.Models.ClubManagement;
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
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string CategoryName { get; set; }
        public string CategoryId { get; set; }
        public string CategoryDescription { get; set; }
        public string RoleTypeId { get; set; }
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
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string FromAmount { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string ToAmount { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string PointType { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string PointValue { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string Status { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public string PointType2 { get; set; }
        public string PointValue2 { get; set; }
        public string MinValue2 { get; set; }
        public string MaxValue2 { get; set; }
        public string PointType3 { get; set; }
        public string PointValue3 { get; set; }
        public string MinValue3 { get; set; }
        public string MaxValue3 { get; set; }
    }
}