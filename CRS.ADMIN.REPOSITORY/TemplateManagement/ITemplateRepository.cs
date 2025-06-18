using CRS.ADMIN.SHARED.TemplateManagement;
using CRS.ADMIN.SHARED;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRS.ADMIN.SHARED.PaginationManagement;

namespace CRS.ADMIN.REPOSITORY.TemplateManagement
{
    public interface ITemplateRepository
    {
        CommonDbResponse ManageTemplate(ManageTemplateCommon objManageTemplateCommon);
        List<TemplateMessageCommon> GetTemplateList(PaginationFilterCommon objPaginationFilterCommon);
        ManageTemplateCommon GetTemplateDetails(string Id = "");
    }
}
