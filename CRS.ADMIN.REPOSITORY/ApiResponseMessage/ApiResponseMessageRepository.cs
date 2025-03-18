using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ApiResponseMessage;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.REPOSITORY.ApiResponseMessage
{
    public class ApiResponseMessageRepository : IApiResponseMessageRepository
    {
        RepositoryDao _DAO;
        public ApiResponseMessageRepository()
        {
            _DAO = new RepositoryDao();
        }

        public ApiResponseMessageCommon ApiResponseMessageDetail(string MessageId = "")
        {
            string SQL = "EXEC sproc_api_response_message_list @Flag='d'";
            SQL += ",@messageId=" + _DAO.FilterString(MessageId);
            var dbResponse = _DAO.ExecuteDataRow(SQL);

            if (dbResponse != null)
            {
                return new ApiResponseMessageCommon()
                {
                    Code = _DAO.ParseColumnValue(dbResponse, "code").ToString(),
                    Message = _DAO.ParseColumnValue(dbResponse, "message").ToString(),
                    MessageEng = _DAO.ParseColumnValue(dbResponse, "messageEng").ToString(),
                    Category = _DAO.ParseColumnValue(dbResponse, "category").ToString(),
                    Description = _DAO.ParseColumnValue(dbResponse, "description").ToString(),
                    MessageType = _DAO.ParseColumnValue(dbResponse, "messageType").ToString(),
                    HttpStatusCode = _DAO.ParseColumnValue(dbResponse, "httpStatusCode").ToString(),
                    MessageId = _DAO.ParseColumnValue(dbResponse, "sno").ToString()

                };
            }

            return new ApiResponseMessageCommon();

        }

        public List<ApiResponseMessageCommon> ApiResponseMessageList(PaginationFilterCommon Request)
        {
            var response = new List<ApiResponseMessageCommon>();
            string SQL = "EXEC sproc_api_response_message_list @Flag='s'";
            SQL += !string.IsNullOrEmpty(Request.SearchFilter) ? ",@SearchFilter=N" + _DAO.FilterString(Request.SearchFilter) : null;
            SQL += ",@Skip=" + Request.Skip;
            SQL += ",@Take=" + Request.Take;
            var dbResponse = _DAO.ExecuteDataTable(SQL);

            if (dbResponse != null)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new ApiResponseMessageCommon()
                    {
                        Code = _DAO.ParseColumnValue(item, "code").ToString(),
                        Message = _DAO.ParseColumnValue(item, "message").ToString(),
                        MessageEng = _DAO.ParseColumnValue(item, "messageEng").ToString(),
                        Category = _DAO.ParseColumnValue(item, "category").ToString(),
                        Description = _DAO.ParseColumnValue(item, "description").ToString(),
                        MessageType = _DAO.ParseColumnValue(item, "messageType").ToString(),
                        HttpStatusCode = _DAO.ParseColumnValue(item, "httpStatusCode").ToString(),
                        MessageId = _DAO.ParseColumnValue(item, "sno").ToString()
                        //MessageId = Convert.ToInt32(_DAO.ParseColumnValue(item, "SNO").ToString())

                    });
                }
            }
            return response;
        }

        public CommonDbResponse StoreApiResponseMessage(ApiResponseMessageCommon Request)
        {
            var Response = new CommonDbResponse();
            string SQL = "EXEC sproc_add_api_response_message ";
                SQL += "@code = " + _DAO.FilterString(Request.Code);
                SQL += ",@message = N" + _DAO.FilterString(Request.Message);
                SQL += ",@messageEng = " + _DAO.FilterString(Request.MessageEng);
                SQL += ",@category = " + _DAO.FilterString(Request.Category);
                SQL += ",@description = " + _DAO.FilterString(Request.Description);
                SQL += ",@messageType = " + _DAO.FilterString(Request.MessageType);
                SQL += ",@httpStatusCode = " + _DAO.FilterString(Request.HttpStatusCode);
                SQL += ",@actionUser = " + _DAO.FilterString(Request.ActionUser);

            Response = _DAO.ParseCommonDbResponse(SQL);
            return Response;
            
        }

        public CommonDbResponse UpdateApiResponseMessage(ApiResponseMessageCommon Request)
        {
            var Response = new CommonDbResponse();
            string SQL = "EXEC sproc_update_api_response_message ";
            SQL += "@messageId = " + _DAO.FilterString(Request.MessageId);
            SQL += ",@code = " + _DAO.FilterString(Request.Code);
            SQL += ",@message = N" + _DAO.FilterString(Request.Message);
            SQL += ",@messageEng = " + _DAO.FilterString(Request.MessageEng);
            SQL += ",@category = " + _DAO.FilterString(Request.Category);
            SQL += ",@description = " + _DAO.FilterString(Request.Description);
            SQL += ",@messageType = " + _DAO.FilterString(Request.MessageType);
            SQL += ",@httpStatusCode = " + _DAO.FilterString(Request.HttpStatusCode);
            SQL += ",@actionUser = " + _DAO.FilterString(Request.ActionUser);

            Response = _DAO.ParseCommonDbResponse(SQL);
            return Response;
        }
    }
}
