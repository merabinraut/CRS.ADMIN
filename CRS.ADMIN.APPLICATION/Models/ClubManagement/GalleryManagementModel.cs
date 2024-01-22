using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.APPLICATION.Models.ClubManagement
{
    public class CommonGalleryManagementModel
    {
        public List<GalleryManagementModel> GalleryManagementModel { get; set; }
        public ManageGalleryImageModel ManageGalleryImageModel { get; set; }
    }
    public class GalleryManagementModel
    {
        public string GalleryId { get; set; }
        public string AgentId { get; set; }
        public string ImageTitle { get; set; }
        public string ImagePath { get; set; }
        public string Status { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
    }

    public class ManageGalleryImageModel
    {
        public string GalleryId { get; set; }
        public string AgentId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MinLength(1, ErrorMessage = "Minimum 1 characters required")]
        public string ImageTitle { get; set; }
        public string ImagePath { get; set; }
    }
}