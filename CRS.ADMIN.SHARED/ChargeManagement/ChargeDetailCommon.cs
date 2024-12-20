using CRS.ADMIN.SHARED.PaginationManagement;

namespace CRS.ADMIN.SHARED.ChargeManagement
{
    public class ChargeManagementCommon : Common
    {
        public string categoryId { get; set; }
        public string categoryDetailId { get; set; }
        public int fromAmount { get; set; }
        public int toAmount { get; set; }
        public string chargeType { get; set; }
        public int chargeValue { get; set; }
        public int minChargeValue { get; set; }
        public int maxChargeValue { get; set; }
        public string agentType { get; set; }
        public string sp { get; set; }
    }

    public class ChargeStatusManagementCommon : Common
    {
        public string categoryId { get; set; }
        public string categoryDetailId { get; set; }
        public string status { get; set; }
    }

    public class ChargeDetailCommon: PaginationResponseCommon
    {
        public string categoryDetailId { get; set; }
        public string categoryId { get; set; }
        public int fromAmount { get; set; }
        public int toAmount { get; set; }
        public string chargeType { get; set; }
        public int chargeValue { get; set; }
        public int minChargeValue { get; set; }
        public int maxChargeValue { get; set; }
        public string status { get; set; }
        //public string createdBy { get; set; }
        //public string createdDate { get; set; }
        //public string createdDateUTC { get; set; }
        //public string createdPlatform { get; set; }
        //public string createdIP { get; set; }
        //public string updatedBy { get; set; }
        //public string updatedDate { get; set; }
        //public string updatedDateUTC { get; set; }
        //public string updatedPlatform { get; set; }
        //public string updatedIP { get; set; }
    }
}
