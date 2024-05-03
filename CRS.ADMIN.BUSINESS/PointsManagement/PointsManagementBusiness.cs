using CRS.ADMIN.REPOSITORY.PointSetup;
using CRS.ADMIN.REPOSITORY.PointsManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PointsManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return  _REPO.GetPointTransferList(objPointsTansferReportCommon,objPaginationFilterCommon);
        } 
        public CommonDbResponse ManagePoints(PointsTansferCommon objPointsTansferCommon)
        {
            return  _REPO.ManagePoints(objPointsTansferCommon);
        }
        public CommonDbResponse ManagePointsRequest(PointsRequestCommon objPointsRequestCommon)
        {
            return  _REPO.ManagePointsRequest(objPointsRequestCommon);
        }
    }
}
