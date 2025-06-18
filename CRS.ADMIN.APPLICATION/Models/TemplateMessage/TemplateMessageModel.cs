using CRS.ADMIN.APPLICATION.Models.GroupManagement;
using CRS.ADMIN.APPLICATION.Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.APPLICATION.Models.TemplateMessage
{
    public class TemplateMessageModel
    {
        public string SearchFilter { get; set; }
        public ManageTemplateModel ManageTemplateModel { get; set; }
        public List<TemplateMessageListModel> GetTemplateMessageList { get; set; } = new List<TemplateMessageListModel>();
    }

    public class ManageTemplateModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string contentCategoryDDL { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string contentTypeDDL { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string userTypeDDL { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string subject { get; set; }
        public string fieldTypeDDL { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string messageBody { get; set; }
        public string Id { get; set; }

    }
    public class TemplateMessageListModel
    {
        public string contentCategory { get; set; }
        public int Sno { get; set; }
        public string Id { get; set; }
        public string userType { get; set; }
        public string contentType { get; set; }
        public string subject { get; set; }
        public string createdDate { get; set; }
        public string updatedDate { get; set; }
        public string messageBody { get; set; }
        public int TotalRecords { get; set; }
    }
}