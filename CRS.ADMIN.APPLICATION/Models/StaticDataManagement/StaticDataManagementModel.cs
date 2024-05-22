using System.Collections.Generic;

namespace CRS.ADMIN.APPLICATION.Models.StaticDataManagement
{
    public class StaticDataManagement
    {
        public List<StaticDataManagementModel> GetStaticDataTypeList { get; set; }
        public ManageStaticDataType ManageStaticDataType { get; set; }
    }
    public class StaticDataManagementModel
    {
        public string Id { get; set; }
        public string StaticDataType { get; set; }
        public string StaticDataName { get; set; }
        public string StaticDataDescription { get; set; }
        public string Status { get; set; }
    }
    public class ManageStaticDataType: StaticDataManagementModel
    {

    }
}