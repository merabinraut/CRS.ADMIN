using System.ComponentModel.DataAnnotations;

namespace CRS.ADMIN.APPLICATION.Models.CommissionManagement
{
    public class AssignCommissionModel
    {
        [Required]
        [Display(Name = "Club Name")]
        public string AgentId { get; set; }
        [Required]
        [Display(Name = "Category Name")]
        public string CategoryId { get; set; }
        [Display(Name = "Current Category Name")]
        public string CurrentCategory { get; set; }
    }
}