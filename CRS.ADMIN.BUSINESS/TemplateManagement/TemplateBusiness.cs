using CRS.ADMIN.REPOSITORY.PointSetup;
using CRS.ADMIN.REPOSITORY.TemplateManagement;
using CRS.ADMIN.SHARED.PointSetup;
using CRS.ADMIN.SHARED;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRS.ADMIN.SHARED.TemplateManagement;
using CRS.ADMIN.SHARED.PaginationManagement;

namespace CRS.ADMIN.BUSINESS.TemplateManagement
{
   public class TemplateBusiness: ITemplateBusiness
    {
        ITemplateRepository _REPO;
        public TemplateBusiness(TemplateRepository REPO)
        {
            _REPO = REPO;
        }

        public CommonDbResponse ManageTemplate(ManageTemplateCommon objManageTemplateCommon)
        {
            return _REPO.ManageTemplate(objManageTemplateCommon);
        } 
        public List<TemplateMessageCommon> GetTemplateList(PaginationFilterCommon objPaginationFilterCommon)
        {
            return _REPO.GetTemplateList(objPaginationFilterCommon);
        }
        public ManageTemplateCommon GetTemplateDetails(string Id = "")
        {
            return _REPO.GetTemplateDetails(Id);
        }
    }
}
