using CRS.ADMIN.SHARED.AdminPointManagement;
using CRS.ADMIN.SHARED;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.BUSINESS.AdminPointManagement
{
    public interface IAdminPointManagementBusiness
    {
        CommonDbResponse ManageAdminPointsRequest(ManagePointRequestCommon objManagePointRequestCommon);
    }
}
