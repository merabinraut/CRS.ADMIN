using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.APPLICATION.Models.HostManagement
{
    public class ManageHostCommonModel
    {
        public List<HostListModel> HostListModel { get; set; }
        public ManageHostModel ManageHostModel { get; set; }
    }
    public class HostListModel
    {
        public string SNO { get; set; }
        public string AgentId { get; set; }
        public string HostId { get; set; }
        [Display(Name = "Host Name")]
        public string HostName { get; set; }
        [Display(Name = "Position")]
        public string Position { get; set; }
        [Display(Name = "Rank")]
        public string Rank { get; set; }
        [Display(Name = "Age")]
        public string Age { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public string Action { get; set; }
        public string ClubName { get; set; }
        public string Ratings { get; set; }
        public string TotalVisitors { get; set; }
        public string HostImage { get; set; }
        public string Height { get; set; }
        public string Address { get; set; }
    }

    public class ManageHostModel
    {
        public string AgentId { get; set; }
        public string HostId { get; set; }
        [Display(Name = "Host Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MaxLength(100, ErrorMessage = "Maximum 100 characters allowed")]
        public string HostName { get; set; }
        [Display(Name = "Host Name Japanese")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MaxLength(100, ErrorMessage = "Maximum 100 characters allowed")]
        public string HostNameJapanese { get; set; }
        [Display(Name = "Host Introduction")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MaxLength(500, ErrorMessage = "Maximum 500 characters allowed")]
        public string HostIntroduction { get; set; }
        [Display(Name = "Position")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [MaxLength(50, ErrorMessage = "Maximum 50 characters allowed")]
        public string Position { get; set; }
        [Display(Name = "Rank")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        //[MaxLength(50, ErrorMessage = "Maximum 50 characters allowed")]
        public string Rank { get; set; }
        [Display(Name = "Date of Birth")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string DOB { get; set; }
        [Display(Name = "Constellation Group")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string ConstellationGroup { get; set; }
        [Display(Name = "Height")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        //[MaxLength(5, ErrorMessage = "Maximum 5 digit number allowed")]
        public string Height { get; set; }
        [Display(Name = "Blood Type")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string BloodType { get; set; }
        [Display(Name = "Previous Occupation")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string PreviousOccupation { get; set; }
        [Display(Name = "Liquor Strength")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string LiquorStrength { get; set; }
        [DisplayName("Website")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string WebsiteLink { get; set; }
        [DisplayName("Tiktok")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string TiktokLink { get; set; }
        [DisplayName("Twitter")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string TwitterLink { get; set; }
        [DisplayName("Instagram")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string InstagramLink { get; set; }
        [DisplayName("Line")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string Line { get; set; }
        public string BirthYear { get; set; }
        public string BirthMonth { get; set; }
        public string BirthDate { get; set; }
        public string HostLogo { get; set; }
        public string HostIconImage { get; set; }
        public string Address { get; set; }
        public List<HostIdentityDataModel> HostIdentityDataModel { get; set; } = new List<HostIdentityDataModel>();
    }
    public class HostIdentityDataModel
    {
        public string IdentityLabel { get; set; }
        public string IdentityType { get; set; }
        public string IdentityValue { get; set; }
        public string IdentityDDLType { get; set; }
        public string IdentityDescription { get; set; }
        public string IdentityLabelJapanese { get; set; }
        public string IdentityLabelEnglish { get; set; }
        public string InputType { get; set; }
    }
}