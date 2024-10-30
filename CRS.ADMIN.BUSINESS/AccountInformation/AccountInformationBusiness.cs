using CRS.ADMIN.REPOSITORY.AccountInformation;
using CRS.ADMIN.SHARED.AccountInformation;
using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.BUSINESS.AccountInformation
{
    public class AccountInformationBusiness : IAccountInformationBusiness
    {
        private readonly IAccountInformationRepository _accountInformationRepository;
        public AccountInformationBusiness(AccountInformationRepository accountInformationRepository )
        {
            _accountInformationRepository = accountInformationRepository;
        }
        public List<AccountInformationCommonResponse> GetAccountInformationAsync(AccountInformationFilterCommon request, PaginationFilterCommon dbRequest)
        {
            var response = _accountInformationRepository.GetAccountInformationAsync(request, dbRequest);
            return response;
        }
    }
}
