
using CRS.ADMIN.APPLICATION.Models.TagManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace CRS.ADMIN.APPLICATION.Models.ClubManagement
{

    public class EventModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string EventType { get; set; }
        public string AgentId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string EventDate { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string Description { get; set; }
        public string Image { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string Title { get; set; }
        public string EventId { get; set; }
        public string EventTypeName { get; set; }
        public string Status { get; set; }
        public string LoginId { get; set; }
    }

    public class EventManagementCommonModel
    {
        public string SearchFilter { get; set; }
        public string ClubId { get; set; }
        public List<EventListModel> EventListModel { get; set; }
        public EventModel ManageEventModel { get; set; }
       
    }

    public class EventListModel
    {
        public string SNO { get; set; }
        public string EventType { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public string AgentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }       
        public string Sno { get; set; }
        public string LoginId { get; set; }
        public string Status { get; set; }
        public string EventDate { get; set; }
        public string EventId { get; set; }

    }
}