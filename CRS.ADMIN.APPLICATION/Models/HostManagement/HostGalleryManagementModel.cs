using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.APPLICATION.Models.HostManagement
{
    public class CommonHostGalleryManagementModel
    {
        public List<HostGalleryManagementModel> HostGalleryManagementModel { get; set; }
        public HostManageGalleryImageModel HostManageGalleryImageModel { get; set; }
    }
    public class HostGalleryManagementModel
    {
        public string GalleryId { get; set; }
        public string AgentId { get; set; }
        public string HostId { get; set; }
        public string ImageTitle { get; set; }
        public string ImagePath { get; set; }
        public string Status { get; set; }
        public string UpdatedDate { get; set; }
        public string CreatedDate { get; set; }
    }

    public class HostManageGalleryImageModel
    {
        public string GalleryId { get; set; }
        public string AgentId { get; set; }
        public string HostId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MinLength(1, ErrorMessage = "Minimum 1 characters required")]
        public string ImageTitle { get; set; }
        public string ImagePath { get; set; }
    }
}