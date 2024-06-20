using CRS.ADMIN.APPLICATION.Resources;
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
        public int SNO { get; set; }
        public int TotalRecords { get; set; }
    }

    public class ManageGalleryImageModel
    {
        public string GalleryId { get; set; }
        public string AgentId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MinLength(1, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Minimum_1_characters_required")]
        public string ImageTitle { get; set; }
        public string ImagePath { get; set; }
    }
}