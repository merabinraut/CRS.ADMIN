using CRS.ADMIN.SHARED.AdminPointManagement;
using CRS.ADMIN.SHARED;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.REPOSITORY.AdminPointManagement
{
    public interface IAdminPointManagementRepository
    {
        CommonDbResponse ManageAdminPointsRequest(ManagePointRequestCommon objManagePointRequestCommon);
    }
}
