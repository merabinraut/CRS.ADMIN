using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PointSetup;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.REPOSITORY.PointSetup
{
    public class PointSetupRepository: IPointSetupRepository
    {
        RepositoryDao _DAO;
        public PointSetupRepository()
        {
            _DAO = new RepositoryDao();
        }
        public List<UserTypeCommon> GetUsertypeList(PaginationFilterCommon Request)
        {
            var response = new List<UserTypeCommon>();
            string SQL = "EXEC sproc_tbl_roles_select ";
            SQL += !string.IsNullOrEmpty(Request.SearchFilter) ? ",@SearchFilter=N" + _DAO.FilterString(Request.SearchFilter) : null;            
            SQL += " @Skip=" + Request.Skip;
            SQL += ",@Take=" + Request.Take;
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new UserTypeCommon()
                    {
                        SNO = Convert.ToInt32(_DAO.ParseColumnValue(item, "Sno")),
                        RoleTypeId = Convert.ToString(_DAO.ParseColumnValue(item, "RoleType")),
                        RoleTypeName = Convert.ToString(_DAO.ParseColumnValue(item, "RoleName"))

                    });
                }
            }
            return response;
        }

        public List<CategoryCommon> GetCategoryList(PaginationFilterCommon Request,string RoleTypeId="")
        {
            var response = new List<CategoryCommon>();
            string SQL = "EXEC sproc_point_category_select ";
            SQL += !string.IsNullOrEmpty(RoleTypeId) ? " @RoleTypeId=" + _DAO.FilterString(RoleTypeId) : null;
            SQL += " ,@Skip=" + Request.Skip;
            SQL += ",@Take=" + Request.Take;
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new CategoryCommon()
                    {
                        SNO = Convert.ToInt32(_DAO.ParseColumnValue(item, "Sno")),
                        RoleTypeId = Convert.ToString(_DAO.ParseColumnValue(item, "RoleType")),
                        CategoryId = Convert.ToString(_DAO.ParseColumnValue(item, "Id")),
                        CategoryName = Convert.ToString(_DAO.ParseColumnValue(item, "CategoryName")),
                        CategoryDescription = Convert.ToString(_DAO.ParseColumnValue(item, "Description")),
                        CreatedBy = Convert.ToString(_DAO.ParseColumnValue(item, "ActionUser")),
                        CreatedOn = Convert.ToString(_DAO.ParseColumnValue(item, "ActionDate")),
                        Status = Convert.ToString(_DAO.ParseColumnValue(item, "Status"))
                    });
                }
            }
            return response;
        }

        public CategoryCommon GetCategoryDetails(string roletypeId="", string categoryId="")
        {
            string SQL = "EXEC sproc_point_category_select ";
            SQL += " @RoleTypeId=" + _DAO.FilterString(roletypeId);
            SQL += ",@Id=" + _DAO.FilterString(categoryId);
            var dbResponse = _DAO.ExecuteDataRow(SQL);
            if (dbResponse != null)
            {
                return new CategoryCommon()
                {
                    SNO = Convert.ToInt32(_DAO.ParseColumnValue(dbResponse, "Sno")),
                    RoleTypeId = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "RoleType")),
                    CategoryId = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "Id")),
                    CategoryName = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "CategoryName")),
                    CategoryDescription = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "Description")),
                    CreatedBy = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "ActionUser")),
                    CreatedOn = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "ActionDate")),
                    Status = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "Status"))

                };
            }
            return new CategoryCommon();
        }
        public CommonDbResponse ManageCategory(CategoryCommon Request)
        {
            string SQL = "EXEC sproc_point_category_insertupdate ";
            SQL += " @CategoryName=N" + _DAO.FilterString(Request.CategoryName);
            SQL += ",@Description=N" + _DAO.FilterString(Request.CategoryDescription);
            SQL += ",@RoleType=" + _DAO.FilterString(Request.RoleTypeId);
            SQL += ",@Id=" + _DAO.FilterString(Request.CategoryId);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            return _DAO.ParseCommonDbResponse(SQL);
        }

        public CommonDbResponse BlockUnblockCategory(CategoryCommon Request)
        {
            string SQL = "EXEC sproc_point_category_blockunblock ";         
            SQL += ",@RoleType=" + _DAO.FilterString(Request.RoleTypeId);
            SQL += ",@Id=" + _DAO.FilterString(Request.CategoryId);
            SQL += ",@ActionUser=" + _DAO.FilterString(Request.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(Request.ActionIP);
            SQL += ",@Status=" + _DAO.FilterString(Request.ActionIP);
            return _DAO.ParseCommonDbResponse(SQL);
        }

    }
}
