using System;

namespace CRS.ADMIN.REPOSITORY.LogManagement.ErrorLogManagement
{
    public interface IErrorLogManagementRepository
    {
        string LogError(Exception Exception, string Page);
    }
}
