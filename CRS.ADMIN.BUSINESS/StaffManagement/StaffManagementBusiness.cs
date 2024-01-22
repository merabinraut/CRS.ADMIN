using CRS.ADMIN.REPOSITORY.StaffManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.StaffManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.StaffManagement
{
    public class StaffManagementBusiness : IStaffManagementBusiness
    {
         IStaffManagementRepository _repo;
        public StaffManagementBusiness(StaffManagementRepository repo)
        {
            _repo = repo;
        }

        public CommonDbResponse DeleteStaff(string id, Common commonRequest)
        {
            return _repo.DeleteStaff(id, commonRequest);
        }

        public StaffDetailsCommon GetStaffDetails(string id)
        {
            return _repo.GetStaffDetails(id);
        }

        public List<StaffManagementListModelCommon> GetStaffList()
        {
            return _repo.GetStaffList();
        }

        public CommonDbResponse ManageStaff(ManagerStaffCommon commonModel)
        {
            return _repo.ManageStaff(commonModel);
        }
    }
}
