using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ChargeManagement;
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
            var sql = $"EXEC dbo.sproc_admin_create_charge_category "
                    + $"@categoryName =N{_dao.FilterString(request.categoryName)}"
                    + $",@description= N{_dao.FilterString(request.description)}"
                    + $",@isDefault={_dao.FilterString(request.isDefault)}"
                    + $",@agentType={_dao.FilterString(request.agentType)}"
                    + $",@actionUser=N{_dao.FilterString(request.ActionUser)}"
                    + $",@actionPlatform={_dao.FilterString(request.ActionPlatform)}"
                    + $",@actionIP={_dao.FilterString(request.ActionIP)}";
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

        public List<ChargeCategoryDetailCommon> GetChargeCategory(string agentType, string freeText)
        {
            var response = new List<ChargeCategoryDetailCommon>();
            var sql = $"EXEC dbo.sproc_admin_get_charge_category "
                   + $"@agentType ={_dao.FilterString(agentType)}"
                   + $",@freeText= N{_dao.FilterString(freeText)}";
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
                        isDefault = _dao.ParseColumnValue(item, "isDefault").ToString(),
                        agentType = _dao.ParseColumnValue(item, "agentType").ToString(),
                        status = _dao.ParseColumnValue(item, "status").ToString(),
                        creatorUsername = _dao.ParseColumnValue(item, "creatorUsername").ToString(),
                        creatorFullname = _dao.ParseColumnValue(item, "creatorFullname").ToString(),
                        creatorProfileImage = _dao.ParseColumnValue(item, "creatorProfileImage").ToString(),
                        createdDate = _dao.ParseColumnValue(item, "createdDate").ToString(),
                        createdDateUTC = _dao.ParseColumnValue(item, "createdDateUTC").ToString(),
                        createdPlatform = _dao.ParseColumnValue(item, "createdPlatform").ToString(),
                        createdIP = _dao.ParseColumnValue(item, "createdIP").ToString()
                    });
                }
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
