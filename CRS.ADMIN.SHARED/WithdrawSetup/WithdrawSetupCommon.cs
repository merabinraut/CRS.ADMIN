using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.SHARED.WithdrawSetup
{
    public class WithdrawSetupCommon
    {
    }
    public class ConfigCommon
    {
        public string Id { get; set; }
        public string ConfigKey { get; set; }
        public string ConfigValue { get; set; }
    }
    public class WithdrawSetupListCommon
    {       
        public string minAmount { get; set; }
        public string maxAmount { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string withdrawDate { get; set; }

    }
}
