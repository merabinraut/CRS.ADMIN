using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.StaticDataManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.REPOSITORY.StaticDataManagement
{
    public interface IStaticDataManagementRepository
    {
        #region MANAGE STATIC DATA TYPE
        ManageStaticDataTypeCommon GetStaticDataTypeDetail(string id);
        List<StaticDataTypeCommon> GetStatiDataTypeList(string SearchText);
        CommonDbResponse ManageStaticDataType(ManageStaticDataTypeCommon commonModel);
        CommonDbResponse DeleteStaticDataType(ManageStaticDataTypeCommon request);
        #endregion

        #region MANAGE STATIC DATA
        List<StaticDataModelCommon> GetStaticDataList(string staticDataTypeId);
        ManageStaticDataCommon GetStaticDataDetail(string id);
        CommonDbResponse ManageStaticData(ManageStaticDataCommon commonModel);
        #endregion
    }
}
