using CRS.ADMIN.REPOSITORY;
using CRS.ADMIN.REPOSITORY.CommonManagement;
using CRS.ADMIN.SHARED;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.CommonManagement
{
    public class CommonManagementBusiness : ICommonManagementBusiness
    {
        private ICommonManagementRepository _REPO;
        public CommonManagementBusiness()
        {
            _REPO = new CommonManagementRepository();
        }
        public Dictionary<string, string> GetDropDown(string Flag, string Extra1 = "", string Extra2 = "")
        {
            return _REPO.GetDropDown(Flag, Extra1, Extra2);
        }
        public Dictionary<string, (string Text, string japaneseText, string culture)> GetDropDownValues(string Flag, string Extra1 = "", string Extra2 = "", string culture = "")
        {
            return _REPO.GetDropDownValues(Flag, Extra1, Extra2,  culture );
        }
        public List<StaticDataCommon> GetDropDownLanguage(string Flag, string Extra1 = "", string Extra2 = "", string culture = "")
        {
            return _REPO.GetDropDownLanguage(Flag, Extra1, Extra2,  culture );
        }
    }
}
