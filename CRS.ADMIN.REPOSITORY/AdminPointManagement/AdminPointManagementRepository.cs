using CRS.ADMIN.SHARED.PointsManagement;
using CRS.ADMIN.SHARED;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRS.ADMIN.SHARED.AdminPointManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System.Data;
using DocumentFormat.OpenXml.Bibliography;

namespace CRS.ADMIN.REPOSITORY.AdminPointManagement
{
    public class AdminPointManagementRepository: IAdminPointManagementRepository
    {
        private readonly RepositoryDao _dao;
        public AdminPointManagementRepository(RepositoryDao dao)
        {
            _dao = dao;
        }
        public List<AdminPointManagementCommon> GetManualEntryPointList(PointRequestDetailCommon objPointRequestDetailCommon = null, PaginationFilterCommon objPaginationFilterCommon = null)
        {
            var response = new List<AdminPointManagementCommon>();
            string SQL = "EXEC sproc_get_manual_entry_record ";
            SQL += !string.IsNullOrEmpty(objPointRequestDetailCommon.fromDate) ? "@fromDate=" + _dao.FilterString(objPointRequestDetailCommon.fromDate) : "@fromDate=NULL";
            SQL += !string.IsNullOrEmpty(objPointRequestDetailCommon.toDate) ? " ,@toDate=" + _dao.FilterString(objPointRequestDetailCommon.toDate) : " ,@toDate =NULL";
            SQL += !string.IsNullOrEmpty(objPointRequestDetailCommon.txnId) ? " ,@txnId=" + _dao.FilterString(objPointRequestDetailCommon.txnId) : " ,@txnId =NULL";
            //SQL += " ,@Skip=" + objPaginationFilterCommon.Skip;
            //SQL += ",@Take=" + objPaginationFilterCommon.Take;
            var dbResponse = _dao.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {

                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new AdminPointManagementCommon()
                    {
                        txnId = Convert.ToString(_dao.ParseColumnValue(item, "txnId")),
                        txnDate = !string.IsNullOrEmpty(_dao.ParseColumnValue(item, "createdDate").ToString()) ? DateTime.Parse(_dao.ParseColumnValue(item, "createdDate").ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : _dao.ParseColumnValue(item, "createdDate").ToString(),
                        point = Convert.ToString(_dao.ParseColumnValue(item, "points")),
                        remarks = Convert.ToString(_dao.ParseColumnValue(item, "remark"))

                    });
                }
            }
            return response;
        }
        public CommonDbResponse ManageAdminPointsRequest(ManagePointRequestCommon objManagePointRequestCommon)
        {
            string SQL = "EXEC sproc_insert_admin_current_point_balance";
            SQL += " @amount=" + _dao.FilterString(objManagePointRequestCommon.point);
            SQL += " ,@points=" + _dao.FilterString(objManagePointRequestCommon.point);
            SQL += !string.IsNullOrEmpty(objManagePointRequestCommon.remarks) ? " ,@remark=N" + _dao.FilterString(objManagePointRequestCommon.remarks) : " ,@remark=NULL";  
            SQL += ",@ActionUser=" + _dao.FilterString(objManagePointRequestCommon.actionUser);
            SQL += ",@actionIp=" + _dao.FilterString(objManagePointRequestCommon.actionIp);
            return _dao.ParseCommonDbResponse(SQL);
        }
    }
}
