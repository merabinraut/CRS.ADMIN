﻿using CRS.ADMIN.APPLICATION.Resources;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.APPLICATION.Models.CommissionManagement
{
    public class ManageCommissionCategoryModel
    {
        [DisplayName("Category Name")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(30, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Maximum_50_characters_allowed")]
        public string CategoryName { get; set; }
        [DisplayName("Description")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(30, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Maximum_300_characters_allowed")]
        public string Description { get; set; }
        public string CategoryId { get; set; }
    }

    public class CommissionAssignedClubsModel
    {
        public string ClubName { get; set; }
        public string Logo { get; set; }
        public string Status { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
    }

    public class CommissionCategoryModel
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedByFullName { get; set; }
        public string CreatedByUsername { get; set; }
        public string CreatedByImage { get; set; }

    }

    public class CommissionCategoryRazorViewModel
    {
        public List<CommissionCategoryModel> CommissionCategoryModelGrid { get; set; } = new List<CommissionCategoryModel>();
        public ManageCommissionCategoryModel AddEditCommissionCategory { get; set; } = new ManageCommissionCategoryModel();
    }
}