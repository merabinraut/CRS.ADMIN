using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PlanManagement;
using DocumentFormat.OpenXml.Office2016.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;

namespace CRS.ADMIN.REPOSITORY.PlanManagement
{
    public class PlanManagementRepository : IPlanManagementRepository
    {
        private readonly RepositoryDao _dao;
        public PlanManagementRepository() => _dao = new RepositoryDao();

        public List<PlanManagementCommon> GetPlanList(PaginationFilterCommon Request)
        {
            var planList = new List<PlanManagementCommon>();
            var sql = "Exec sproc_admin_plan_management @Flag='s'";
            sql += !string.IsNullOrEmpty(Request.SearchFilter) ? ",@SearchFilter=N" + _dao.FilterString(Request.SearchFilter) : null;
            sql += ",@Skip=" + Request.Skip;
            sql += ",@Take=" + Request.Take;
            var dt = _dao.ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    planList.Add(new PlanManagementCommon()
                    {
                        PlanId = item["PlanId"].ToString(),
                        PlanName = item["PlanName"].ToString(),
                        PlanType = item["PlanType"].ToString(),
                        PlanTime = item["PlanTime"].ToString(),
                        Price = item["Price"].ToString(),
                        Liquor = item["Liquor"].ToString(),
                        Nomination = item["Nomination"].ToString(),
                        Remarks = item["Remarks"].ToString(),
                        PlanStatus = item["PlanStatus"].ToString(),
                        ActionUser = item["ActionUser"].ToString(),
                        ActionIP = item["ActionIp"].ToString(),
                        ActionPlatform = item["ActionPlatform"].ToString(),
                        ActionDate = item["ActionDate"].ToString(),
                        PlanImage = item["PlanImage"].ToString(),
                        PlanImage2 = item["PlanImage2"].ToString(),
                        TotalRecords = Convert.ToInt32(_dao.ParseColumnValue(item, "TotalRecords").ToString()),
                        SNO = Convert.ToInt32(_dao.ParseColumnValue(item, "SNO").ToString())
                    });
                }
            }
            return planList;
        }

        public PlanManagementCommon GetPlan(PlanManagementCommon planManagementCommon)
        {
            string sql = "Exec sproc_admin_plan_management";
            sql += " @Flag='s'";
            sql += ", @PlanId=" + _dao.FilterString(planManagementCommon.PlanId);
            sql += ", @ActionUser=" + _dao.FilterString(planManagementCommon.ActionUser);
            var dataTable = _dao.ExecuteDataTable(sql);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                return new PlanManagementCommon()
                {
                    PlanId = dataTable.Rows[0]["PlanId"].ToString(),
                    PlanName = dataTable.Rows[0]["PlanName"].ToString(),
                    PlanType = dataTable.Rows[0]["PlanType"].ToString(),
                    PlanTime = dataTable.Rows[0]["PlanTime"].ToString(),
                    Price = dataTable.Rows[0]["Price"].ToString(),
                    Liquor = dataTable.Rows[0]["Liquor"].ToString(),
                    Nomination = dataTable.Rows[0]["Nomination"].ToString(),
                    Remarks = dataTable.Rows[0]["Remarks"].ToString(),
                    PlanStatus = dataTable.Rows[0]["PlanStatus"].ToString(),
                    ActionUser = dataTable.Rows[0]["ActionUser"].ToString(),
                    ActionIP = dataTable.Rows[0]["ActionIp"].ToString(),
                    ActionPlatform = dataTable.Rows[0]["ActionPlatform"].ToString(),
                    ActionDate = dataTable.Rows[0]["ActionDate"].ToString(),
                    PlanImage = dataTable.Rows[0]["PlanImage"].ToString(),
                    PlanImage2 = dataTable.Rows[0]["PlanImage2"].ToString(),
                    ExtraField1 = dataTable.Rows[0]["AdditionalValue1"].ToString(),
                    ExtraField2 = dataTable.Rows[0]["AdditionalValue2"].ToString(),
                    ExtraField3 = dataTable.Rows[0]["AdditionalValue3"].ToString(),
                    PlanCategory = dataTable.Rows[0]["PlanCategory"].ToString(),
                    NoOfPeople = !string.IsNullOrEmpty(dataTable.Rows[0]["NoOfPeople"].ToString()) ? Convert.ToInt32(dataTable.Rows[0]["NoOfPeople"].ToString()) : 0
                };
            }
            return new PlanManagementCommon();
        }

        public PlanManagementCommon GetPlanDetail(PlanManagementCommon planManagementCommon)
        {
            string sql = "Exec sproc_admin_plan_management";
            sql += " @Flag='sd'";
            sql += ", @PlanId=" + _dao.FilterString(planManagementCommon.PlanId);
            sql += ", @ActionUser=" + _dao.FilterString(planManagementCommon.ActionUser);
            var dataTable = _dao.ExecuteDataTable(sql);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                return new PlanManagementCommon()
                {
                    PlanId = dataTable.Rows[0]["PlanId"].ToString(),
                    PlanName = dataTable.Rows[0]["PlanName"].ToString(),
                    PlanType = dataTable.Rows[0]["PlanType"].ToString(),
                    PlanTime = dataTable.Rows[0]["PlanTime"].ToString(),
                    Price = dataTable.Rows[0]["Price"].ToString(),
                    Liquor = dataTable.Rows[0]["Liquor"].ToString(),
                    Nomination = dataTable.Rows[0]["Nomination"].ToString(),
                    Remarks = dataTable.Rows[0]["Remarks"].ToString(),
                    PlanStatus = dataTable.Rows[0]["PlanStatus"].ToString(),
                    ActionUser = dataTable.Rows[0]["ActionUser"].ToString(),
                    ActionIP = dataTable.Rows[0]["ActionIp"].ToString(),
                    ActionPlatform = dataTable.Rows[0]["ActionPlatform"].ToString(),
                    ActionDate = dataTable.Rows[0]["ActionDate"].ToString(),
                    PlanImage = dataTable.Rows[0]["PlanImage"].ToString(),
                    PlanImage2 = dataTable.Rows[0]["PlanImage2"].ToString(),
                    ExtraField1 = dataTable.Rows[0]["AdditionalValue1"].ToString(),
                    ExtraField2 = dataTable.Rows[0]["AdditionalValue2"].ToString(),
                    ExtraField3 = dataTable.Rows[0]["AdditionalValue3"].ToString(),
                    PlanCategory = dataTable.Rows[0]["PlanCategory"].ToString(),
                    NoOfPeople = !string.IsNullOrEmpty(dataTable.Rows[0]["NoOfPeople"].ToString()) ? Convert.ToInt32(dataTable.Rows[0]["NoOfPeople"].ToString()) : 0,
                    StrikePrice = dataTable.Rows[0]["StrikePrice"].ToString(),
                    IsStrikeOut = dataTable.Rows[0]["IsStrikeOut"].ToString()

                };
            }
            return new PlanManagementCommon();
        }

        public CommonDbResponse ManagePlan(PlanManagementCommon planManagementCommon)
        {
            string sql = "Exec sproc_admin_plan_management";
            string flag = planManagementCommon.PlanId is null ? "i" : "u";
            sql += $" @Flag='{flag}'";
            sql += !string.IsNullOrEmpty(planManagementCommon.PlanId) ? ", @PlanId=" + _dao.FilterString(planManagementCommon.PlanId) : null;
            sql += ", @PlanName=N" + _dao.FilterString(planManagementCommon.PlanName);
            sql += ", @PlanType=" + _dao.FilterString(planManagementCommon.PlanType);
            sql += ", @Time=" + _dao.FilterString(planManagementCommon.PlanTime);
            sql += ", @Price=" + _dao.FilterString(planManagementCommon.Price);
            sql += ", @Liquor=" + _dao.FilterString(planManagementCommon.Liquor);
            sql += ", @Nomination=" + planManagementCommon.Nomination;
            sql += ",@Remarks=" + (!string.IsNullOrEmpty(planManagementCommon.Remarks) ? "N" + _dao.FilterString(planManagementCommon.Remarks) : _dao.FilterString(planManagementCommon.Remarks));
            sql += ", @ActionIp=" + _dao.FilterString(planManagementCommon.ActionIP);
            sql += ", @ActionPlatform=" + _dao.FilterString(planManagementCommon.ActionPlatform);
            sql += ", @ActionUser=" + _dao.FilterString(planManagementCommon.ActionUser);
            sql += ", @PlanImage=" + _dao.FilterString(planManagementCommon.PlanImage);
            sql += ", @PlanImage2=" + _dao.FilterString(planManagementCommon.PlanImage2);
            sql += ", @ExtraField1=N" + _dao.FilterString(planManagementCommon.ExtraField1);
            sql += ", @ExtraField2=N" + _dao.FilterString(planManagementCommon.ExtraField2);
            sql += ", @ExtraField3=N" + _dao.FilterString(planManagementCommon.ExtraField3);
            sql += ", @PlanCategory=" + _dao.FilterString(planManagementCommon.PlanCategory);
            sql += ", @NoOfPeople=" + planManagementCommon.NoOfPeople;
            sql += ", @StrikePrice=" + _dao.FilterString(planManagementCommon.StrikePrice);
            sql += ", @IsStrikeOut=" + _dao.FilterString(planManagementCommon.IsStrikeOut);
            return _dao.ParseCommonDbResponse(sql);
        }

        public CommonDbResponse EnableDisablePlans(PlanManagementCommon planManagement)
        {
            string sql = "Exec sproc_admin_plan_management";
            sql += " @Flag='bu'";
            sql += ", @PlanId=" + _dao.FilterString(planManagement.PlanId);
            sql += ", @ActionUser=" + _dao.FilterString(planManagement.ActionUser);
            sql += ", @ActionIP=" + _dao.FilterString(planManagement.ActionIP);
            sql += ", @ActionPlatform=" + _dao.FilterString(planManagement.ActionPlatform);
            return _dao.ParseCommonDbResponse(sql);
        }

        #region DDL
        public List<StaticDataCommon> GetDDL(string StaticType)
        {
            string SQL = "EXEC sproc_admin_plan_management @Flag = 'gpddl'";
            SQL += ",@StaticType=" + _dao.FilterString(StaticType);
            var dbResponse = _dao.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0) return _dao.DataTableToListObject<StaticDataCommon>(dbResponse).ToList();
            return new List<StaticDataCommon>();
        }

        public List<PlanRequesResponseListCommon> GetPlanRequestList(PaginationFilterCommon Request)
        {
            var planList = new List<PlanRequesResponseListCommon>();
            var sql = "Exec apiproc_admin_get_club_own_plan_list";
            sql += " @SearchFilter=N" + _dao.FilterString(Request.SearchFilter);
            sql += ", @Skip=" + Request.Skip;
            sql += ",@Take=" + Request.Take;
            var dt = _dao.ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    planList.Add(new PlanRequesResponseListCommon()
                    {
                        planId = item["PlanId"].ToString(),
                        clubId = item["clubId"].ToString(),
                        clubName = item["clubName"].ToString(),
                        plantype = item["plantype"].ToString(),
                        planTitle = item["planTitle"].ToString(),
                        planTime = item["planTime"].ToString(),
                        planPrice = item["planPrice"].ToString(),
                        numberOfPeople = item["numberOfPeople"].ToString(),
                        nomination = item["Nomination"].ToString(),
                        requestDate = item["requestDate"].ToString(),
                        planStatus = item["planStatus"].ToString(),
                        TotalRecords = Convert.ToInt32(_dao.ParseColumnValue(item, "TotalRecords").ToString()),
                        SNO = Convert.ToInt32(_dao.ParseColumnValue(item, "SNO").ToString())
                    });
                }
            }
            return planList;
        }

        public CommonDbResponse ApprovePlanRequest(string clubId, string type, string planId)
        {
            string SQL = "EXEC apiproc_clp_approve_club_plan_by_admin";
            SQL += " @clubId=" + _dao.FilterString(clubId);
            SQL += ",@status=" + _dao.FilterString(type);
            SQL += ",@ActionUser=" + _dao.FilterString("admin");
            SQL += ",@actionPlatform=" + _dao.FilterString("web");
            SQL += ",@planRequestId=" + _dao.FilterString(planId);
            var dbResponse = _dao.ParseCommonDbResponse(SQL);
            return dbResponse;
        }

        public PlanRequesResponseListCommon GetPlanRequestDetails(string clubId, string planId)
        {
            string SQL = "EXEC apiproc_clp_get_club_own_plan_details";
            SQL += " @clubId=" + _dao.FilterString(clubId);
            SQL += ",@planId=" + _dao.FilterString(planId);
            SQL += ",@ActionUserId=" + _dao.FilterString("admin");
            SQL += ",@actionPlatform=" + _dao.FilterString("web");
            var dataTable = _dao.ExecuteDataTable(SQL);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                return new PlanRequesResponseListCommon()
                {

                    clubName = dataTable.Rows[0]["clubName"].ToString(),
                    clubId = dataTable.Rows[0]["clubId"].ToString(),
                    planId = dataTable.Rows[0]["planId"].ToString(),
                    plantype = dataTable.Rows[0]["plantype"].ToString(),
                    planTitle = dataTable.Rows[0]["PlanName"].ToString(),
                    planTime = dataTable.Rows[0]["planTime"].ToString(),
                    planPrice = dataTable.Rows[0]["price"].ToString(),
                    numberOfPeople = dataTable.Rows[0]["noOfPeople"].ToString(),
                    requestDate = dataTable.Rows[0]["requestDate"].ToString(),
                    planStatus = dataTable.Rows[0]["planStatus"].ToString(),
                    nomination = dataTable.Rows[0]["nomination"].ToString(),
                    lastEntryTime = dataTable.Rows[0]["lastEntrytime"].ToString(),
                };
            }
            return new PlanRequesResponseListCommon();
        }

        public List<StaticDataCommon> GetTimeInterval(string clubId)
        {
            string SQL = "EXEC sproc_admin_get_time_interval_list";
            SQL += " @StaticType=" + _dao.FilterString(clubId);
            var dbResponse = _dao.ExecuteDataTable(SQL);
            if (dbResponse != null && dbResponse.Rows.Count > 0) return _dao.DataTableToListObject<StaticDataCommon>(dbResponse).ToList();
            return new List<StaticDataCommon>();
        }

        public CommonDbResponse ManageClubPlan(PlanRequesRequestCommon requestMapped)
        {
            string SQL = "EXEC sproc_admin_update_club_plan";
            SQL += " @clubId=" + _dao.FilterString(requestMapped.clubId);
            SQL += ",@planId=" + _dao.FilterString(requestMapped.planId);
            SQL += ",@planPrice=" + _dao.FilterString(requestMapped.planPrice);
            SQL += ",@numberNomination=" + _dao.FilterString(requestMapped.nomination);
            SQL += ",@lastEntryTime=" + _dao.FilterString(requestMapped.planTime);
            SQL += ",@noOfPeople=" + _dao.FilterString(requestMapped.numberOfPeople);
            SQL += ",@actionUserId=" + _dao.FilterString("admin");
            SQL += ",@actionIP=" + _dao.FilterString("::1");
            SQL += ",@actionPlatform=" + _dao.FilterString("web");
            var dbResponse = _dao.ParseCommonDbResponse(SQL);
            return dbResponse;
        }
        #endregion
    }
}