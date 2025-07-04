using CRS.ADMIN.SHARED.PointSetup;
using CRS.ADMIN.SHARED;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRS.ADMIN.SHARED.TemplateManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System.Data;

namespace CRS.ADMIN.REPOSITORY.TemplateManagement
{
    public class TemplateRepository: ITemplateRepository
    {
        RepositoryDao _DAO;
        public TemplateRepository()
        {
            _DAO = new RepositoryDao();
        }

        public CommonDbResponse ManageTemplate(ManageTemplateCommon objManageTemplateCommon)
        {
            string SQL = "EXEC sproc_tbl_message_templates ";
            if (!string.IsNullOrEmpty(objManageTemplateCommon.Id))
            {
                SQL += "@flag = 'UPDATE' ,";
                SQL += "@updatedBy = N" + _DAO.FilterString(objManageTemplateCommon.ActionUser) + ", ";
                SQL += "@updatedIP = N" + _DAO.FilterString(objManageTemplateCommon.ActionIP) + ", ";
                SQL += "@updatedPlatform = N" + _DAO.FilterString(objManageTemplateCommon.ActionPlatform);
            }
            else
            {
                SQL += "@flag = 'INSERT' ,";
                SQL += "@createdBy = N" + _DAO.FilterString(objManageTemplateCommon.ActionUser) + ", ";
                SQL += "@createdIP = N" + _DAO.FilterString(objManageTemplateCommon.ActionIP) + ", ";
                SQL += "@createdPlatform = N" + _DAO.FilterString(objManageTemplateCommon.ActionPlatform);
            }
            SQL += ",@sno =" + _DAO.FilterString(objManageTemplateCommon.Id) + ", ";
            SQL += "@roleType =" + _DAO.FilterString(objManageTemplateCommon.userTypeDDL) + ", ";
            SQL += "@contentCategory = N" + _DAO.FilterString(objManageTemplateCommon.contentCategoryDDL) + ", ";
            SQL += "@contentType = N" + _DAO.FilterString(objManageTemplateCommon.contentTypeDDL) + ", ";
            SQL += "@subject = N" + _DAO.FilterString(objManageTemplateCommon.subject) + ", ";
            SQL += "@body = N" + _DAO.FilterString(objManageTemplateCommon.messageBody)  ;
       
            return _DAO.ParseCommonDbResponse(SQL);
        }

        public List<TemplateMessageCommon> GetTemplateList(PaginationFilterCommon objPaginationFilterCommon)
        {
            var response = new List<TemplateMessageCommon>();
            string SQL = "EXEC sproc_tbl_message_templates ";
            SQL += "@flag = 'LIST'";
            SQL += ", @skip = " + objPaginationFilterCommon.Skip;
            SQL += ", @Take = " + objPaginationFilterCommon.Take;
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {


                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new TemplateMessageCommon()
                    {
                        Sno = Convert.ToInt32(_DAO.ParseColumnValue(item, "sno")),
                        userType = Convert.ToString(_DAO.ParseColumnValue(item, "userType")),
                        contentCategory = Convert.ToString(_DAO.ParseColumnValue(item, "contentCategory")),
                        contentType = Convert.ToString(_DAO.ParseColumnValue(item, "contentType")),
                        subject = Convert.ToString(_DAO.ParseColumnValue(item, "subject")),
                        messageBody = Convert.ToString(_DAO.ParseColumnValue(item, "body")),
                        createdDate = Convert.ToString(_DAO.ParseColumnValue(item, "createdDate")),
                        State = Convert.ToBoolean(_DAO.ParseColumnValue(item, "state")),
                        Id = Convert.ToString(_DAO.ParseColumnValue(item, "Id")),
                        TotalRecords = Convert.ToInt32(_DAO.ParseColumnValue(item, "TotalRecords").ToString()),
                    });
                }
            }
            return response;
        }
        public ManageTemplateCommon GetTemplateDetails(string Id = "")
        {
            string SQL = "EXEC sproc_tbl_message_templates ";
            SQL += "@flag = 'EDIT'";
            SQL += ", @Sno = " + Id;
            var dbResponse = _DAO.ExecuteDataRow(SQL);
            if (dbResponse != null)
            {
                return new ManageTemplateCommon()
                {
                    Id = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "Sno")),
                    contentCategoryDDL = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "contentCategory")),
                    contentTypeDDL = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "contentType")),
                    userTypeDDL = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "userType")),
                    subject = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "subject")),
                    messageBody = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "body")),
                    

                };
            }
            return new ManageTemplateCommon();
        }

        public CommonDbResponse ManageToggleState(bool isOn, string id)
        {
            string SQL = "EXEC sproc_tbl_message_templates ";           
                SQL += "@flag = 'STATE' ,";
                SQL += "@isToggleState = N" + _DAO.FilterString(isOn.ToString()) + ",";
                SQL += "@sno = N" + _DAO.FilterString(id) + "";
            return _DAO.ParseCommonDbResponse(SQL);
        }
    }
}
