using CRS.ADMIN.SHARED.PaginationManagement;

namespace CRS.ADMIN.SHARED.PromotionManagement
{
    public class PromotionManagementCommon : PaginationResponseCommon
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string IsDeleted { get; set; }

    }
    public class AdvertisementManagementCommon : PaginationResponseCommon
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string Status { get; set; }
        public string IsDeleted { get; set; }
        public string Link { get; set; }
        public string DisplayOrder { get; set; }

    }
    public class AdvertisementDetailCommon : Common
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string IsDeleted { get; set; }
        public string Link { get; set; }
        public string DisplayOrder { get; set; }
        public string Status { get; set; }
    }

}