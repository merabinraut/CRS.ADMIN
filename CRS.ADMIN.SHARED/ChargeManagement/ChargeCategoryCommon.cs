using CRS.ADMIN.SHARED.PaginationManagement;

namespace CRS.ADMIN.SHARED.ChargeManagement
{
    public class ChargeCategoryManagementCommon : Common
    {
        public string categoryId { get; set; }
        public string sp { get; set; }
        public string categoryName { get; set; }
        public string description { get; set; }
        public string isDefault { get; set; }
        public string agentType { get; set; }
        public string agentTypeValue { get; set; }
    }

    public class ChargeCategoryStatusManagementCommon : Common
    {
        public string categoryId { get; set; }
        public string status { get; set; }
    }

    public class ChargeCategoryDetailCommon: PaginationResponseCommon
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
        public string agentTypeValue { get; set; }
        public string isUserCandeleteCategory { get; set; }
    }
}
