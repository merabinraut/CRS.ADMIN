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
        List<UserTypeCommon> GetUsertypeList(PaginationFilterCommon Request);
        List<CategoryCommon> GetCategoryList(PaginationFilterCommon Request, string RoleTypeId = "");
        CategoryCommon GetCategoryDetails(string roletypeId = "", string categoryId = "");
        CommonDbResponse ManageCategory(CategoryCommon Request);
        CommonDbResponse BlockUnblockCategory(CategoryCommon Request);
    }
}
