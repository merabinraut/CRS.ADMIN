using CRS.ADMIN.SHARED.AdminPointManagement;
using CRS.ADMIN.SHARED;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRS.ADMIN.SHARED.PaginationManagement;

namespace CRS.ADMIN.REPOSITORY.AdminPointManagement
{
    public interface IAdminPointManagementRepository
    {
        List<AdminPointManagementCommon> GetManualEntryPointList(PointRequestDetailCommon objPointRequestDetailCommon = null, PaginationFilterCommon objPaginationFilterCommon = null);
        CommonDbResponse ManageAdminPointsRequest(ManagePointRequestCommon objManagePointRequestCommon);

    }
}
