using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.StaffManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.StaffManagement
{
    public interface IStaffManagementBusiness
    {
        CommonDbResponse DeleteStaff(string id, Common commonRequest);
        StaffDetailsCommon GetStaffDetails(string id);
        List<StaffManagementListModelCommon> GetStaffList();
        CommonDbResponse ManageStaff(ManagerStaffCommon commonModel);
    }
}
