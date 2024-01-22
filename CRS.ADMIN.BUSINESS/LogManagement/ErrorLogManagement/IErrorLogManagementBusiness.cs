using System;

namespace CRS.ADMIN.BUSINESS.LogManagement.ErrorLogManagement
{
    public interface IErrorLogManagementBusiness
    {
        string LogError(Exception Exception, string Page);
    }
}
