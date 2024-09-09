using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.SHARED.AdminPointManagement
{
    public class AdminPointManagementCommon
    {
        public string searchFilter { get; set; }
        public string txnId { get; set; }
        public string point { get; set; }
        public string remarks { get; set; }
        public string createdby { get; set; }

    }
    public class ManagePointRequestCommon
    {
        public string point { get; set; }
        public string remarks { get; set; }
        public string actionIp { get; set; }
        public string actionUser { get; set; }
    }
}
