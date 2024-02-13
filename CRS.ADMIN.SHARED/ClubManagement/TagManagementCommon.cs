namespace CRS.ADMIN.SHARED.ClubManagement
{
    public class ManageTagCommon : Common
    {
        public string Sno { get; set; }
        public string TagId { get; set; }
        public string ClubId { get; set; }
        public string Tag1Location { get; set; }
        public string Tag1Status { get; set; }
        public string Tag2RankName { get; set; }
        public string Tag2RankDescription { get; set; }
        public string Tag2Status { get; set; }
        public string Tag3CategoryName { get; set; }
        public string Tag3CategoryDescription { get; set; }
        public string Tag3Status { get; set; }
        public string Tag4ExcellencyName { get; set; }
        public string Tag4ExcellencyDescription { get; set; }
        public string Tag4Status { get; set; }
        public string Tag5StoreName { get; set; }
        public string Tag5StoreDescription { get; set; }
        public string Tag5Status { get; set; }
        public string StaticType { get; set; }
        public string StaticLabel { get; set; }
        public string StaticVaue { get; set; }
        public string StaticDescription { get; set; }
        public string StaticStatus { get; set; }
        public string StaticLabelJapanese { get; set; }
    }
    public class LocationListCommon : Common
    {
        public string LocationID { get; set; }
        public string LocationName { get; set; }

    }
    public class AvailabilityTagModelCommon
    {
        #region "Club Availability"
        public string StaticType { get; set; }
        public string StaticLabel { get; set; }
        public string StaticVaue { get; set; }
        public string StaticDescription { get; set; }
        public string StaticStatus { get; set; }
        public string StaticLabelJapanese { get; set; }
        #endregion
    }
}
