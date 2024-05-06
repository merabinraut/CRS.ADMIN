using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PointSetup;
using CRS.ADMIN.SHARED.PointsManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.REPOSITORY.PointsManagement
{
    public class PointsManagementRepository: IPointsManagementRepository
    {
        RepositoryDao _DAO;
        public PointsManagementRepository()
        {
            _DAO = new RepositoryDao();
        }
        public List<PointsTansferReportCommon> GetPointTransferList(PointsManagementCommon objPointsTansferReportCommon=null, PaginationFilterCommon objPaginationFilterCommon = null)
        {
            var response = new List<PointsTansferReportCommon>();
            string SQL = "EXEC sproc_admin_point_transfer_retrieve_select ";
            SQL += !string.IsNullOrEmpty(objPaginationFilterCommon.SearchFilter) ? " @SearchFilter=N" + _DAO.FilterString(objPaginationFilterCommon.SearchFilter) : " @SearchFilter=null ";            
            SQL += !string.IsNullOrEmpty(objPointsTansferReportCommon.UserType) ? " ,@UserTypeId=" + _DAO.FilterString(objPointsTansferReportCommon.UserType): " ,@UserTypeId=null " ;
            SQL += !string.IsNullOrEmpty(objPointsTansferReportCommon.UserName) ?  ",@UserId=" + _DAO.FilterString(objPointsTansferReportCommon.UserName): ",@UserId=null";
            SQL += !string.IsNullOrEmpty(objPointsTansferReportCommon.TransferTypeId) ? " ,@TransactionType=" + _DAO.FilterString(objPointsTansferReportCommon.TransferTypeId): " ,@TransactionType =null";
            SQL += !string.IsNullOrEmpty(objPointsTansferReportCommon.FromDate) ? ",@FromDate=" +_DAO.FilterString(objPointsTansferReportCommon.FromDate) : ",@FromDate=null";
            SQL += !string.IsNullOrEmpty(objPointsTansferReportCommon.ToDate) ? " ,@ToDate=" + _DAO.FilterString(objPointsTansferReportCommon.ToDate): " ,@ToDate =null";

            SQL += " ,@Skip=" + objPaginationFilterCommon.Skip;
            SQL += ",@Take=" + objPaginationFilterCommon.Take;
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {

                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new PointsTansferReportCommon()
                    {
                        SNO = Convert.ToInt32(_DAO.ParseColumnValue(item, "SNO").ToString()),
                        TotalRecords = Convert.ToInt32(_DAO.ParseColumnValue(item, "TotalRecords").ToString()),
                        TransactionId = Convert.ToString(_DAO.ParseColumnValue(item, "TransactionId")),
                        TransactionType = Convert.ToString(_DAO.ParseColumnValue(item, "TransactionTypeName")),
                        TransactionDate = Convert.ToString(_DAO.ParseColumnValue(item, "TransactionDate")),
                        UserType = Convert.ToString(_DAO.ParseColumnValue(item, "UserType")),
                        FromUser = Convert.ToString(_DAO.ParseColumnValue(item, "FromUser")),
                        ToUser = Convert.ToString(_DAO.ParseColumnValue(item, "ToUser")),
                        Points = Convert.ToString(_DAO.ParseColumnValue(item, "Amount")),
                        Remarks = Convert.ToString(_DAO.ParseColumnValue(item, "Remark"))

                    });
                }
            }
            return response;
        }
        public CommonDbResponse ManagePoints(PointsTansferCommon objPointsTansferCommon)
        {
            string SQL = "EXEC sproc_admin_point_transfer_retrieve ";
            SQL += " @UserTypeId=" + _DAO.FilterString(objPointsTansferCommon.UserTypeId);
            SQL += ",@UserId=" + _DAO.FilterString(objPointsTansferCommon.UserId);
            SQL += ",@transactionType=" + _DAO.FilterString(objPointsTansferCommon.TransferType);
            SQL += ",@Point=" + _DAO.FilterString(objPointsTansferCommon.Points);
            SQL += ",@Remarks=N" + _DAO.FilterString(objPointsTansferCommon.Remarks);
            SQL += ",@Image=" + _DAO.FilterString(objPointsTansferCommon.Image);
            SQL += ",@ActionUser=" + _DAO.FilterString(objPointsTansferCommon.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(objPointsTansferCommon.ActionIP);
            return _DAO.ParseCommonDbResponse(SQL);
        }
        public CommonDbResponse ManagePointsRequest(PointsRequestCommon objPointsRequestCommon)
        {
            string SQL = "EXEC sproc_admin_point_request ";
            SQL += " @Point=" + _DAO.FilterString(objPointsRequestCommon.Points);
            SQL += ",@Remarks=" + _DAO.FilterString(objPointsRequestCommon.Remarks);
            SQL += ",@ActionUser=" + _DAO.FilterString(objPointsRequestCommon.ActionUser);
            return _DAO.ParseCommonDbResponse(SQL);
        }

    }
}
