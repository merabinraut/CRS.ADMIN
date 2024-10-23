using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.WithdrawSetup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.REPOSITORY.WithdrawSetup
{
    public interface IWithdrawSetupRepository
    {
        CommonDbResponse ManageWithdrawSetup(string Xml,string Type);
        List<WithdrawSetupListCommon> GetWithdrawSetupList();
        WithdrawSetupListCommon GetWithdrawSetupDetail();
    }
}
