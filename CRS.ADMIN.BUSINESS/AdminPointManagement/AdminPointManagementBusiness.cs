using CRS.ADMIN.REPOSITORY.AdminPointManagement;
using CRS.ADMIN.REPOSITORY.AffiliateManagement;
using CRS.ADMIN.SHARED.AffiliateManagement;
using CRS.ADMIN.SHARED;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRS.ADMIN.SHARED.AdminPointManagement;

namespace CRS.ADMIN.BUSINESS.AdminPointManagement
{
    public class AdminPointManagementBusiness: IAdminPointManagementBusiness
    {
        private readonly IAdminPointManagementRepository _repo;
        public AdminPointManagementBusiness(AdminPointManagementRepository repo)
        {
            _repo = repo;
        }

        public CommonDbResponse ManageAdminPointsRequest(ManagePointRequestCommon objManagePointRequestCommon)
        {
            return _repo.ManageAdminPointsRequest(objManagePointRequestCommon);
        }

    }
}
