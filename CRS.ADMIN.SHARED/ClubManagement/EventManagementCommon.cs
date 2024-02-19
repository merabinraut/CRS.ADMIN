using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.SHARED.ClubManagement
{
    public class EventListCommon : PaginationResponseCommon
    {
        public string EventType { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public string AgentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Sno { get; set; }
        public string EventDate { get; set; }
        public string EventId { get; set; }
        public string Status { get; set; }
        public string LoginId { get; set; }

    }
    public class EventCommon
    {
        public string EventType { get; set; }
        public string EventTypeName { get; set; }
        public string AgentId { get; set; }
        public string EventDate { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string flag { get; set; }

        public string ActionUser { get; set; }
        public string ActionIP { get; set; }
        public string ActionPlatform { get; set; }
        public string EventId { get; set; }
    
    }
}
