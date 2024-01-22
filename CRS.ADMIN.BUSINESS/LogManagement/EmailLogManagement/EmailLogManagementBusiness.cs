using CRS.ADMIN.REPOSITORY.LogManagement.EmailLogManagement;

namespace CRS.ADMIN.BUSINESS.LogManagement.EmailLogManagement
{
    public class EmailLogManagementBusiness : IEmailLogManagementBusiness
    {
        IEmailLogManagementRepository _REPO;
        public EmailLogManagementBusiness(EmailLogManagementRepository REPO)
        {
            _REPO = REPO;
        }
    }
}
