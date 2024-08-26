using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.APPLICATION.Models.Inquiries
{
    public class InquiriesModel
    {
         public string SNO { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string Posted_Date { get; set; }
        public string Action { get; set; }
    }
    public class InquiryListModel
    {
        public string Sno { get; set; }
        public string EmailAddress { get; set; }
        public string Message { get; set; }
        public string ActionDate { get; set; }
        public string ActionBy { get; set; }
    }
}
