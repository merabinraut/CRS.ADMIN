using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.APPLICATION.Models.PointsManagement
{
    public class PointsManagementModel
    {
        public List<PointsTansferReportModel> PointsTansferReportList = new List<PointsTansferReportModel>();
        public PointsTansferModel ManagePointsTansfer { get; set; }
        public PointsRequestModel ManagePointsRequest { get; set; }
        public string ListType { get; set; }
        public string SearchFilter { get; set; }
        public string UserType { get; set; }
        public string UserName { get; set; }
        public string TransferTypeId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

    }
    public class PointsTansferModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string UserTypeId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string TransferType { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string UserId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The field must be a number.")]
        public string Points { get; set; }
        public string Remarks { get; set; }
        public string Image { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string Status { get; set; }
        public string Id { get; set; }
    }
    public class PointsTansferReportModel
    {
        public string TransactionId { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string UserType { get; set; }
        public string FromUser { get; set; }
        public string ToUser { get; set; }
        public string Points { get; set; }
        public string Remarks { get; set; }
        public string TransferTypeId { get; set; }
        public string UserTypeId { get; set; }
        

    }
    public class PointsRequestModel
    {
        public string TransactionId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The field must be a number.")]
        public string Points { get; set; }
        public string Remarks { get; set; }
        public string ActionIp { get; set; }
        public string ActionUser { get; set; }

    }
}