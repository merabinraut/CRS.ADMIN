
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.PointsManagement;
using CRS.ADMIN.BUSINESS.AdminPointManagement;
using CRS.ADMIN.SHARED.PointsManagement;
using CRS.ADMIN.SHARED;
using System.Web.Mvc;
using CRS.ADMIN.APPLICATION.Models.AdminPointManagement;
using CRS.ADMIN.SHARED.AdminPointManagement;
using CRS.ADMIN.SHARED.PaginationManagement;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class AdminPointManagementController : BaseController
    {
        private readonly IAdminPointManagementBusiness _adminpointBuss;
        public AdminPointManagementController(IAdminPointManagementBusiness adminpointBuss)
        {
            _adminpointBuss = adminpointBuss;
        }
        public ActionResult AdminPointList(string SearchFilter = "", int StartIndex = 0, int PageSize = 10,string fromDate=null,string toDate=null )
        {
            ViewBag.SearchFilter = null;
            Session["CurrentURL"] = "/AdminPointManagement/AdminPointList";
            string RenderId = "";
            var objAdminPointManagementModel = new AdminPointManagementModel();
            if (TempData.ContainsKey("ManagePointRequestModel")) objAdminPointManagementModel.ManagePointRequest = TempData["ManagePointRequestModel"] as ManagePointRequestModel;
            else objAdminPointManagementModel.ManagePointRequest = new ManagePointRequestModel();
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            PaginationFilterCommon dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null
            };
            PointRequestDetailCommon Commonmodel = new PointRequestDetailCommon();
            Commonmodel.fromDate = !string.IsNullOrEmpty(fromDate) ? fromDate : null;
            Commonmodel.toDate = !string.IsNullOrEmpty(toDate) ? toDate : null;
            Commonmodel.txnId = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter: null;
            var dbResponse = _adminpointBuss.GetManualEntryPointList(Commonmodel, dbRequest);
            objAdminPointManagementModel.PointRequestDetailList = dbResponse.MapObjects<PointRequestDetail>();
            objAdminPointManagementModel.fromDate = fromDate;
            objAdminPointManagementModel.toDate = toDate;
            objAdminPointManagementModel.searchFilter = SearchFilter;
            return View(objAdminPointManagementModel);  
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManagePointRequest(ManagePointRequestModel objManagePointRequestModel)
        {

            var culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
            string ErrorMessage = string.Empty;
            if (ModelState.IsValid)
            {
                ManagePointRequestCommon commonModel = objManagePointRequestModel.MapObject<ManagePointRequestCommon>();
                commonModel.actionUser = ApplicationUtilities.GetSessionValue("Userid").ToString().DecryptParameter();
                commonModel.actionIp = ApplicationUtilities.GetIP();
                var dbResponse = _adminpointBuss.ManageAdminPointsRequest(commonModel);
                if (dbResponse != null && dbResponse.Code == 0)
                {

                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
                    });
                    return RedirectToAction("AdminPointList", "AdminPointManagement");
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = NotificationMessage.INFORMATION.ToString()
                    });

                    TempData["ManagePointRequestModel"] = objManagePointRequestModel;
                    TempData["RenderId"] = "ManageAdminPoint";
                    return RedirectToAction("AdminPointList", "AdminPointManagement");
                }
            }
            TempData["ManagePointRequestModel"] = objManagePointRequestModel;
            TempData["RenderId"] = "ManageAdminPoint";
            return RedirectToAction("AdminPointList", "AdminPointManagement");
        }
    }
}