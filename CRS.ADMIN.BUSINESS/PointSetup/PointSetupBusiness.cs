using CRS.ADMIN.REPOSITORY.ClubManagement;
using CRS.ADMIN.REPOSITORY.PointSetup;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PointSetup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.BUSINESS.PointSetup
{
    public class PointSetupBusiness:IPointSetupBusiness
    {
        IPointSetupRepository _REPO;
        public PointSetupBusiness(PointSetupRepository REPO)
        {
            _REPO = REPO;
        }

        public List<UserTypeCommon> GetUsertypeList(PaginationFilterCommon objPaginationFilterCommon)
        {
            return _REPO.GetUsertypeList(objPaginationFilterCommon);
        }
        public List<CategoryCommon> GetCategoryList(PaginationFilterCommon objPaginationFilterCommon, string roleTypeId="")
        {
            return _REPO.GetCategoryList(objPaginationFilterCommon, roleTypeId);
        }
        public CategoryCommon GetCategoryDetails(string roletypeId= "", string categoryId = "")
        {
            return _REPO.GetCategoryDetails(roletypeId, categoryId);
        }
        public CommonDbResponse ManageCategory(CategoryCommon objCategoryCommon)
        {
            return _REPO.ManageCategory(objCategoryCommon);
        } 
        public CommonDbResponse BlockUnblockCategory(CategoryCommon objCategoryCommon)
        {
            return _REPO.BlockUnblockCategory(objCategoryCommon);
        }

        public List<CategorySlabCommon> GetCategorySlabList(PaginationFilterCommon objPaginationFilterCommon, string roleTypeId = "", string categoryId = "")
        {
            return _REPO.GetCategorySlabList(objPaginationFilterCommon,roleTypeId,categoryId);
        }
        public CategorySlabCommon GetCategorySlabDetails(string roletypeId = "", string categoryId = "", string categorySlabId = "")
        {
            return _REPO.GetCategorySlabDetails(roletypeId,categoryId,categorySlabId);
        }
        public CommonDbResponse ManageCategorySlab(CategorySlabCommon objCategorySlabCommon)
        {
            return _REPO.ManageCategorySlab(objCategorySlabCommon);
        }
        public CommonDbResponse DeleteCategorySlab(CategorySlabCommon objCategorySlabCommon)
        {
            return _REPO.DeleteCategorySlab(objCategorySlabCommon);
        }
    }
}

