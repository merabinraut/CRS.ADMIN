using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.BasicClubManagement;
using CRS.ADMIN.SHARED.ClubManagement;
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
        CommonDbResponse DeleteBasicClubStatus(string AgentId, Common Request);
        CommonDbResponse BlockBasicClubStatus(string AgentId, Common Request);
        CommonDbResponse UnBlockBasicClubStatus(string AgentId, Common Request);
        ClubDetailCommon GetBasicConversionClubDetails(string AgentId, String culture = "");
        CommonDbResponse ManageConversionClub(ManageClubCommon Request);
    }
}
