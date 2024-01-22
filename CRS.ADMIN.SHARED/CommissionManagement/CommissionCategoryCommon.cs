namespace CRS.ADMIN.SHARED.CommissionManagement
{
    public class ManageCommissionCategoryCommon : Common
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
    }

    public class CommissionAssignedClubsCommon
    {
        public string ClubName { get; set; }
        public string Logo { get; set; }
        public string Status { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
    }

    public class CommissionCategoryCommon
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
}
