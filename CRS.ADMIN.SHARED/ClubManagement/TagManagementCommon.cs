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
        public string AvailableForPrivateBooking { get; set; }
        public string LargeStaffPresence { get; set; }
        public string VIPRoomAvailable { get; set; }
        public string SuitableForGirlsNgtOut { get; set; }
    }
    public class LocationListCommon : Common
    {
        public string LocationID { get; set; }
        public string LocationName { get; set; }

    }
}
