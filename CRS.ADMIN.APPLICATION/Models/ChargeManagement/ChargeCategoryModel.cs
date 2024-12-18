namespace CRS.ADMIN.APPLICATION.Models.ChargeManagement
{
    public class ChargeCategoryManagementModel
    {
        public string categoryId { get; set; }
        public string categoryName { get; set; }
        public string description { get; set; }
        public string isDefault { get; set; }
        public string agentType { get; set; }
    }

    public class ChargeCategoryStatusManagementModel
    {
        public string categoryId { get; set; }
        public string status { get; set; }
    }

    public class ChargeCategoryDetailModel
    {
        public string categoryId { get; set; }
        public string categoryName { get; set; }
        public string description { get; set; }
        public string isDefault { get; set; }
        public string agentType { get; set; }
        public string status { get; set; }
        public string creatorUsername { get; set; }
        public string creatorFullname { get; set; }
        public string creatorProfileImage { get; set; }
        public string createdDate { get; set; }
        public string createdDateUTC { get; set; }
        public string createdPlatform { get; set; }
        public string createdIP { get; set; }
    }
}