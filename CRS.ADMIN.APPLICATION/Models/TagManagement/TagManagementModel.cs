using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CRS.ADMIN.APPLICATION.Models.TagManagement
{
    public class TagManagementModel
    {
        public string Sno { get; set; }
        public string TagId { get; set; }
        public string ClubId { get; set; }
        public string Tag1Location { get; set; }
        public string Tag1Status { get; set; }
        public string Tag2RankName { get; set; }
        public string Tag2Description { get; set; }
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
        public string ActionUser { get; set; }
        public string ActionIp { get; set; }
        public string ActionDate { get; set; }
    }
    public class ManageTag
    {
        public string TagId { get; set; }
        public string ClubId { get; set; }
        [DisplayName("Location Name")]
        public string Tag1Location { get; set; }
        [DisplayName("Status")]
        public string Tag1Status { get; set; }
        [DisplayName("Rank Name")]
        public string Tag2RankName { get; set; }
        [DisplayName("Rank Description")]
        public string Tag2RankDescription { get; set; }
        [DisplayName("Status")]
        public string Tag2Status { get; set; }
        [DisplayName("Category Name")]
        public string Tag3CategoryName { get; set; }
        [DisplayName("Category Description")]
        public string Tag3CategoryDescription { get; set; }
        [DisplayName("Status")]
        public string Tag3Status { get; set; }
        [DisplayName("Excellency Name")]
        public string Tag4ExcellencyName { get; set; }
        [DisplayName("Excellency Description")]
        public string Tag4ExcellencyDescription { get; set; }
        [DisplayName("Status")]
        public string Tag4Status { get; set; }
        [DisplayName("Store Name")]
        public string Tag5StoreName { get; set; }
        [DisplayName("Store Description")]
        public string Tag5StoreDescription { get; set; }
        [DisplayName("Status")]
        public string Tag5Status { get; set; }
    }
    public class LocationDDL
    {
        public string LocationID { get; set; }
        public string LocationName { get; set; }
    }
}