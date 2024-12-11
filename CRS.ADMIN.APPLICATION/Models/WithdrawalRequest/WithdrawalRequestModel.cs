
using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CRS.ADMIN.APPLICATION.Models.WithdrawalRequest
{
    public class WithdrawalRequestModel: PaginationResponseCommon
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public List<WithdrawalMonthlyList> WithdrawalMonthlyList { get; set; }=new List<WithdrawalMonthlyList>();        

    }
    public class WithdrawalMonthlyList : PaginationResponseCommon
    {
        public string formattedDate { get; set; }
        public string withdrawDay { get; set; }
        public string totalAmount { get; set; }
    }
    public class WithdrawalDetailsModel
    {
        public string FromDate { get; set; } 
        public string SearchFilter { get; set; }
        public string ToDate { get; set; }
        public string bankName { get; set; }
        public string bankType { get; set; }
        public string branchName { get; set; }
        public string affiliateInfo { get; set; }
        public string id { get; set; }      
        public List<WithdrawalMonthlyDetailsModel> WithdrawalMonthlyList { get; set; } = new List<WithdrawalMonthlyDetailsModel>();

    }
    public class WithdrawalMonthlyDetailsModel: PaginationResponseCommon
    {
        public string id { get; set; }
        public string requestId { get; set; }
        public string requestedDate { get; set; }
        public string affiliateInfo { get; set; }
        public string users { get; set; }
        public string emailAddress { get; set; }
        public string phoneNumber { get; set; }
        public string bankName { get; set; }
        public string bankType { get; set; }
        public string branchName { get; set; }
        public string bankAccountNumber { get; set; }
        public string bankAccountName { get; set; }
        public string requestedAmount { get; set; }
        public string chargeAmount { get; set; }
        public string transferAmount { get; set; }
        public string status { get; set; }
    }
    public class ManageWithdrawalModel
    {
        public string id { get; set; }
        public string sno { get; set; }
      
    }
}