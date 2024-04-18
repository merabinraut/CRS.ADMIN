using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PointSetup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRS.ADMIN.BUSINESS.PointSetup
{
    public interface IPointSetupBusiness
    {
        List<UserTypeCommon> GetUsertypeList(PaginationFilterCommon Request);
        List<CategoryCommon> GetCategoryList(PaginationFilterCommon Request, string RoleTypeId="");
        CategoryCommon GetCategoryDetails(string roletypeId = "", string categoryId = "");
        CommonDbResponse ManageCategory(CategoryCommon Request);
        CommonDbResponse BlockUnblockCategory(CategoryCommon Request);
    }
}
