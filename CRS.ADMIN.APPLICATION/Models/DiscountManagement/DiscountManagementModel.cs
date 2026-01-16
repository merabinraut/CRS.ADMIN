
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Models.DiscountManagement
{
    public class DiscountManagementModel
    {
        public string categoryId { get; set; }
        public string categoryName {  get; set; }
        public string description { get; set; }       
    }

    public class DiscountManagementListResponseModel 
    {
        public string ListType { get; set; }
        public string TabValue { get; set; }
        public DiscountManagementModel discountCategoryModel =new DiscountManagementModel();
        public List<DiscountManagementResponseModel> listDiscountManagement =new List<DiscountManagementResponseModel>();
        public List<DiscountCategoryDetailsResponseModel> listDiscountSubCategoryManagement =new List<DiscountCategoryDetailsResponseModel>();
        public DiscountSubCategoryManagementRequestModel discountSubCategoryModel = new DiscountSubCategoryManagementRequestModel();
        public AssignDiscountModel assignDiscountModel = new AssignDiscountModel();
    }

    public class DiscountManagementResponseModel
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public int TotalRecords { get; set; }
        public int SNO { get; set; }
    }


    public class DiscountCategoryDetailsResponseModel
    {
        public string SubCategoryId { get; set; }
        public string CategoryId { get; set; }
        public string categoryName { get; set; }
        public long FromAmount { get; set; }
        public long ToAmount { get; set; }
        public string DiscountType { get; set; }
        public int Value { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public string Status { get; set; }
        public string createdDate { get; set; }
        public string updatedDate { get; set; }
        public string SNO { get; set; }
    }
    public class DiscountSubCategoryManagementRequestModel
    {
       
        public string categoryId { get; set; }
        public string subCategoryId { get; set; }
        [Required]
        public long? FromAmount { get; set; }
        [Required]
        public long? ToAmount { get; set; }
        [Required]
        public string DiscountType { get; set; }
        [Required]
        public int? Value { get; set; }
        public long? MinValue { get; set; }
        public long? MaxValue { get; set; }
        public string categoryName { get; set; }
        public List<SelectListItem> discountTypeList { get; set; } = new List<SelectListItem>();


    }

    public class DiscountSubCategoryRequestModel
    {
        public string categoryId { get; set; }
        public string categoryName { get; set; }
        public string subCategoryId { get; set; }
    }
    public class AssignDiscountModel
    {
        [Required]
        [Display(Name = "Club Name")]
        public string AgentId { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }

        [Display(Name = "Current Category Name")]
        public string CurrentCategory { get; set; }
        public string NewDiscountCategoryDDL { get; set; }
        public string LocationDDL1 { get; set; }
        public string ClubDDLList { get; set; }
    }
}