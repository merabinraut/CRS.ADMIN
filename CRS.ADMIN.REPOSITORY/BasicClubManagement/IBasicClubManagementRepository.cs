using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.BasicClubManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.REPOSITORY.BasicClubManagement
{
    public interface IBasicClubManagementRepository
    {
        List<BasicClubManagementCommon> GetBasicClubList(PaginationFilterCommon Request);
        CommonDbResponse ManageBasicClub(ManageBasicClubCommon Request);
        ManageBasicClubCommon GetBasicClubDetails(string AgentId, String culture = "");
        CommonDbResponse ManageBasicClubStatus(string AgentId, string Status, Common Request);
    }
}
