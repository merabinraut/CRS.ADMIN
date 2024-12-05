using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.SHARED.PointsManagement
{
    public class PointsManagementCommon
    { 
        public List<PointsTansferReportCommon> PointsTansferReportList = new List<PointsTansferReportCommon>();
       
        public PointsTansferCommon ManagePointsTansfer { get; set; }
        public string UserType { get; set; }
        public string UserName { get; set; }
        public string TransferTypeId { get; set; }
        public string FromDate { get; set; }

        public string ToDate { get; set; }
    }
    public class PointsTansferCommon : PaginationResponseCommon
    {
        public string UserTypeId { get; set; }
        public string TransferType { get; set; }
        public string UserId { get; set; }
        public string Points { get; set; }
        public string Remarks { get; set; }

        public string Image { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string Status { get; set; }

        public string SpName { get; set; }
    }
    public class PointsTansferReportCommon:PaginationResponseCommon
    {
        public string TransactionId { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string UserType { get; set; }
        public string UserName { get; set; }
        public string FromUser { get; set; }
        public string ToUser { get; set; }
        public string Points { get; set; }
        public string Remarks { get; set; }
        public string TransferTypeId { get; set; }
        public string UserTypeId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public string Id { get; set; }


    }
    public class PointsRequestCommon
    {
        public string Points { get; set; }
        public string Remarks { get; set; }

        public string ActionIp { get; set; }
        public string ActionUser { get; set; }

    }
    public class PointsTansferRetriveDetailsCommon
    {
        public string TransactionId { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string UserType { get; set; }
        public string FromUser { get; set; }

        public string ToUser { get; set; }
        public string Points { get; set; }
        public string Remarks { get; set; }
        public string Image { get; set; }

        public string Id { get; set; }
    }

}
