using CRS.ADMIN.REPOSITORY.CustomerManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.CustomerManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.CustomerManagement
{
    public class CustomerManagementBusiness : ICustomerManagementBusiness
    {
        ICustomerManagementRepository _REPO;
        public CustomerManagementBusiness(CustomerManagementRepository REPO)
        {
            _REPO = REPO;
        }

        public ManageCustomerCommon GetCustomerDetail(string AgentId)
        {
            return _REPO.GetCustomerDetail(AgentId);
        }

        public List<CustomerListCommon> GetCustomerList(CustomerSearchFilterCommon Request)
        {
            return _REPO.GetCustomerList(Request);
        }

        public CommonDbResponse ManageCustomer(ManageCustomerCommon Request)
        {
            return _REPO.ManageCustomer(Request);
        }

        public CommonDbResponse ResetCustomerPassword(string AgentId, Common Request)
        {
            return _REPO.ResetCustomerPassword(AgentId, Request);
        }

        public CommonDbResponse ManageCustomerStatus(string AgentId, string Status, Common Request)
        {
            return _REPO.ManageCustomerStatus(AgentId, Status, Request);
        }
    }
}
