using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.PointSetup;
using DocumentFormat.OpenXml.Office2016.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.REPOSITORY.ClubPlanManagement
{
    public class ClubPlanManagementRepository : IClubPlanManagementRepository
    {
        RepositoryDao _DAO;
        public ClubPlanManagementRepository()
        {
            _DAO = new RepositoryDao();
        }
        public List<PlanListCommon> GetClubPlanIdentityList(string culture, string clubid)
        {
            var plan = new List<planIdentityDataCommon>();
            var PlanListCommon = new List<PlanListCommon>();
            string SQL1 = "EXEC sproc_admin_club_plan_management @Flag='plan_nf'";
            SQL1 += ",@AgentId=" + _DAO.FilterString(!string.IsNullOrEmpty(clubid) ? clubid : null);
            string SQL2 = "EXEC sproc_admin_club_plan_management @Flag='cpi'";
            SQL2 += ",@AgentId=" + _DAO.FilterString(!string.IsNullOrEmpty(clubid) ? clubid : null);
            string SQL3 = "EXEC sproc_admin_club_plan_management @Flag='getplanlist'";
            SQL3 += ",@AgentId=" + _DAO.FilterString(!string.IsNullOrEmpty(clubid) ? clubid : null);
            var dbResponse1 = _DAO.ExecuteDataTable(SQL1);
            var dbResponse2 = _DAO.ExecuteDataTable(SQL2);
            var dbResponse3 = _DAO.ExecuteDataTable(SQL3);
            var response = new List<planIdentityDataCommon>();
            List<PlanListCommon> listcomm = new List<PlanListCommon>();
            //----------------------Start Get plan details of club ---------------------------------//
            int i = 0;
            if (!string.IsNullOrEmpty(Convert.ToString( dbResponse3)))
            {

                if (dbResponse3.Rows.Count > 0)
                {
                    foreach (DataRow item in dbResponse3.Rows.Cast<DataRow>()
                                                          .Where(row => _DAO.ParseColumnValue(row, "PlanListId").ToString() == Convert.ToString(i)))
                    {
                        List<planIdentityDataCommon> filteredPlan = new List<planIdentityDataCommon>();
                        foreach (DataRow row in dbResponse3.Rows.Cast<DataRow>()
                            .Where(row => _DAO.ParseColumnValue(row, "PlanListId").ToString() == Convert.ToString(i)))
                        {
                            filteredPlan.Add(new planIdentityDataCommon()
                            {
                                StaticDataValue = _DAO.ParseColumnValue(row, "StaticDataValue").ToString(),
                                English = _DAO.ParseColumnValue(row, "English").ToString(),
                                PlanListId = _DAO.ParseColumnValue(row, "PlanListId").ToString(),
                                japanese = _DAO.ParseColumnValue(row, "japanese").ToString(),
                                inputtype = _DAO.ParseColumnValue(row, "inputtype").ToString(),
                                name = _DAO.ParseColumnValue(row, "name").ToString(),
                                IdentityDescription = _DAO.ParseColumnValue(row, "Description").ToString(),
                                PlanId = _DAO.ParseColumnValue(row, "Description").ToString(),
                                PlanStatus = _DAO.ParseColumnValue(row, "PlanStatus").ToString(),
                                Id = _DAO.ParseColumnValue(row, "Id").ToString(),
                                IdentityLabel = culture.ToLower() == "en" ? _DAO.ParseColumnValue(row, "English").ToString() : _DAO.ParseColumnValue(row, "japanese").ToString(),
                            });



                        }
                        response.AddRange(filteredPlan);
                        listcomm.Add(new PlanListCommon { PlanIdentityList = filteredPlan });
                        i++;
                    }
                }
            }
            //----------------------END Get plan details of club ---------------------------------//

            //----------------------Start Get plan details from admin that are not found in club---------------------------------//
            if (!string.IsNullOrEmpty(Convert.ToString( dbResponse1)))
            {
                if (dbResponse1.Rows.Count > 0)
                {

                    foreach (DataRow row in dbResponse1.Rows)
                    {
                        List<planIdentityDataCommon> filteredPlan = new List<planIdentityDataCommon>();
                        foreach (DataRow rows in dbResponse2.Rows)
                        {
                            filteredPlan.Add(new planIdentityDataCommon()
                            {
                                StaticDataValue = _DAO.ParseColumnValue(rows, "StaticDataValue").ToString(),
                                English = _DAO.ParseColumnValue(rows, "English").ToString(),
                                PlanListId = Convert.ToString(i),
                                japanese = _DAO.ParseColumnValue(rows, "japanese").ToString(),
                                inputtype = _DAO.ParseColumnValue(rows, "inputtype").ToString(),
                                //IdentityDescription = (_DAO.ParseColumnValue(rows, "StaticDataValue").ToString() == "1") ? _DAO.ParseColumnValue(row, "planId").ToString() : "",
                                IdentityDescription = (_DAO.ParseColumnValue(rows, "StaticDataValue").ToString() == "1") ? _DAO.ParseColumnValue(row, "planId").ToString() :
                                           (_DAO.ParseColumnValue(rows, "StaticDataValue").ToString() == "5") ? "B" : null,
                                 name = _DAO.ParseColumnValue(rows, "name").ToString(),
                                IdentityLabel = culture.ToLower() == "en" ? _DAO.ParseColumnValue(rows, "English").ToString() : _DAO.ParseColumnValue(rows, "japanese").ToString(),
                            }); 

                        }
                        response.AddRange(filteredPlan);
                        listcomm.Add(new PlanListCommon { PlanIdentityList = filteredPlan });
                        i++;
                    }


                }
            }
           

            //----------------------END Get plan details from admin that are not found in club---------------------------------//
            return listcomm;
        }

        public CommonDbResponse ManageClubPlan(ManageClubPlan Request)
        {
            var Response = new CommonDbResponse();
            var i = 0;

            foreach (var planList in Request.ClubPlanDetailList)
            {
                foreach (var planIdentity in planList.PlanIdentityList)
                {
                    try
                    {
                        string SQL2 = "EXEC sproc_admin_club_plan_management ";
                        SQL2 += "@Flag='rcp'";
                        SQL2 += ",@ClubPlanTypeId=" + _DAO.FilterString(planIdentity.StaticDataValue);
                        SQL2 += ",@Description=N" + _DAO.FilterString(planIdentity.IdentityDescription);
                        //SQL2 += ",@PlanListId=" + _DAO.FilterString(Convert.ToString(i));
                        SQL2 += ",@PlanListId=" + _DAO.FilterString(Convert.ToString(planIdentity.PlanListId));
                        //SQL2 += ",@ClubId=" + _DAO.FilterString(!string.IsNullOrEmpty(Response.Extra1) ? Response.Extra1 : null);
                        SQL2 += ",@AgentId=" + _DAO.FilterString(!string.IsNullOrEmpty(Request.ClubId) ? Request.ClubId : null);
                        SQL2 += ",@ActionPlatform=" + _DAO.FilterString(Request.ActionPlatform);
                        SQL2 += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
                        //SQL2 += ",@Id=" + _DAO.FilterString(string.IsNullOrEmpty(Request.holdId) ? null : planIdentity.Id);

                        // Execute SQL command within the transaction
                        Response= _DAO.ParseCommonDbResponse(SQL2);


                    }
                    catch (Exception ex)
                    { 
                    }

                }
                i++;
            }

            // Commit the transaction if all statements succeed                          

            return Response;

        }
        public List<ClubplanListCommon> GetClubPlanList(string culture, string clubid)
        {          
            string SQL1 = "EXEC sproc_club_plan_list ";
            SQL1 += " @AgentId=" + _DAO.FilterString(!string.IsNullOrEmpty(clubid) ? clubid : null);                     
            var response = new List<ClubplanListCommon>();
            var dbResponse = _DAO.ExecuteDataTable(SQL1);
            if (dbResponse != null)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new ClubplanListCommon()
                    {
                        //Status = _DAO.ParseColumnValue(item, "Status").ToString(),
                        Id = _DAO.ParseColumnValue(item, "Id").ToString(),
                        PlanName = _DAO.ParseColumnValue(item, "PlanName").ToString(),
                        PlanId = _DAO.ParseColumnValue(item, "PlanId").ToString(),
                        LastEntryTime = _DAO.ParseColumnValue(item, "LastEntryTime").ToString(),
                        LastOrderTime = _DAO.ParseColumnValue(item, "LastOrderTime").ToString(),
                        CreatedDate = _DAO.ParseColumnValue(item, "CreatedDate").ToString(),
                        UpdatedDate = _DAO.ParseColumnValue(item, "UpdatedDate").ToString(),
                        NoofPeople = _DAO.ParseColumnValue(item, "NoofPeople").ToString(),
                        Status = _DAO.ParseColumnValue(item, "Status").ToString(),                        
                        //SNO = Convert.ToInt32(_DAO.ParseColumnValue(item, "holdId").ToString()),                        
                    });
                }
            }
            return response;
            
        }
        public List<PlanListCommon> EditClubPlanIdentityList(string culture, string clubid,string planlistid)
        {
            var plan = new List<planIdentityDataCommon>();
            var PlanListCommon = new List<PlanListCommon>();   
            string SQL3 = "EXEC sproc_admin_club_plan_management @Flag='e_pld'";
            SQL3 += ",@AgentId=" + _DAO.FilterString(!string.IsNullOrEmpty(clubid) ? clubid : null);
            SQL3 += ",@PlanListId=" + _DAO.FilterString(!string.IsNullOrEmpty(planlistid) ? planlistid : null);
            var dbResponse3 = _DAO.ExecuteDataTable(SQL3);
            var response = new List<planIdentityDataCommon>();
            List<PlanListCommon> listcomm = new List<PlanListCommon>();
            int i = 0;
            if (dbResponse3.Rows.Count > 0)
            {
                
                    List<planIdentityDataCommon> filteredPlan = new List<planIdentityDataCommon>();
                    foreach (DataRow row in dbResponse3.Rows)
                    {
                        filteredPlan.Add(new planIdentityDataCommon()
                        {
                            StaticDataValue = _DAO.ParseColumnValue(row, "StaticDataValue").ToString(),
                            English = _DAO.ParseColumnValue(row, "English").ToString(),
                            PlanListId = _DAO.ParseColumnValue(row, "PlanListId").ToString(),
                            japanese = _DAO.ParseColumnValue(row, "japanese").ToString(),
                            inputtype = _DAO.ParseColumnValue(row, "inputtype").ToString(),
                            name = _DAO.ParseColumnValue(row, "name").ToString(),
                            IdentityDescription = _DAO.ParseColumnValue(row, "Description").ToString(),
                            PlanId = _DAO.ParseColumnValue(row, "Description").ToString(),
                            PlanStatus = _DAO.ParseColumnValue(row, "PlanStatus").ToString(),
                            Id = _DAO.ParseColumnValue(row, "Id").ToString(),
                            IdentityLabel = culture.ToLower() == "en" ? _DAO.ParseColumnValue(row, "English").ToString() : _DAO.ParseColumnValue(row, "japanese").ToString(),
                        });



                    }
                    response.AddRange(filteredPlan);
                    listcomm.Add(new PlanListCommon { PlanIdentityList = filteredPlan });
                
            }
                     
            return listcomm;
        }

        public CommonDbResponse BlockUnblockPlan(ClubplanListCommon objClubplanListCommon = null,string ActionUser=null,string ActionIp=null)
        {
            string SQL = "EXEC sproc_admin_club_plan_management @Flag='u_status' ";
            SQL += ",@AgentId=" + _DAO.FilterString(objClubplanListCommon.ClubId);
            SQL += ",@PlanListId=" + _DAO.FilterString(objClubplanListCommon.PlanId);
            SQL += ",@Status=" + _DAO.FilterString(objClubplanListCommon.Status);
            SQL += ",@ActionIP=" + _DAO.FilterString(ActionIp);
            SQL += ",@ActionUser=" + _DAO.FilterString(ActionUser);
            return _DAO.ParseCommonDbResponse(SQL);
        }
    }

}
