using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRS.ADMIN.REPOSITORY.BasicClubManagement;
using CRS.ADMIN.REPOSITORY.WithdrawSetup;
using CRS.ADMIN.SHARED.WithdrawSetup;

namespace CRS.ADMIN.BUSINESS.WithdrawSetup
{
    public class WithdrawSetupBusiness: IWithdrawSetupBusiness
    {
        IWithdrawSetupRepository _REPO;
        public WithdrawSetupBusiness(WithdrawSetupRepository REPO)
        {
            _REPO = REPO;
        }
        public CommonDbResponse ManageWithdrawSetup(string Xml,string Type)
        {
            return _REPO.ManageWithdrawSetup(Xml, Type);
        }
        public List<WithdrawSetupListCommon> GetWithdrawSetupList()
        {
            return _REPO.GetWithdrawSetupList();
        }
        public WithdrawSetupListCommon GetWithdrawSetupDetail()
        {
            return _REPO.GetWithdrawSetupDetail();
        }
    }
}
