using CRS.ADMIN.APPLICATION.Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.APPLICATION.Models.StaticDataManagement
{
    public class StaticDataManagement
    {
        public string SearchFilter { get; set; }
        public List<StaticDataTypeModel> GetStaticDataTypeList { get; set; }
        public List<StaticDataModel> GetStaticDataList { get; set; }
        public ManageStaticDataType ManageStaticDataType { get; set; }
        public ManageStaticData ManageStaticData { get; set; }
    }
    public class StaticDataTypeModel
    {
        public string SNO { get; set; }
        public string Id { get; set; }
        public string StaticDataType { get; set; }
        public string StaticDataName { get; set; }
        public string StaticDataDescription { get; set; }
        public string Status { get; set; }
    }
    public class ManageStaticDataType
    {
        public string SNO { get; set; }
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string StaticDataType { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string StaticDataName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string StaticDataDescription { get; set; }
        public string Status { get; set; }
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
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string StaticDataType { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string StaticDataLabel { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string StaticDataDescription { get; set; }
        public string AdditionalValue1 { get; set; }
        public string AdditionalValue2 { get; set; }
        public string Status { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string StaticDataValue { get; set; }
        public string AdditionalValue3 { get; set; }
        public string AdditionalValue4 { get; set; }
    }
}