using CRS.ADMIN.REPOSITORY.BasicClubManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.BasicClubManagement;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CRS.ADMIN.BUSINESS.BasicClubManagement
{
    public class BasicClubManagementBusiness : IBasicClubManagementBusiness
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
        public CommonDbResponse DeleteBasicClubStatus(string AgentId, Common Request)
        {
            return _REPO.DeleteBasicClubStatus(AgentId, Request);
        }
        public CommonDbResponse BlockBasicClubStatus(string AgentId, Common Request)
        {
            return _REPO.BlockBasicClubStatus(AgentId, Request);
        }
        public CommonDbResponse UnBlockBasicClubStatus(string AgentId, Common Request)
        {
            return _REPO.UnBlockBasicClubStatus(AgentId, Request);
        }
        public ClubDetailCommon GetBasicConversionClubDetails(string AgentId, String culture = "")
        {
            return _REPO.GetBasicConversionClubDetails(AgentId, culture);
        }
        public CommonDbResponse ManageConversionClub(ManageClubCommon Request, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            return _REPO.ManageConversionClub(Request, connection, transaction);
        }
    }
}
