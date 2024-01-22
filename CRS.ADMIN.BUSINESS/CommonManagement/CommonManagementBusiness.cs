using CRS.ADMIN.REPOSITORY;
using CRS.ADMIN.REPOSITORY.CommonManagement;
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
    }
}
