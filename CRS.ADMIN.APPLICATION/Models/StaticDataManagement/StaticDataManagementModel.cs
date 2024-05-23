using CRS.ADMIN.SHARED;
using System.Collections.Generic;

namespace CRS.ADMIN.APPLICATION.Models.StaticDataManagement
{
    public class StaticDataManagement
    {
        public List<StaticDataTypeModel> GetStaticDataTypeList { get; set; }
        public List<StaticDataModel> GetStaticDataList { get; set; }
        public ManageStaticDataType ManageStaticDataType { get; set; }
        public ManageStaticData ManageStaticData { get; set; }
    }
    public class StaticDataTypeModel
    {
        public string Id { get; set; }
        public string StaticDataType { get; set; }
        public string StaticDataName { get; set; }
        public string StaticDataDescription { get; set; }
        public string Status { get; set; }
    }
    public class ManageStaticDataType : StaticDataTypeModel
    {

    }
    public class StaticDataModel : StaticDataTypeModel
    {
        public string AdditionalValue1 { get; set; }
        public string AdditionalValue2 { get; set; }
        public string StaticDataLabel { get; set; }
    }
    public class ManageStaticData 
    {
        public string Id { get; set; }
        public string StaticDataType { get; set; }
        public string StaticDataLabel { get; set; }
        public string StaticDataDescription { get; set; }
        public string AdditionalValue1 { get; set; }
        public string AdditionalValue2 { get; set; }
        public string Status { get; set; }
    }
}