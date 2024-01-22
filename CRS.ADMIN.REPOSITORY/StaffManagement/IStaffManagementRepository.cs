using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.StaffManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.REPOSITORY.StaffManagement
{
    public interface IStaffManagementRepository
    {
        CommonDbResponse DeleteStaff(string id, Common commonRequest);
        StaffDetailsCommon GetStaffDetails(string id);
        List<StaffManagementListModelCommon> GetStaffList();
        CommonDbResponse ManageStaff(ManagerStaffCommon commonModel);
    }
}
