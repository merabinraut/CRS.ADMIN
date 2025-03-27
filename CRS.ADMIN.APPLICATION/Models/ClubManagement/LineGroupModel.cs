
using CRS.ADMIN.APPLICATION.Resources;
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
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string groupName { get; set; }
        public string qrImage { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        public string link { get; set; }
        public string searchFilter { get; set; }
        public int startIndex { get; set; }
        public int pageSize { get; set; }
    }
}