using CRS.ADMIN.SHARED.AccountInformation;
using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.REPOSITORY.AccountInformation
{
    public interface IAccountInformationRepository
    {
        List<AccountInformationCommonResponse> GetAccountInformationAsync(AccountInformationFilterCommon request, PaginationFilterCommon paginationFilterCommon);
    }
}
