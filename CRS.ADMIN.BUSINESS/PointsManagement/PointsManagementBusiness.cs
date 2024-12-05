using CRS.ADMIN.REPOSITORY.PointsManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PointsManagement;
using DocumentFormat.OpenXml.Office2016.Excel;
using System.Collections.Generic;

namespace CRS.ADMIN.BUSINESS.PointsManagement
{
    public class PointsManagementBusiness : IPointsManagementBusiness
    {
        IPointsManagementRepository _REPO;
        public PointsManagementBusiness(PointsManagementRepository REPO)
        {
            _REPO = REPO;
        }

        public List<PointsTansferReportCommon> GetPointTransferList(PointsManagementCommon objPointsTansferReportCommon = null, PaginationFilterCommon objPaginationFilterCommon = null)
        {
            return _REPO.GetPointTransferList(objPointsTansferReportCommon, objPaginationFilterCommon);
        }
        public CommonDbResponse ManagePoints(PointsTansferCommon objPointsTansferCommon)
        {
            return _REPO.ManagePoints(objPointsTansferCommon);
        }

        public CommonDbResponse ManagePointsRequest(PointsRequestCommon objPointsRequestCommon)
        {
            return _REPO.ManagePointsRequest(objPointsRequestCommon);
        }
        #region Point transfer List
        public List<PointRequestListCommon> GetPointRequestList(PointRequestListFilterCommon request)
        {
            return _REPO.GetPointRequestList(request);
        }
        public CommonDbResponse ManageClubPointRequest(ManageClubPointRequestCommon request)
        {
            return _REPO.ManageClubPointRequest(request);
        }

        public List<PointBalanceStatementResponseCommon> GetPointBalanceStatementDetailsAsync(PointBalanceStatementRequestCommon requestModel)
        {
            return _REPO.GetPointBalanceStatementDetailsAsync(requestModel);
        }

        public List<SystemTransferReponseCommon> GetSystemTransferDetailsAsync(SystemTransferRequestCommon mappedObject)
        {
            return _REPO.GetSystemTransferDetailsAsync(mappedObject);
        }
        public PointsTansferRetriveDetailsCommon GetPointTransferDetails(string id)
        {
            return _REPO.GetPointTransferDetails(id);
        }

        #endregion
    }
}
