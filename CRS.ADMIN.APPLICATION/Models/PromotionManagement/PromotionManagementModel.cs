using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CRS.ADMIN.APPLICATION.Resources;

namespace CRS.ADMIN.APPLICATION.Models.PromotionManagement
{
    public class PromotionManagementCommonModel
    {
        public string SearchFilter { get; set; }
        public string SearchFilter2 { get; set; }
        public List<PromotionManagementListModel> PromotionManagementListModel { get; set; } = new List<PromotionManagementListModel>();
        public List<AdvertisementManagementListModel> AdvertisementManagementListModel { get; set; } = new List<AdvertisementManagementListModel>();
        public PromotionManagementModel PromotionManagementModel { get; set; } = new PromotionManagementModel(); 
        public AdvertisementManagementModel AdvertisementManagementModel { get; set; } = new AdvertisementManagementModel();
    }
    public class PromotionManagementModel
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Title_is_required")]
        public string Title { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Description_is_required")]
        public string Description { get; set; }
        public string ImagePath { get; set; }
    }
    public class PromotionManagementListModel
    {
        public string SNO { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string IsDeleted { get; set; }
        public string ActionDate { get; set; }
    }
    public class AdvertisementManagementListModel
    {
        public string SNO { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string IsDeleted { get; set; }
        public string ActionDate { get; set; }
        public string Link { get; set; }
        public string DisplayOrder { get; set; }
    }
    public class AdvertisementManagementModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string IsDeleted { get; set; }
        public string Link { get; set; }
        public string DisplayOrder { get; set; }
    }
}