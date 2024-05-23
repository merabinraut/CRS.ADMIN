namespace CRS.ADMIN.SHARED.StaticDataManagement
{
    public class StaticDataTypeCommon
    {
        public string Id { get; set; }
        public string StaticDataType { get; set; }
        public string StaticDataName { get; set; }
        public string StaticDataDescription { get; set; }
        public string Status { get; set; }
    }
    public class ManageStaticDataTypeCommon : Common
    {
        public string Id { get; set; }
        public string StaticDataType { get; set; }
        public string StaticDataName { get; set; }
        public string StaticDataDescription { get; set; }
        public string Status { get; set; }
    }
    public class StaticDataModelCommon : StaticDataTypeCommon
    {
        public string AdditionalValue1 { get; set; }
        public string AdditionalValue2 { get; set; }
        public string StaticDataLabel { get; set; }
    }
    public class ManageStaticDataCommon : Common
    {
        public string Id { get; set; }
        public string StaticDataType { get; set; }
        public string StaticDataLabel { get; set; }
        public string StaticDataDescription { get; set; }
        public string Status { get; set; }
    }

}
