using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PointsManagement;
using DocumentFormat.OpenXml.Office2016.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CRS.ADMIN.REPOSITORY.PointsManagement
{
    public class PointsManagementRepository : IPointsManagementRepository
    {
        RepositoryDao _DAO;
        public PointsManagementRepository()
        {
            _DAO = new RepositoryDao();
        }
        public List<PointsTansferReportCommon> GetPointTransferList(PointsManagementCommon objPointsTansferReportCommon = null, PaginationFilterCommon objPaginationFilterCommon = null)
        {
            var response = new List<PointsTansferReportCommon>();
            string SQL = "EXEC sproc_admin_point_transfer_retrieve_report ";

            SQL += !string.IsNullOrEmpty(objPaginationFilterCommon.SearchFilter) ? " @SearchFilter=N" + _DAO.FilterString(objPaginationFilterCommon.SearchFilter) : " @SearchFilter=null ";
            SQL += !string.IsNullOrEmpty(objPointsTansferReportCommon.UserType) ? " ,@UserTypeId=" + _DAO.FilterString(objPointsTansferReportCommon.UserType) : " ,@UserTypeId=null ";
            SQL += !string.IsNullOrEmpty(objPointsTansferReportCommon.UserName) ? ",@UserId=" + _DAO.FilterString(objPointsTansferReportCommon.UserName) : ",@UserId=null";
            SQL += !string.IsNullOrEmpty(objPointsTansferReportCommon.TransferTypeId) ? " ,@TransactionType=" + _DAO.FilterString(objPointsTansferReportCommon.TransferTypeId) : " ,@TransactionType =null";
            SQL += !string.IsNullOrEmpty(objPointsTansferReportCommon.FromDate) ? ",@FromDate=" + _DAO.FilterString(objPointsTansferReportCommon.FromDate) : ",@FromDate=null";
            SQL += !string.IsNullOrEmpty(objPointsTansferReportCommon.ToDate) ? " ,@ToDate=" + _DAO.FilterString(objPointsTansferReportCommon.ToDate) : " ,@ToDate =null";

            SQL += " ,@Skip=" + objPaginationFilterCommon.Skip;
            SQL += ",@Take=" + objPaginationFilterCommon.Take;
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {

                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new PointsTansferReportCommon()
                    {
                        SNO = Convert.ToInt32(_DAO.ParseColumnValue(item, "sNo").ToString()),
                        TotalRecords = Convert.ToInt32(_DAO.ParseColumnValue(item, "totalRecords").ToString()),
                        TransactionId = Convert.ToString(_DAO.ParseColumnValue(item, "transactionId")),
                        TransactionType = Convert.ToString(_DAO.ParseColumnValue(item, "transactionTypeName")),
                        TransactionDate = !string.IsNullOrEmpty(_DAO.ParseColumnValue(item, "transactionDate").ToString()) ? DateTime.Parse(_DAO.ParseColumnValue(item, "transactionDate").ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : _DAO.ParseColumnValue(item, "transactionDate").ToString(),
                        UserType = Convert.ToString(_DAO.ParseColumnValue(item, "userType")),
                        FromUser = Convert.ToString(_DAO.ParseColumnValue(item, "fromUser")),
                        ToUser = Convert.ToString(_DAO.ParseColumnValue(item, "toUser")),
                        Points = Convert.ToString(_DAO.ParseColumnValue(item, "points")),
                        Remarks = Convert.ToString(_DAO.ParseColumnValue(item, "remark")),
                        Id = Convert.ToString(_DAO.ParseColumnValue(item, "id"))

                    });
                }
            }
            return response;
        }
        public CommonDbResponse ManagePoints(PointsTansferCommon objPointsTansferCommon)
        {
            var Sp = objPointsTansferCommon.SpName;
            string SQL = $"EXEC {Sp}"; 
            SQL += " @roleType=" + _DAO.FilterString(objPointsTansferCommon.UserTypeId);
            SQL += ",@agentId=" + _DAO.FilterString(objPointsTansferCommon.UserId);
            //SQL += ",@transactionType=" + _DAO.FilterString(objPointsTansferCommon.TransferType);
            SQL += ",@points=" + _DAO.FilterString(objPointsTansferCommon.Points);
            SQL += ",@remark=N" + _DAO.FilterString(objPointsTansferCommon.Remarks);
            SQL += ",@receiptPhoto=" + _DAO.FilterString(objPointsTansferCommon.Image);
            SQL += ",@actionUser=" + _DAO.FilterString(objPointsTansferCommon.ActionUser);
            SQL += ",@actionIP=" + _DAO.FilterString(objPointsTansferCommon.ActionIP);
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
        #region Point transfer List
        public List<PointRequestListCommon> GetPointRequestList(PointRequestListFilterCommon request)
        {
            var response = new List<PointRequestListCommon>();
            //string SQL = "EXEC sproc_club_to_admin_payment_transaction_list";
            string SQL = "EXEC sproc_admin_payment_transaction_list";
            SQL += !string.IsNullOrEmpty(request.SearchFilter) ? " @SearchFilter=N" + _DAO.FilterString(request.SearchFilter) : " @SearchFilter= ''";
            SQL += !string.IsNullOrEmpty(request.ClubName) ? ",@ClubName=N" + _DAO.FilterString(request.ClubName) : string.Empty;
            SQL += !string.IsNullOrEmpty(request.PaymentMethodId) ? ",@TxnType=" + _DAO.FilterString(request.PaymentMethodId) : string.Empty;
            SQL += !string.IsNullOrEmpty(request.FromDate) ? ",@FromDate=" + _DAO.FilterString(request.FromDate) : string.Empty;
            SQL += !string.IsNullOrEmpty(request.ToDate) ? ",@ToDate=" + _DAO.FilterString(request.ToDate) : string.Empty;
            SQL += ",@Skip=" + request.Skip;           
            SQL += !string.IsNullOrEmpty(request.LocationId) ? ",@LocationId=" + _DAO.FilterString(request.LocationId) : string.Empty;

            SQL += ",@Take=" + request.Take;
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null)
                response = _DAO.DataTableToListObject<PointRequestListCommon>(dbResponse).ToList();
            return response;
        }

        public CommonDbResponse ManageClubPointRequest(ManageClubPointRequestCommon request)
        {
            string SQL = "EXEC sproc_admin_point_request_approvalrejection ";
            //SQL += "@AgentId=" + _DAO.FilterString(request.AgentId);
            //SQL += ",@UserId=" + _DAO.FilterString(request.UserId);
            SQL += " @sno=" + _DAO.FilterString(request.Id);
            SQL += ",@status=" + _DAO.FilterString(request.Status);
            SQL += ",@remark=N" + _DAO.FilterString(request.AdminRemark);
            SQL += ",@image=" + _DAO.FilterString(request.ImageURL);
            SQL += ",@actionUser=" + _DAO.FilterString(request.ActionUser);
            SQL += ",@actionIP=" + _DAO.FilterString(request.ActionIP);
            return _DAO.ParseCommonDbResponse(SQL);
        }

        public List<PointBalanceStatementResponseCommon> GetPointBalanceStatementDetailsAsync(PointBalanceStatementRequestCommon request)
        {
            var response = new List<PointBalanceStatementResponseCommon>();
            string SQL = "EXEC sproc_club_GetPoint_Balance_StatementDetails ";
            SQL += !string.IsNullOrEmpty(request.SearchFilter) ? " @search=N" + _DAO.FilterString(request.SearchFilter) : " @search=null ";
            //SQL += !string.IsNullOrEmpty(request.UserTypeList) ? " ,@UserTypeId=" + _DAO.FilterString(request.UserTypeList) : " ,@UserTypeId=null ";
            SQL += !string.IsNullOrEmpty(request.UserNameList) ? ",@ClubId=" + _DAO.FilterString(request.UserNameList) : ",@ClubId=null";
            SQL += !string.IsNullOrEmpty(request.TransferTypeList) ? " ,@TranscationType=" + _DAO.FilterString(request.TransferTypeList) : " ,@TranscationType =null";
            SQL += !string.IsNullOrEmpty(request.From_Date) ? ",@FromDate=" + _DAO.FilterString(request.From_Date) : ",@FromDate=null";
            SQL += !string.IsNullOrEmpty(request.To_Date) ? " ,@ToDate=" + _DAO.FilterString(request.To_Date) : " ,@ToDate =null";
            SQL += " ,@Row=" + request.Skip;
            SQL += ",@Fetch=" + request.Take;
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {

                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new PointBalanceStatementResponseCommon()
                    {
                        SNO = Convert.ToInt32(_DAO.ParseColumnValue(item, "SN").ToString()),
                        RowTotal = Convert.ToString(_DAO.ParseColumnValue(item, "RowTotal").ToString()),                        
                        TransactionId = Convert.ToString(_DAO.ParseColumnValue(item, "TranscationID")),
                        TransactionDate = !string.IsNullOrEmpty(_DAO.ParseColumnValue(item, "TransactionDate").ToString()) ? DateTime.Parse(_DAO.ParseColumnValue(item, "TransactionDate").ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : _DAO.ParseColumnValue(item, "TransactionDate").ToString(),
                        TransactionType = Convert.ToString(_DAO.ParseColumnValue(item, "TransactionType")),
                        User = Convert.ToString(_DAO.ParseColumnValue(item, "Users")),
                        TotalPrice = Convert.ToInt64(_DAO.ParseColumnValue(item, "TotalAmount")).ToString("N0"),
                        TotalCommission = Convert.ToInt64(_DAO.ParseColumnValue(item, "TotalCommissionAmount")).ToString("N0"),
                        Remarks = Convert.ToString(_DAO.ParseColumnValue(item, "Remark")),
                        Credit = Convert.ToInt64(_DAO.ParseColumnValue(item, "Credit")).ToString("N0"),
                        Debit = Convert.ToInt64(_DAO.ParseColumnValue(item, "Debit")).ToString("N0")
                    });
                }
            }
            return response;
        }

        public List<SystemTransferReponseCommon> GetSystemTransferDetailsAsync(SystemTransferRequestCommon request)
        {

            var response = new List<SystemTransferReponseCommon>();
            string SQL = "EXEC sproc_get_system_transferReport ";
            SQL += !string.IsNullOrEmpty(request.SearchFilter) ? " @search=N" + _DAO.FilterString(request.SearchFilter) : " @search=null ";
            //SQL += !string.IsNullOrEmpty(request.UserTypeList) ? " ,@UserTypeId=" + _DAO.FilterString(request.UserTypeList) : " ,@UserTypeId=null ";
            SQL += !string.IsNullOrEmpty(request.User_name) ? ",@ClubId=" + _DAO.FilterString(request.User_name) : ",@ClubId=null";
            SQL += !string.IsNullOrEmpty(request.TransferType) ? " ,@TranscationType=" + _DAO.FilterString(request.TransferType) : " ,@TranscationType =null";
            SQL += !string.IsNullOrEmpty(request.From_Date1) ? ",@FromDate=" + _DAO.FilterString(request.From_Date1) : ",@FromDate=null";
            SQL += !string.IsNullOrEmpty(request.To_Date1) ? " ,@ToDate=" + _DAO.FilterString(request.To_Date1) : " ,@ToDate =null";
            SQL += " ,@Row=" + request.Skip;
            SQL += ",@Fetch=" + request.Take;
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new SystemTransferReponseCommon()
                    {
                        SNO = Convert.ToInt32(_DAO.ParseColumnValue(item, "SN").ToString()),
                        RowTotal = Convert.ToString(_DAO.ParseColumnValue(item, "RowTotal").ToString()),
                        TransactionId = Convert.ToString(_DAO.ParseColumnValue(item, "TranscationID")),
                        TransactionDate = Convert.ToString(_DAO.ParseColumnValue(item, "TransactionDate")),
                        TransactionType = Convert.ToString(_DAO.ParseColumnValue(item, "TransactionType")),
                        UserName = Convert.ToString(_DAO.ParseColumnValue(item, "Users")),
                        UserType = Convert.ToString(_DAO.ParseColumnValue(item, "UserType")),
                        Points = Convert.ToString(_DAO.ParseColumnValue(item, "Point")),
                        Remarks = Convert.ToString(_DAO.ParseColumnValue(item, "Remark"))                 
                    });
                }
            }
            return response;
        }

        #endregion

        public PointsTansferRetriveDetailsCommon GetPointTransferDetails(string id)
        {
           
            string SQL = "sproc_admin_point_transfer_retrieve_details ";
            SQL += " @Id=" + _DAO.FilterString(id);
            var dbResponse = _DAO.ExecuteDataRow(SQL);
            if (dbResponse != null)
            {
                return new PointsTansferRetriveDetailsCommon()
                {
                   
                    
                    TransactionId = _DAO.ParseColumnValue(dbResponse, "transactionId").ToString(),
                    TransactionType = _DAO.ParseColumnValue(dbResponse, "transactionTypeName").ToString(),
                    Remarks = _DAO.ParseColumnValue(dbResponse, "remarks").ToString(),
                    FromUser = _DAO.ParseColumnValue(dbResponse, "fromUser").ToString(),
                    ToUser = _DAO.ParseColumnValue(dbResponse, "toUser").ToString(),
                    UserType = _DAO.ParseColumnValue(dbResponse, "userType").ToString(),
                    Points = _DAO.ParseColumnValue(dbResponse, "points").ToString(),
                    Image = _DAO.ParseColumnValue(dbResponse, "receiptPhoto").ToString(),                   
                    TransactionDate = !string.IsNullOrEmpty(_DAO.ParseColumnValue(dbResponse, "transactionDate").ToString()) ? Convert.ToDateTime(_DAO.ParseColumnValue(dbResponse, "transactionDate")).ToString("yyyy/MM/dd") : _DAO.ParseColumnValue(dbResponse, "transactionDate").ToString(),                    
                   
                };
            }

            return new PointsTansferRetriveDetailsCommon();
        }
    }
}
