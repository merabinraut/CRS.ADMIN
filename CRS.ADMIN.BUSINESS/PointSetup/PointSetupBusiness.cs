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

        public List<UserTypeCommon> GetUsertypeList(PaginationFilterCommon Request)
        {
            return _REPO.GetUsertypeList(Request);
        }
        public List<CategoryCommon> GetCategoryList(PaginationFilterCommon Request, string RoleTypeId="")
        {
            return _REPO.GetCategoryList(Request,RoleTypeId);
        }
        public CategoryCommon GetCategoryDetails(string roletypeId= "", string categoryId = "")
        {
            return _REPO.GetCategoryDetails(roletypeId, categoryId);
        }
        public CommonDbResponse ManageCategory(CategoryCommon Request)
        {
            return _REPO.ManageCategory(Request);
        } 
        public CommonDbResponse BlockUnblockCategory(CategoryCommon Request)
        {
            return _REPO.BlockUnblockCategory(Request);
        }
    }
}
