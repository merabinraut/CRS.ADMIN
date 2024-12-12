using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.SHARED.WithdrawalRequest
{
    public class WithdrawalCommon: PaginationResponseCommon
    {
        public string formattedDate { get; set; }
        public string withdrawDay { get; set; }
        public string totalAmount { get; set; }
        
    }
    public class WithdrawalFilterCommon: PaginationFilterCommon
    {
        public string id { get; set; }
        public string bankName { get; set; }
        public string bankType { get; set; }
        public string branchName { get; set; }
        public string affiliateInfo { get; set; }

    }
    public class WithdrawalMonthlyDetailsCommon : PaginationResponseCommon
    {
        public string id { get; set; }
        public string requestId { get; set; }         
        public string name { get; set; }
        public string requestedDate { get; set; }
        public string affiliateInfo { get; set; }
        public string users { get; set; }
        public string emailAddress { get; set; }
        public string phoneNumber { get; set; }
        public string bankName { get; set; }
        public string bankType { get; set; }
        public string branchName { get; set; }
        public string accountSymbol { get; set; }
        public string bankAccountNumber { get; set; }
        public string bankAccountName { get; set; }
        public string requestedAmount { get; set; }
        public string chargeAmount { get; set; }
        public string transferAmount { get; set; }
        public string status { get; set; }
    }

}
