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
    public interface IPointsManagementBusiness
    {
        List<PointsTansferReportCommon> GetPointTransferList(PointsManagementCommon objPointsTansferReportCommon = null, PaginationFilterCommon objPaginationFilterCommon = null);
        CommonDbResponse ManagePoints(PointsTansferCommon objPointsTansferCommon);
        CommonDbResponse ManagePointsRequest(PointsRequestCommon objPointsRequestCommon);
    }
}
