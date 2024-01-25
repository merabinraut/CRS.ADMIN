using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.APPLICATION.Models.PromotionManagement
{
    public class PromotionManagementCommonModel
    {
        public string SearchFilter { get; set; }
        public List<PromotionManagementListModel> PromotionManagementListModel { get; set; } = new List<PromotionManagementListModel>();
        public PromotionManagementModel PromotionManagementModel { get; set; } = new PromotionManagementModel();
    }
    public class PromotionManagementModel
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Title is required")]
        public string Title { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Description is required")]
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
}