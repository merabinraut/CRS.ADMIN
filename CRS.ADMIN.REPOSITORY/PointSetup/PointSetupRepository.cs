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
        public List<UserTypeCommon> GetUsertypeList(PaginationFilterCommon objPaginationFilterCommon)
        {
            var response = new List<UserTypeCommon>();
            string SQL = "EXEC sproc_tbl_static_data_select ";
            //SQL += !string.IsNullOrEmpty(objPaginationFilterCommon.SearchFilter) ? ",@SearchFilter=N" + _DAO.FilterString(objPaginationFilterCommon.SearchFilter) : null;            
            //SQL += " @Skip=" + objPaginationFilterCommon.Skip;
            //SQL += ",@Take=" + objPaginationFilterCommon.Take;
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {
                var filteredRows = dbResponse.Rows.Cast<DataRow>()
                        .Where(row => row["StaticDataType"] != DBNull.Value && Convert.ToInt32(row["StaticDataType"]) == 1);

                foreach (DataRow item in filteredRows)
                {
                    response.Add(new UserTypeCommon()
                    {
                        SNO = Convert.ToInt32(_DAO.ParseColumnValue(item, "Id")),
                        RoleTypeId = Convert.ToString(_DAO.ParseColumnValue(item, "StaticDataValue")),
                        RoleTypeName = Convert.ToString(_DAO.ParseColumnValue(item, "StaticDataLabel"))

                    });
                }
            }
            return response;
        }

        public List<CategoryCommon> GetCategoryList(PaginationFilterCommon objPaginationFilterCommon, string roleTypeId="")
        {
            var response = new List<CategoryCommon>();
            string SQL = "EXEC sproc_point_category_select ";
            SQL += !string.IsNullOrEmpty(roleTypeId) ? " @RoleTypeId=" + _DAO.FilterString(roleTypeId) : null;
            SQL += " ,@Skip=" + objPaginationFilterCommon.Skip;
            SQL += ",@Take=" + objPaginationFilterCommon.Take;
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new CategoryCommon()
                    {
                        SNO = Convert.ToInt32(_DAO.ParseColumnValue(item, "Sno")),
                        RoleTypeId = Convert.ToString(_DAO.ParseColumnValue(item, "RoleType")),
                        RoleName = Convert.ToString(_DAO.ParseColumnValue(item, "RoleName")),
                        CategoryId = Convert.ToString(_DAO.ParseColumnValue(item, "Id")),
                        CategoryName = Convert.ToString(_DAO.ParseColumnValue(item, "CategoryName")),
                        CategoryDescription = Convert.ToString(_DAO.ParseColumnValue(item, "Description")),
                        CreatedBy = Convert.ToString(_DAO.ParseColumnValue(item, "ActionUser")),
                        CreatedOn =!string.IsNullOrEmpty(_DAO.ParseColumnValue(item, "ActionDate").ToString())? DateTime.Parse(_DAO.ParseColumnValue(item, "ActionDate").ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : Convert.ToString(_DAO.ParseColumnValue(item, "ActionDate")),
                        Status = Convert.ToString(_DAO.ParseColumnValue(item, "Status")),
                        CategoryType = Convert.ToString(_DAO.ParseColumnValue(item, "CategoryType"))
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
                    CreatedOn = !string.IsNullOrEmpty(_DAO.ParseColumnValue(dbResponse, "ActionDate").ToString()) ? DateTime.Parse(_DAO.ParseColumnValue(dbResponse, "ActionDate").ToString()).ToString("yyyy'年'MM'月'dd'日' HH:mm:ss") : Convert.ToString(_DAO.ParseColumnValue(dbResponse, "ActionDate")),
                    Status = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "Status"))

                };
            }
            return new CategoryCommon();
        }
        public CommonDbResponse ManageCategory(CategoryCommon objCategoryCommon)
        {
            string SQL = "EXEC sproc_point_category_insertupdate ";
            SQL += " @CategoryName=N" + _DAO.FilterString(objCategoryCommon.CategoryName);
            SQL += ",@Description=N" + _DAO.FilterString(objCategoryCommon.CategoryDescription);
            SQL += ",@RoleType=" + _DAO.FilterString(objCategoryCommon.RoleTypeId);
            SQL += ",@Id=" + _DAO.FilterString(objCategoryCommon.CategoryId);
            SQL += ",@ActionUser=" + _DAO.FilterString(objCategoryCommon.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(objCategoryCommon.ActionIP);
            return _DAO.ParseCommonDbResponse(SQL);
        }

        public CommonDbResponse BlockUnblockCategory(CategoryCommon objCategoryCommon)
        {
            string SQL = "EXEC sproc_point_category_blockunblock ";         
            SQL += " @RoleType=" + _DAO.FilterString(objCategoryCommon.RoleTypeId);
            SQL += ",@Id=" + _DAO.FilterString(objCategoryCommon.CategoryId);
            SQL += ",@ActionUser=" + _DAO.FilterString(objCategoryCommon.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(objCategoryCommon.ActionIP);
            SQL += ",@Status=" + _DAO.FilterString(objCategoryCommon.Status);
            return _DAO.ParseCommonDbResponse(SQL);
        }

        #region Category Slab
        public List<CategorySlabCommon> GetCategorySlabList(PaginationFilterCommon objPaginationFilterCommon, string roleTypeId = "", string categoryId = "")
        {
            var response = new List<CategorySlabCommon>();
            string SQL = "EXEC sproc_point_category_details_select ";
            SQL += !string.IsNullOrEmpty(roleTypeId) ? " @RoleTypeId=" + _DAO.FilterString(roleTypeId) : null;
            SQL += !string.IsNullOrEmpty(categoryId) ? " ,@CategoryId=" + _DAO.FilterString(categoryId) : null;
            SQL += " ,@Skip=" + objPaginationFilterCommon.Skip;
            SQL += ",@Take=" + objPaginationFilterCommon.Take;
            var dbResponse = _DAO.ExecuteDataTable(SQL);
            if (dbResponse != null)
            {
                foreach (DataRow item in dbResponse.Rows)
                {
                    response.Add(new CategorySlabCommon()
                    {
                        SNO = Convert.ToInt32(_DAO.ParseColumnValue(item, "Sno")),
                        RoleTypeId = Convert.ToString(_DAO.ParseColumnValue(item, "RoleType")),
                        CategoryId = Convert.ToString(_DAO.ParseColumnValue(item, "CategoryId")),
                        CategoryName = Convert.ToString(_DAO.ParseColumnValue(item, "CategoryName")),
                        FromAmount = Convert.ToInt64(_DAO.ParseColumnValue(item, "FromAmount")).ToString("N0"),
                        PointType = Convert.ToString(_DAO.ParseColumnValue(item, "PointType")),
                        ToAmount = Convert.ToInt64(_DAO.ParseColumnValue(item, "ToAmount")).ToString("N0"),
                        PointValue = Convert.ToInt64(_DAO.ParseColumnValue(item, "PointValue")).ToString("N0"),
                        CategorySlabId = Convert.ToString(_DAO.ParseColumnValue(item, "Id")),
                        //CreatedOn = Convert.ToString(_DAO.ParseColumnValue(item, "ActionDate")),
                        //Status = Convert.ToString(_DAO.ParseColumnValue(item, "Status"))
                    });
                }
            }
            return response;
        }
        public CategorySlabCommon GetCategorySlabDetails(string roletypeId = "", string categoryId = "", string categorySlabId = "")
        {
            string SQL = "EXEC sproc_point_category_details_select ";
            SQL += " @RoleTypeId=" + _DAO.FilterString(roletypeId);
            SQL += " ,@Id=" + _DAO.FilterString(categorySlabId);
            SQL += ",@CategoryId=" + _DAO.FilterString(categoryId);
            var dbResponse = _DAO.ExecuteDataRow(SQL);
            if (dbResponse != null)
            {
                return new CategorySlabCommon()
                {
                    CategorySlabId = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "Id")),
                    RoleTypeId = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "RoleType")),
                    CategoryId = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "CategoryId")),
                    CategoryName = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "CategoryName")),
                    FromAmount = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "FromAmount")),
                    PointType = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "PointType")),
                    ToAmount = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "ToAmount")),
                    PointValue = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "PointValue")),
                    PointValue2 = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "PointValue2")),
                    PointValue3 = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "PointValue3")),
                    PointType2 = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "PointType2")),
                    PointType3 = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "PointType3")),
                    MinValue = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "MinValue")),
                    MinValue2 = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "MinValue2")),
                    MinValue3 = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "MinValue3")),
                    MaxValue = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "MaxValue")),
                    MaxValue2 = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "MaxValue2")),
                    MaxValue3 = Convert.ToString(_DAO.ParseColumnValue(dbResponse, "MaxValue3")),

                };
            }
            return new CategorySlabCommon();
        }
        public CommonDbResponse ManageCategorySlab(CategorySlabCommon objCategorySlabCommon)
        {
            string SQL = "EXEC sproc_point_category_detail_insertupdate ";
            SQL += " @Id=" + _DAO.FilterString(objCategorySlabCommon.CategorySlabId);
            SQL += ",@FromAmount=" + _DAO.FilterString(objCategorySlabCommon.FromAmount);
            SQL += ",@ToAmount=" + _DAO.FilterString(objCategorySlabCommon.ToAmount);
            SQL += ",@PointType=" + _DAO.FilterString(objCategorySlabCommon.PointType);
            SQL += ",@PointValue=" + _DAO.FilterString(objCategorySlabCommon.PointValue);
            SQL += ",@RoleType=" + _DAO.FilterString(objCategorySlabCommon.RoleTypeId);
            SQL += ",@CategoryId=" + _DAO.FilterString(objCategorySlabCommon.CategoryId);
            SQL += ",@ActionUser=" + _DAO.FilterString(objCategorySlabCommon.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(objCategorySlabCommon.ActionIP);
            SQL += ",@PointType2=" + _DAO.FilterString(objCategorySlabCommon.PointType2);
            SQL += ",@PointValue2=" + _DAO.FilterString(objCategorySlabCommon.PointValue2);
            SQL += ",@PointType3=" + _DAO.FilterString(objCategorySlabCommon.PointType3);
            SQL += ",@PointValue3=" + _DAO.FilterString(objCategorySlabCommon.PointValue3);
            SQL += ",@MinValue=" + _DAO.FilterString(objCategorySlabCommon.MinValue);
            SQL += ",@MaxValue=" + _DAO.FilterString(objCategorySlabCommon.MaxValue);
            SQL += ",@MinValue2=" + _DAO.FilterString(objCategorySlabCommon.MinValue2);
            SQL += ",@MaxValue2=" + _DAO.FilterString(objCategorySlabCommon.MaxValue2);
            SQL += ",@MinValue3=" + _DAO.FilterString(objCategorySlabCommon.MinValue3);
            SQL += ",@MaxValue3=" + _DAO.FilterString(objCategorySlabCommon.MaxValue3);
            //SQL += ",@CategoryId=" + _DAO.FilterString(objCategorySlabCommon.CategoryId);
            //SQL += ",@Min=" + _DAO.FilterString(objCategorySlabCommon.MinValue);
            //SQL += ",@Max=" + _DAO.FilterString(objCategorySlabCommon.MaxValue);
            return _DAO.ParseCommonDbResponse(SQL);
        } 
        public CommonDbResponse DeleteCategorySlab(CategorySlabCommon objCategorySlabCommon)
        {
            string SQL = "EXEC sproc_point_category_detail_delete ";
            SQL += " @Id=" + _DAO.FilterString(objCategorySlabCommon.CategorySlabId);           
            SQL += ",@RoleType=" + _DAO.FilterString(objCategorySlabCommon.RoleTypeId);
            SQL += ",@CategoryId=" + _DAO.FilterString(objCategorySlabCommon.CategoryId);
            SQL += ",@Status=" + _DAO.FilterString(objCategorySlabCommon.Status);
            SQL += ",@ActionUser=" + _DAO.FilterString(objCategorySlabCommon.ActionUser);
            SQL += ",@ActionIP=" + _DAO.FilterString(objCategorySlabCommon.ActionIP);
            return _DAO.ParseCommonDbResponse(SQL);
        }
        #endregion

        #region Assign Category
        public CommonDbResponse AssignCategory(PointSetupCommon objPointSetupCommon)
        {
            string SQL = "EXEC sproc_point_category_assign ";
            SQL += " @RoleTypeId=" + _DAO.FilterString(objPointSetupCommon.UserTypeId);
            SQL += ",@CategoryId=" + _DAO.FilterString(objPointSetupCommon.NewCategoryId);
            SQL += ",@AgentId=" + _DAO.FilterString(objPointSetupCommon.AgentId);
            return _DAO.ParseCommonDbResponse(SQL);
        }
        #endregion
    }
}
