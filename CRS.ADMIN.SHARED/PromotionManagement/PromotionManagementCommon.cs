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
}