using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.SHARED.TemplateManagement
{
    public class TemplateMessageCommon
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
    public class ManageTemplateCommon: Common
    {      
      public string contentCategoryDDL { get; set; }
      public string contentTypeDDL { get; set; }
      public string userTypeDDL { get; set; }
      public string subject { get; set; }
      public string messageBody { get; set; }
      public string Id { get; set; }
    }
}
