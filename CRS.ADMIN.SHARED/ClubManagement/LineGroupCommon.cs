using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.SHARED.ClubManagement
{
    public class LineGroupCommon
    {
        public string clubId { get; set; }
        public string groupId { get; set; }
        public string groupName { get; set; }
        public string qrImage { get; set; }
        public string link { get; set; }
    }
    public class SubDomainCommon 
    {
        public string clubId { get; set; }
        public string clubCode { get; set; }
        public string SubDomainName { get; set; }
        public string SubDomainUrl { get; set; }
        public string Description { get; set; }
        public string SearchFilter { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string mobileNumber { get; set; }
        public string cognitoUserId { get; set; }
        public int StartIndex { get; set; }
        public int PageSize { get; set; }
        public string code { get; set; }

    }


    public class subDomainResponseCommon : CommonDbResponse
    {
        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public string Extra3 { get; set; }
        public string cognitoUserId { get; set; }

    }
}
