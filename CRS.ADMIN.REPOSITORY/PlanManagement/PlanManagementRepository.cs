using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PlanManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
            sql += ", @Remarks=N" + _dao.FilterString(planManagementCommon.Remarks);
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
        #endregion
    }
}