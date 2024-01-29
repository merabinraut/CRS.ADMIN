namespace CRS.ADMIN.SHARED.PaginationManagement
{
    public class PaginationFilterCommon
    {
        public string SearchFilter { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }

    public class PaginationResponseCommon : Common
    {
        public int SNO { get; set; } = 0;
        public int TotalRecords { get; set; } = 0;
    }
}
