using CRS.ADMIN.REPOSITORY.LogManagement.APILogManagement;

namespace CRS.ADMIN.BUSINESS.LogManagement.APILogManagement
{
    public class APILogManagementBusiness : IAPILogManagementBusiness
    {
        IAPILogManagementRepository _REPO;
        public APILogManagementBusiness(APILogManagementRepository REPO)
        {
            _REPO = REPO;
        }
    }
}
