
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace CRS.ADMIN.APPLICATION.Models.ClubManagement
{
    public class LineGroupModel
    {
        public string clubId { get; set; }
        public string groupId { get; set; }
        public string groupName { get; set; }
        public string qrImage { get; set; }
        public string link { get; set; }
        public string searchFilter { get; set; }
        public int startIndex { get; set; }
        public int pageSize { get; set; }
    }
}