using CRS.ADMIN.SHARED;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.CommonManagement
{
    public interface ICommonManagementRepository
    {
        Dictionary<string, string> GetDropDown(string Flag, string Extra1 = "", string Extra2 = "");
        Dictionary<string, (string Text, string japaneseText, string culture)> GetDropDownValues(string Flag, string Extra1 = "", string Extra2 = "", string culture = "");
        List<StaticDataCommon> GetDropDownLanguage(string Flag, string Extra1 = "", string Extra2 = "", string culture = "");
        List<MultipleItemCommon> GetDropDownItem(string Flag, string Extra1 = "", string Extra2 = "", string culture = "");
    }
}
