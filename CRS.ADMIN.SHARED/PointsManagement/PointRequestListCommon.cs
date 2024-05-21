using CRS.ADMIN.SHARED.PaginationManagement;

namespace CRS.ADMIN.SHARED.PointsManagement
{
    public class PointRequestListFilterCommon
    {
        public string SearchFilter { get; set; }
        public string LocationId { get; set; }
        public string ClubName { get; set; }
        public string PaymentMethodId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 10;
    }
    public class PointRequestListCommon
    {
        public string AgentId { get; set; }
        public string UserId { get; set; }
        public string RequestId { get; set; }
        public string RequestedDate { get; set; }
        public string ClubName { get; set; }
        public string PaymentMethod { get; set; }
        public string AmountTransferred { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string RowsTotal { get; set; }
    }

    public class ManageClubPointRequestCommon : Common
    {
        public string AgentId { get; set; }
        public string UserId { get; set; }
        public string TxnId { get; set; }
        public string Status { get; set; }
        public string AdminRemark { get; set; }
        public string ImageURL { get; set; }
    }

    public class PointBalanceStatement:Common
    {

        public string SearchFilter { get; set; }
        public string UserType { get; set; }
        public string User { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
    public class PointBalanceStatementRequestCommon
    {
        public string SearchFilter { get; set; }
        public string UserTypeList { get; set; }
        public string UserNameList { get; set; }
        public string TransferTypeList { get; set; }
        public string From_Date { get; set; }
        public string To_Date { get; set; }
        public string userid { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 10;
    }
    public class PointBalanceStatementResponseCommon : Common
    {
       public int SNO {  get; set; }
        public string TransactionId { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string User { get; set; }
        public string TotalPrice { get; set; }
        public string TotalCommission { get; set; }
        public string Remarks { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public string RowTotal { get; set; }

    }

    public class SystemTransferRequestCommon
    {
        public string SearchFilter { get; set; }
        public string UserType { get; set; }
        public string UserName { get; set; }
        public string TransferType { get; set; }
        public string From_Date1 { get; set; }
        public string To_Date1 { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 10;
    }
    public class SystemTransferReponseCommon
    {
        public int SNO { get; set; }
        public string TransactionId { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string UserType { get; set; }
        public string UserName { get; set; }
        public string Points { get; set; }
        public string Remarks { get; set; }
        public string RowTotal { get; set; }
    }




}
