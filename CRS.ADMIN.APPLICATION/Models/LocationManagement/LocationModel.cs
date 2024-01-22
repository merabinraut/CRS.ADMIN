using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.APPLICATION.Models.LocationManagement
{
    public class LocationManagementModel
    {
        public string SearchFilter { get; set; } = string.Empty;
        public List<LocationListModel> LocationListModel { get; set; } = new List<LocationListModel>();
        public LocationModel LocationModel { get; set; } = new LocationModel();
    }
    public class LocationListModel
    {
        public string LocationId { get; set; }
        public string LocationName { get; set; }
        public string LocationImage { get; set; }
        public string TotalClubs { get; set; }
        public string LocationStatus { get; set; }
    }

    public class LocationModel
    {
        public string LocationId { get; set; }
        [DisplayName("Location Title")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Location title is required")]
        public string LocationName { get; set; }
        public string LocationImage { get; set; }
        [DisplayName("Map Link")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Map Link is required")]
        public string LocationURL { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Longitude is required")]
        public string Longitude { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Latitude is required")]
        public string Latitude { get; set; }
    }
}