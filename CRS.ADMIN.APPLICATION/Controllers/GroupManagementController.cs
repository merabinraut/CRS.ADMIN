using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.GroupManagement;
using CRS.ADMIN.BUSINESS.GroupManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class GroupManagementController : BaseController
    {
        private readonly IGroupManagementBusiness _business;
        public GroupManagementController(IGroupManagementBusiness business)
        {
            _business = business;
        }
        [HttpGet]
        public ActionResult Index(string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            Session["CurrentURL"] = "GroupManagement/Index";
            PaginationFilterCommon paginationFilter = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null
            };
            GroupOverviewModel commonResponse = new GroupOverviewModel();
            var dbGroupResponse = _business.GetGroupList(paginationFilter);
            var dbGroupAnalyticResponse = _business.GetGroupAnalytic();
            commonResponse.GetGroupList = dbGroupResponse.MapObjects<GroupInfoModel>();
            commonResponse.GetGroupAnalytic = dbGroupAnalyticResponse.MapObject<GroupAnalyticModel>();
            return View(commonResponse);
        }
    }         
}