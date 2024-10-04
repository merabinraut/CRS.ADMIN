using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.BasicClubManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.BUSINESS.BasicClubManagement
{
    public interface IBasicClubManagementBusiness
    {
        List<BasicClubManagementCommon> GetBasicClubList(PaginationFilterCommon Request);
        CommonDbResponse ManageBasicClub(ManageBasicClubCommon Request);
        CommonDbResponse ManageBasicClubStatus(string AgentId, string Status, Common Request);
    }
}
