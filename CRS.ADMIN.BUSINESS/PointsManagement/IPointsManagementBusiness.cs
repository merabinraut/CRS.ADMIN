using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PointsManagement;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.PointsManagement
{
    public interface IPointsManagementBusiness
    {
        List<PointsTansferReportCommon> GetPointTransferList(PointsManagementCommon objPointsTansferReportCommon = null, PaginationFilterCommon objPaginationFilterCommon = null);
        CommonDbResponse ManagePoints(PointsTansferCommon objPointsTansferCommon);

        CommonDbResponse ManagePointsRequest(PointsRequestCommon objPointsRequestCommon);
        #region Point transfer List
        List<PointRequestListCommon> GetPointRequestList(PointRequestListFilterCommon request);
        CommonDbResponse ManageClubPointRequest(ManageClubPointRequestCommon request);
        List<PointBalanceStatementResponseCommon> GetPointBalanceStatementDetailsAsync(PointBalanceStatementRequestCommon requestModel=null);
        List<SystemTransferReponseCommon> GetSystemTransferDetailsAsync(SystemTransferRequestCommon mappedObject);
        #endregion
        PointsTansferRetriveDetailsCommon GetPointTransferDetails(string id);
    }
}
