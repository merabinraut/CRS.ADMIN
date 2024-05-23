using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.StaticDataManagement;
using System.Collections.Generic;
using System.Linq;

namespace CRS.ADMIN.REPOSITORY.StaticDataManagement
{
    public class StaticDataManagementRepository : IStaticDataManagementRepository
    {
        RepositoryDao _dao;
        public StaticDataManagementRepository()
        {
            _dao = new RepositoryDao();
        }

        #region MANAGE STATIC DATA TYPE
        public CommonDbResponse DeleteStaticDataType(ManageStaticDataTypeCommon request)
        {
            string sp_name = "sproc_tbl_static_data_Type_Delete ";
            sp_name += "@Id" + _dao.FilterString(request.Id);
            sp_name += ",@ActionUser" + _dao.FilterString(request.ActionUser);
            return _dao.ParseCommonDbResponse(sp_name);
        }

        public ManageStaticDataTypeCommon GetStaticDataTypeDetail(string id)
        {
            string sp_name = "EXEC sproc_tbl_static_data_Type_Select ";
            sp_name += "@Id=" + _dao.FilterString(id);
            var dbResponseInfo = _dao.ExecuteDataRow(sp_name);
            if (dbResponseInfo != null)
            {
                return new ManageStaticDataTypeCommon()
                {
                    Id = _dao.ParseColumnValue(dbResponseInfo, "Id").ToString(),
                    StaticDataType = _dao.ParseColumnValue(dbResponseInfo, "StaticDataType").ToString(),
                    StaticDataName = _dao.ParseColumnValue(dbResponseInfo, "StaticDataName").ToString(),
                    StaticDataDescription = _dao.ParseColumnValue(dbResponseInfo, "StaticDataDescription").ToString(),
                    Status = _dao.ParseColumnValue(dbResponseInfo, "Status").ToString()
                };
            }
            return new ManageStaticDataTypeCommon();
        }

        public List<StaticDataTypeCommon> GetStatiDataTypeList(string SearchText = "")
        {
            string sp_name = "EXEC sproc_tbl_static_data_type_list ";
            sp_name += "@Search=" + _dao.FilterString(SearchText);
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null && dbResponseInfo.Rows.Count > 0) return _dao.DataTableToListObject<StaticDataTypeCommon>(dbResponseInfo).ToList();
            return new List<StaticDataTypeCommon>();
        }

        public CommonDbResponse ManageStaticDataType(ManageStaticDataTypeCommon commonModel)
        {
            string sp_name = "EXEC sproc_tbl_static_data_Type_InserUpdate ";
            sp_name += "@Id=" + _dao.FilterString(commonModel.Id);
            sp_name += ",@StaticDataType=" + _dao.FilterString(commonModel.StaticDataType);
            sp_name += ",@StaticDataName=" + _dao.FilterString(commonModel.StaticDataName);
            sp_name += ",@StaticDataDescription=" + _dao.FilterString(commonModel.StaticDataDescription);
            sp_name += ",@Status=" + _dao.FilterString("A");
            sp_name += ",@ActionUser=" + _dao.FilterString(commonModel.ActionUser);
            return _dao.ParseCommonDbResponse(sp_name);
        }
        #endregion

        #region MANAGE STATIC DATA
        public List<StaticDataModelCommon> GetStaticDataList(string staticDataTypeId)
        {
            string sp_name = "EXEC sproc_admin__manage_static_data @Flag='gsdl'";
            sp_name += ",@StaticDataType=" + _dao.FilterString(staticDataTypeId);
            var dbResponseInfo = _dao.ExecuteDataTable(sp_name);
            if (dbResponseInfo != null && dbResponseInfo.Rows.Count > 0) return _dao.DataTableToListObject<StaticDataModelCommon>(dbResponseInfo).ToList();
            return new List<StaticDataModelCommon>();
        }

        public ManageStaticDataCommon GetStaticDataDetail(string id)
        {
            string sp_name = "EXEC sproc_admin__manage_static_data @Flag='gsdd'";
            sp_name += " @Id=" + _dao.FilterString(id);
            var dbResponse = _dao.ExecuteDataRow(sp_name);
            if (dbResponse != null)
            {
                return new ManageStaticDataCommon()
                {
                    Id = _dao.ParseColumnValue(dbResponse, "Id").ToString(),
                    StaticDataType = _dao.ParseColumnValue(dbResponse, "StaticDataType").ToString(),
                    StaticDataLabel = _dao.ParseColumnValue(dbResponse, "StaticDataLabel").ToString(),
                    StaticDataDescription = _dao.ParseColumnValue(dbResponse, "StaticDataDescription").ToString(),
                    Status = _dao.ParseColumnValue(dbResponse, "Status").ToString(),
                };
            }
            return new ManageStaticDataCommon();
        }

        public CommonDbResponse ManageStaticData(ManageStaticDataCommon commonModel)
        {
            string sp_name = "EXEC sproc_admin__manage_static_data ";
            sp_name += string.IsNullOrEmpty(commonModel.Id) ? "@Flag='msd'" : "@Flag='umsd'";
            sp_name += ",@StaticDataLabel" + _dao.FilterString(commonModel.StaticDataLabel);
            sp_name += ",@StaticDataDescription=" + _dao.FilterString(commonModel.StaticDataDescription);
            sp_name += ",@StaticDataType=" + _dao.FilterString(commonModel.StaticDataType);
            sp_name += ",@Status=" + _dao.FilterString("A");
            sp_name += ",@ActionUser=" + _dao.FilterString(commonModel.ActionUser);
            return _dao.ParseCommonDbResponse(sp_name);
        }
        #endregion
    }
}
