using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.SHARED.AccountInformation
{
    public class AccountInformationCommon
    {
    }
    public class AccountInformationFilterCommon
    {
        public string searchFilter { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public int StartIndex { get; set; }
        public int PageSize { get; set; }
    }
    public class AccountInformationCommonResponse : PaginationResponseCommon
    {
        public string bankType { get; set; }
        public string bankName { get; set; }
        public string branchName { get; set; }
        public string accountType { get; set; }
        public string accountNumber { get; set; }
        public string accountHolderName { get; set; }
        public string affiliateId { get; set; }
        public string accountSymbol { get; set; }
        public string roleType { get; set; }
        public string status { get; set; }
        public string updatedBy { get; set; }
        public string updatedDate { get; set; }
        public string createdDate { get; set; }
        public string createdBy { get; set; }
        public string AffiliateName { get; set; }
        public string AffiliateNameEng { get; set; }
        public string MobileNumber { get; set; }
    }
}
