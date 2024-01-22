using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.CommonManagement
{
    public interface ICommonManagementBusiness
    {
        Dictionary<string, string> GetDropDown(string Flag, string Extra1 = "", string Extra2 = "");
    }
}
