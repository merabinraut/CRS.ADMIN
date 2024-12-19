using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ChargeManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PointSetup;
using System;
using System.Collections.Generic;
using System.Data;

namespace CRS.ADMIN.REPOSITORY.ChargeManagement
{
    public class ChargeManagementRepository : IChargeManagementRepository
    {
        private readonly RepositoryDao _dao;
        public ChargeManagementRepository()
        {
            _dao = new RepositoryDao();
        }

        #region Charge Category Management
        public CommonDbResponse CreateChargeCategory(ChargeCategoryManagementCommon request)
        {
            var sql = $"EXEC  {request.sp} "
                    + $"@categoryName =N{_dao.FilterString(request.categoryName)}"
                    + $",@description= N{_dao.FilterString(request.description)}"
                    + $",@isDefault={_dao.FilterString(request.isDefault)}"
                    + $",@agentType={_dao.FilterString(request.agentType)}"
                    + $",@actionUser=N{_dao.FilterString(request.ActionUser)}"
                    + $",@actionPlatform={_dao.FilterString(request.ActionPlatform)}"
                    + $",@actionIP={_dao.FilterString(request.ActionIP)}"
                    + $",@categoryId={_dao.FilterString(request.categoryId)}";
            return _dao.ParseCommonDbResponse(sql);
        }

        public CommonDbResponse ManageChargeCategoryStatus(ChargeCategoryStatusManagementCommon request)
        {
            var sql = $"EXEC dbo.sproc_admin_manage_charge_category_status "
                    + $"@categoryId ={_dao.FilterString(request.categoryId)}"
                    + $",@status= {_dao.FilterString(request.status)}"
                    + $",@actionUser=N{_dao.FilterString(request.ActionUser)}"
                    + $",@actionPlatform={_dao.FilterString(request.ActionPlatform)}"
                    + $",@actionIP={_dao.FilterString(request.ActionIP)}";
            return _dao.ParseCommonDbResponse(sql);
        }

