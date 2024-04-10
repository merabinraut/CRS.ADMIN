using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.SHARED.ClubManagement
{
    public class ManageManagerCommon:Common
    {
        public string ManagerId { get; set; }
        public string ClubId { get; set; }        
        public string ManagerName { get; set; }      
        public string Email { get; set; }      
        public string MobileNumber { get; set; }
    }
}
