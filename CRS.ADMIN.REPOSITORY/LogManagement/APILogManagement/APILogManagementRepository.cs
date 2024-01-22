namespace CRS.ADMIN.REPOSITORY.LogManagement.APILogManagement
{
    public class APILogManagementRepository : IAPILogManagementRepository
    {
        RepositoryDao _REPO;
        public APILogManagementRepository()
        {
            _REPO = new RepositoryDao();
        }
    }
}
