using CRS.ADMIN.SHARED.Middleware.AmazonCognitoModel;
using CRS.ADMIN.SHARED.PaginationManagement;
using System.Web;

namespace CRS.ADMIN.SHARED.PlanManagement
{
    public class PlanManagementCommon : PaginationResponseCommon
    {
        public string PlanId { get; set; }
        public string PlanName { get; set; }
        public string PlanType { get; set; }
        public string PlanTime { get; set; }
        public string Price { get; set; }
        public string Liquor { get; set; }
        public string Nomination { get; set; }
        public string Remarks { get; set; }
        public string PlanStatus { get; set; }
        public string PlanImage { get; set; }
        public string PlanImage2 { get; set; }
        public string ExtraField1 { get; set; }
        public string ExtraField2 { get; set; }
        public string ExtraField3 { get; set; }
        public string PlanCategory { get; set; }
        public int NoOfPeople { get; set; }
        public string StrikePrice { get; set; }
        public string IsStrikeOut { get; set; }
        public string PlanNameEnglish { get; set; }
    }
    public class PlanRequesResponseListCommon : PaginationResponseCommon
    {
        public int SNO { get; set; }
        public string clubName { get; set; }
        public string clubId { get; set; }
        public string planId { get; set; }
        public string plantype { get; set; }
        public string planTitle { get; set; }
        public string planTime { get; set; }
        public string planPrice { get; set; }
        public string numberOfPeople { get; set; }
        public string requestDate { get; set; }
        public string planStatus { get; set; }
        public string nomination { get; set; }
        public string lastEntryTime { get; set; }

    }

    public class PlanRequesRequestCommon
    {
        public string clubName { get; set; }
        public string clubId { get; set; }
        public string planId { get; set; }
        public string plantype { get; set; }
        public string planTitle { get; set; }
        public string planTime { get; set; }
        public string planPrice { get; set; }
        public string numberOfPeople { get; set; }
        public string requestDate { get; set; }
        public string planStatus { get; set; }
        public string nomination { get; set; }

    }
    public class EntryTimeInterval
    {
        public string entryTime { get; set; }
    }

}