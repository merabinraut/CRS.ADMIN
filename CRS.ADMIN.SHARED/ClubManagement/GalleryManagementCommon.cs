namespace CRS.ADMIN.SHARED.ClubManagement
{
    public class GalleryManagementCommon
    {
        public string GalleryId { get; set; }
        public string AgentId { get; set; }
        public string ImageTitle { get; set; }
        public string ImagePath { get; set; }
        public string Status { get; set; }
        public string UpdatedDate { get; set; }
        public string CreatedDate { get; set; }
        public int SNO { get; set; }
        public int TotalRecords { get; set; }
    }

    public class ManageGalleryImageCommon : Common
    {
        public string GalleryId { get; set; }
        public string AgentId { get; set; }
        public string ImageTitle { get; set; }
        public string ImagePath { get; set; }
    }
}
