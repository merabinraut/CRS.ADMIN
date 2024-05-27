using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PointSetup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.REPOSITORY.PointSetup
{
    public interface IPointSetupRepository
    {
        List<UserTypeCommon> GetUsertypeList(PaginationFilterCommon objPaginationFilterCommon);
        List<CategoryCommon> GetCategoryList(PaginationFilterCommon objPaginationFilterCommon, string roleTypeId = "");
        CategoryCommon GetCategoryDetails(string roletypeId = "", string categoryId = "");
        CommonDbResponse ManageCategory(CategoryCommon objCategoryCommon);
        CommonDbResponse BlockUnblockCategory(CategoryCommon objCategoryCommon);
        List<CategorySlabCommon> GetCategorySlabList(PaginationFilterCommon objPaginationFilterCommon, string roleTypeId = "", string categoryId = "");
        CategorySlabCommon GetCategorySlabDetails(string roletypeId = "", string categoryId = "", string categorySlabId = "");
        CommonDbResponse ManageCategorySlab(CategorySlabCommon objCategorySlabCommon);
        CommonDbResponse DeleteCategorySlab(CategorySlabCommon objCategorySlabCommon);
        CommonDbResponse AssignCategory(PointSetupCommon objPointSetupCommon);
    }
}

