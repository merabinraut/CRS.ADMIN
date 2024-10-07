using CRS.ADMIN.REPOSITORY.BasicClubManagement;
using CRS.ADMIN.REPOSITORY.ClubManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.BasicClubManagement;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.BUSINESS.BasicClubManagement
{
    public class BasicClubManagementBusiness: IBasicClubManagementBusiness
    {
        IBasicClubManagementRepository _REPO;
        public BasicClubManagementBusiness(BasicClubManagementRepository REPO)
        {
            _REPO = REPO;
        }
        public List<BasicClubManagementCommon> GetBasicClubList(PaginationFilterCommon Request)
        {
            return _REPO.GetBasicClubList(Request);
        }
        public CommonDbResponse ManageBasicClub(ManageBasicClubCommon Request)
        {
            return _REPO.ManageBasicClub(Request);
        }
        public ManageBasicClubCommon GetBasicClubDetails(string AgentId, String culture = "")
        {
            return _REPO.GetBasicClubDetails(AgentId, culture);
        }
        public CommonDbResponse ManageBasicClubStatus(string AgentId, string Status, Common Request)
        {
            return _REPO.ManageBasicClubStatus(AgentId, Status, Request);
        }
    }
}
