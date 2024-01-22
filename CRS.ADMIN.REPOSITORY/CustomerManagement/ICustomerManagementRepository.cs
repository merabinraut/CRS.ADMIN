using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.CustomerManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.CustomerManagement
{
    public interface ICustomerManagementRepository
    {
        List<CustomerListCommon> GetCustomerList(CustomerSearchFilterCommon Request);
        CommonDbResponse ManageCustomer(ManageCustomerCommon Request);
        ManageCustomerCommon GetCustomerDetail(string AgentId);
        CommonDbResponse ManageCustomerStatus(string AgentId, string Status, Common Request);
        CommonDbResponse ResetCustomerPassword(string AgentId, Common Request);
    }
}
