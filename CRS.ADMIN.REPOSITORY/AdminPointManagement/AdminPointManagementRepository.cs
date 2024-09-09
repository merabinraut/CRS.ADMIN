using CRS.ADMIN.SHARED.PointsManagement;
using CRS.ADMIN.SHARED;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRS.ADMIN.SHARED.AdminPointManagement;

namespace CRS.ADMIN.REPOSITORY.AdminPointManagement
{
    public class AdminPointManagementRepository: IAdminPointManagementRepository
    {
        private readonly RepositoryDao _dao;
        public AdminPointManagementRepository(RepositoryDao dao)
        {
            _dao = dao;
        }
        public CommonDbResponse ManageAdminPointsRequest(ManagePointRequestCommon objManagePointRequestCommon)
        {
            string SQL = "EXEC sproc_insert_admin_current_point_balance";
            SQL += " @amount=" + _dao.FilterString(objManagePointRequestCommon.point);
            SQL += " ,@points=" + _dao.FilterString(objManagePointRequestCommon.point);
            SQL += ",@remark=" + _dao.FilterString(objManagePointRequestCommon.remarks);
            SQL += ",@ActionUser=" + _dao.FilterString(objManagePointRequestCommon.actionUser);
            SQL += ",@actionIp=" + _dao.FilterString(objManagePointRequestCommon.actionIp);
            return _dao.ParseCommonDbResponse(SQL);
        }
    }
}