        public List<ChargeCategoryDetailCommon> GetChargeCategory(string agentType,string categoryId, PaginationFilterCommon dbRequest)
        {
            var response = new List<ChargeCategoryDetailCommon>();
            var sql = $"EXEC dbo.sproc_admin_get_charge_category "
                   + $"@agentType ={_dao.FilterString(agentType)}"
                   + ($", @freeText = {(string.IsNullOrEmpty(dbRequest.SearchFilter) ? "NULL" : $"N{_dao.FilterString(dbRequest.SearchFilter)}")}")
                   + $", @Skip = {(dbRequest.Skip == 0 ? 0 : dbRequest.Skip)}"
                  + $", @Take = {(dbRequest.Take == 0 ? 0 : dbRequest.Take)}" 
                  + $", @categoryId = {_dao.FilterString(categoryId)}";
            var dbResponse = _dao.ExecuteDataTable(sql);
            if (dbResponse != null)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new ChargeCategoryDetailCommon
                    {
                        categoryId = _dao.ParseColumnValue(item, "categoryId").ToString(),
                        categoryName = _dao.ParseColumnValue(item, "categoryName").ToString(),
                        description = _dao.ParseColumnValue(item, "description").ToString(),
                        SNO = Convert.ToInt32(_dao.ParseColumnValue(item, "sNo").ToString()),
                        agentTypeValue = _dao.ParseColumnValue(item, "agentTypeValue").ToString(),
                        TotalRecords = Convert.ToInt32(_dao.ParseColumnValue(item, "totalRecords").ToString()),
                        isDefault = _dao.ParseColumnValue(item, "isDefault").ToString(),
                        agentType = _dao.ParseColumnValue(item, "agentType").ToString(),
                        status = _dao.ParseColumnValue(item, "status").ToString(),
                        creatorUsername = _dao.ParseColumnValue(item, "creatorUsername").ToString(),
                        creatorFullname = _dao.ParseColumnValue(item, "creatorFullname").ToString(),
                        creatorProfileImage = _dao.ParseColumnValue(item, "creatorProfileImage").ToString(),
                        createdDate = _dao.ParseColumnValue(item, "createdDate").ToString(),
                        createdDateUTC = _dao.ParseColumnValue(item, "createdDateUTC").ToString(),
                        createdPlatform = _dao.ParseColumnValue(item, "createdPlatform").ToString(),
                        createdIP = _dao.ParseColumnValue(item, "createdIP").ToString(),
                        isUserCandeleteCategory = _dao.ParseColumnValue(item, "isUserCandeleteCategory").ToString()
                    });
                }
            }
            return response;
        }
        public ChargeCategoryDetailCommon GetChargeCategoryDetails(string agentType,string categoryId, PaginationFilterCommon dbRequest)
        {
            var response = new ChargeCategoryDetailCommon();
            var sql = $"EXEC dbo.sproc_admin_get_charge_category "
                   + $"@agentType ={_dao.FilterString(agentType)}"
                   + ($", @freeText = {(string.IsNullOrEmpty(dbRequest.SearchFilter) ? "NULL" : $"N{_dao.FilterString(dbRequest.SearchFilter)}")}")
                   + $", @Skip = {(dbRequest.Skip == 0 ? 0 : dbRequest.Skip)}"
                  + $", @Take = {(dbRequest.Take == 0 ? 0 : dbRequest.Take)}" 
                  + $", @categoryId = {_dao.FilterString(categoryId)}";

            var dbResponse = _dao.ExecuteDataRow(sql);
            if (dbResponse != null)
            {
                return new ChargeCategoryDetailCommon()
                {
                    categoryId = _dao.ParseColumnValue(dbResponse, "categoryId").ToString(),
                    categoryName = _dao.ParseColumnValue(dbResponse, "categoryName").ToString(),
                    description = _dao.ParseColumnValue(dbResponse, "description").ToString(),
                    SNO = Convert.ToInt32(_dao.ParseColumnValue(dbResponse, "sNo").ToString()),
                    agentTypeValue = _dao.ParseColumnValue(dbResponse, "agentTypeValue").ToString(),
                    TotalRecords = Convert.ToInt32(_dao.ParseColumnValue(dbResponse, "totalRecords").ToString()),
                    isDefault = _dao.ParseColumnValue(dbResponse, "isDefault").ToString(),
                    agentType = _dao.ParseColumnValue(dbResponse, "agentType").ToString(),
                    status = _dao.ParseColumnValue(dbResponse, "status").ToString(),
                    creatorUsername = _dao.ParseColumnValue(dbResponse, "creatorUsername").ToString(),
                    creatorFullname = _dao.ParseColumnValue(dbResponse, "creatorFullname").ToString(),
                    creatorProfileImage = _dao.ParseColumnValue(dbResponse, "creatorProfileImage").ToString(),
                    createdDate = _dao.ParseColumnValue(dbResponse, "createdDate").ToString(),
                    createdDateUTC = _dao.ParseColumnValue(dbResponse, "createdDateUTC").ToString(),
                    createdPlatform = _dao.ParseColumnValue(dbResponse, "createdPlatform").ToString(),
                    createdIP = _dao.ParseColumnValue(dbResponse, "createdIP").ToString()

                };
            }
                   
            return response;
        }
        #endregion
        #region Charge Management
        public CommonDbResponse CreateCharge(ChargeManagementCommon request)
        {
            var sql = $"EXEC dbo.sproc_admin_create_charge_category_detail "
                    + $"@categoryId={_dao.FilterString(request.categoryId)}"
                    + $",@fromAmount= {request.fromAmount}"
                    + $",@toAmount={request.toAmount}"
                    + $",@chargeType={_dao.FilterString(request.chargeType)}"
                    + $",@chargeValue={request.chargeValue}"
                    + $",@minChargeValue={request.minChargeValue}"
                    + $",@maxChargeValue={request.maxChargeValue}"
                    + $",@agentType={_dao.FilterString(request.agentType)}"
                    + $",@actionUser={_dao.FilterString(request.ActionUser)}"
                    + $",@actionPlatform={_dao.FilterString(request.ActionPlatform)}"
                    + $",@actionIP={_dao.FilterString(request.ActionIP)}";
            return _dao.ParseCommonDbResponse(sql);
        }

        public CommonDbResponse UpdateCharge(ChargeManagementCommon request)
        {
            var sql = $"EXEC dbo.sproc_admin_update_charge_category_detail "
                    + $"@categoryId={_dao.FilterString(request.categoryId)}"
                    + $"@categoryDetailId={_dao.FilterString(request.categoryDetailId)}"
                    + $",@fromAmount= {request.fromAmount}"
                    + $",@toAmount={request.toAmount}"
                    + $",@chargeType={_dao.FilterString(request.chargeType)}"
                    + $",@chargeValue={request.chargeValue}"
                    + $",@minChargeValue={request.minChargeValue}"
                    + $",@maxChargeValue={request.maxChargeValue}"
                    + $",@agentType={_dao.FilterString(request.agentType)}"
                    + $",@actionUser={_dao.FilterString(request.ActionUser)}"
                    + $",@actionPlatform={_dao.FilterString(request.ActionPlatform)}"
                    + $",@actionIP={_dao.FilterString(request.ActionIP)}";
            return _dao.ParseCommonDbResponse(sql);
        }

        public CommonDbResponse ManageChargeStatus(ChargeStatusManagementCommon request)
        {
            var sql = $"EXEC dbo.sproc_admin_manage_charge_category_detail_status "
                    + $"@categoryId ={_dao.FilterString(request.categoryId)}"
                    + $"@categoryDetailId ={_dao.FilterString(request.categoryDetailId)}"
                    + $",@status= {_dao.FilterString(request.status)}"
                    + $",@actionUser=N{_dao.FilterString(request.ActionUser)}"
                    + $",@actionPlatform={_dao.FilterString(request.ActionPlatform)}"
                    + $",@actionIP={_dao.FilterString(request.ActionIP)}";
            return _dao.ParseCommonDbResponse(sql);
        }

        public List<ChargeDetailCommon> GetCharge(string categoryId, string categoryDetailId)
        {
            var response = new List<ChargeDetailCommon>();
            var sql = $"EXEC dbo.sproc_admin_get_charge_category_detail "
                   + $"@categoryId ={_dao.FilterString(categoryId)}"
                   + $",@categoryDetailId= {_dao.FilterString(categoryDetailId)}";
            var dbResponse = _dao.ExecuteDataTable(sql);
            if (dbResponse != null)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new ChargeDetailCommon
                    {
                        categoryDetailId = _dao.ParseColumnValue(item, "categoryDetailId").ToString(),
                        categoryId = _dao.ParseColumnValue(item, "categoryId").ToString(),
                        fromAmount = Convert.ToInt32(_dao.ParseColumnValue(item, "fromAmount")),
                        toAmount = Convert.ToInt32(_dao.ParseColumnValue(item, "toAmount")),
                        chargeType = _dao.ParseColumnValue(item, "chargeType").ToString(),
                        chargeValue = Convert.ToInt32(_dao.ParseColumnValue(item, "chargeValue")),
                        minChargeValue = Convert.ToInt32(_dao.ParseColumnValue(item, "minChargeValue")),
                        maxChargeValue = Convert.ToInt32(_dao.ParseColumnValue(item, "maxChargeValue")),
                        status = _dao.ParseColumnValue(item, "status").ToString(),
                        createdBy = _dao.ParseColumnValue(item, "createdBy").ToString(),
                        createdDate = _dao.ParseColumnValue(item, "createdDate").ToString(),
                        createdDateUTC = _dao.ParseColumnValue(item, "createdDateUTC").ToString(),
                        createdPlatform = _dao.ParseColumnValue(item, "createdPlatform").ToString(),
                        createdIP = _dao.ParseColumnValue(item, "createdIP").ToString(),
                        updatedBy = _dao.ParseColumnValue(item, "updatedBy").ToString(),
                        updatedDate = _dao.ParseColumnValue(item, "updatedDate").ToString(),
                        updatedDateUTC = _dao.ParseColumnValue(item, "updatedDateUTC").ToString(),
                        updatedPlatform = _dao.ParseColumnValue(item, "updatedPlatform").ToString(),
                        updatedIP = _dao.ParseColumnValue(item, "updatedIP").ToString()
                    });
                }
            }
            return response;
        }
        #endregion
    }
}
