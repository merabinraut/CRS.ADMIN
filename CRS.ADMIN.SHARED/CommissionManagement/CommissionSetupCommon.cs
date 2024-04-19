namespace CRS.ADMIN.SHARED.CommissionManagement
{
    public class CommissionDetailCommon
    {
        public string CategoryId { get; set; }
        public string AdminCommissionTypeId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDetailId { get; set; }
        public string FromAmount { get; set; }
        public string ToAmount { get; set; }
        public string CommissionType { get; set; }
        public string CommissionValue { get; set; }
        public string CommissionPercentageType { get; set; }
        public string MinCommissionValue { get; set; }
        public string MaxCommissionValue { get; set; }
    }

    public class ManageCommissionDetailCommon : Common
    {
        public string CategoryId { get; set; }
        public string AdminCommissionTypeId { get; set; }
        public string CategoryDetailId { get; set; }
        public string FromAmount { get; set; }
        public string ToAmount { get; set; }
        public string CommissionType { get; set; }
        public string CommissionValue { get; set; }
        public string CommissionPercentageType { get; set; }
        public string MinCommissionValue { get; set; }
        public string MaxCommissionValue { get; set; }
    }
}
