using CRS.ADMIN.APPLICATION.Resources;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.APPLICATION.Models.ApiResponseMessage
{
    public class ApiResponseModel
    {
        public string SearchFilter { get; set; }
        public string moduleName { get; set; }
        public ApiResponseMessageModel ManageResponse { get; set; }
        public List<ApiResponseMessageModel> ApiResponseMessageList { get; set; }
    }

    public class ApiResponseMessageModel
    {
        public string SearchFilter { get; set; }
        public string SearchFilterPending { get; set; }
        public string SearchFilterReject { get; set; }
        public string MessageId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string Code { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string Message { get; set; }       
        public string MessageEng { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string Category { get; set; }      
        public string Description { get; set; }

        public string MessageType { get; set; }
        public string Module { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string UserCategory { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string HttpStatusCode { get; set; }
        public string ActionUser { get; set; }
        public int SNO { get; set; }
        public int StartIndex { get; set; }
        public int PageSize { get; set; }
        public string UserCategoryFilter { get; set; }
        public string CategoryFilter { get; set; }
        public string ModuleNameFilter { get; set; }
        public bool IsVariableExists { get; set; }


    }
}