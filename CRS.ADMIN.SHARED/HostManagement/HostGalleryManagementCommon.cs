namespace CRS.ADMIN.SHARED.HostManagement
{
    public class HostGalleryManagementCommon
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

    public class HostManageGalleryImageCommon : Common
    {
        public string GalleryId { get; set; }
        public string AgentId { get; set; }
        public string HostId { get; set; }
        public string ImageTitle { get; set; }
        public string ImagePath { get; set; }
    }
}
