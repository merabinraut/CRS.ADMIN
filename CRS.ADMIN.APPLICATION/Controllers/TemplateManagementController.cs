using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.SHARED;
using System.Linq;
using System.Web.Mvc;
using CRS.ADMIN.APPLICATION.Models.TemplateMessage;
using CRS.ADMIN.APPLICATION.Helper;
using System.Collections.Generic;
using CRS.ADMIN.SHARED.TemplateManagement;
using CRS.ADMIN.BUSINESS.TemplateManagement;
using CRS.ADMIN.SHARED.PaginationManagement;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class TemplateManagementController : BaseController
    {
        private readonly ITemplateBusiness _BUSS;
        public TemplateManagementController(ITemplateBusiness BUSS)
        {
            _BUSS = BUSS;
        }

        [HttpGet]
        public ActionResult MessageTemplateList(string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            ViewBag.SearchFilter = null;
            Session["CurrentURL"] = "/TemplateManagement/MessageTemplateList";
            string RenderId = "";
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            TemplateMessageModel objTemplateMessageModel = new TemplateMessageModel();
            if (TempData.ContainsKey("ManageTemplateModel")) objTemplateMessageModel.ManageTemplateModel = TempData["ManageTemplateModel"] as ManageTemplateModel;
            else objTemplateMessageModel.ManageTemplateModel = new ManageTemplateModel();
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            PaginationFilterCommon dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null
            };
            var dbResponse = _BUSS.GetTemplateList(dbRequest);

            objTemplateMessageModel.GetTemplateMessageList = dbResponse.MapObjects<TemplateMessageListModel>();
            if (dbResponse.Count > 0)
            {
                objTemplateMessageModel.GetTemplateMessageList.ForEach(x => x.Id = !string.IsNullOrEmpty(x.Id) ? x.Id.EncryptParameter() : x.Id);
            }
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            ViewBag.contentCategoryDDL = ApplicationUtilities.SetDDLValue(DDLHelper.LoadDropdownList("CONTENTCATEGORY", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.contentTypeDDL = ApplicationUtilities.SetDDLValue(DDLHelper.LoadDropdownList("CONTENTTYPE", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.fieldTypeDDL = ApplicationUtilities.SetDDLValue(DDLHelper.LoadDropdownList("FIELDTYPE", "", culture) as Dictionary<string, string>, null, "");
            ViewBag.userTypeDDL = ApplicationUtilities.SetDDLValue(DDLHelper.LoadDropdownList("USERTYPEALL", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.userTypeKey = objTemplateMessageModel.ManageTemplateModel.userTypeDDL;
            ViewBag.contentCategoryKey = objTemplateMessageModel.ManageTemplateModel.contentCategoryDDL;
            ViewBag.contentTypeKey = objTemplateMessageModel.ManageTemplateModel.contentTypeDDL;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.SearchFilter = SearchFilter;
            ViewBag.TotalData = dbResponse != null && dbResponse.Any() ? dbResponse[0].TotalRecords : 0;
            return View(objTemplateMessageModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ManageTemplate(ManageTemplateModel Model)
        {
            string ErrorMessage = string.Empty;

            if (ModelState.IsValid)
            {
                ManageTemplateCommon commonModel = Model.MapObject<ManageTemplateCommon>();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.ActionIP = ApplicationUtilities.GetIP();
                commonModel.userTypeDDL = Model.userTypeDDL.DecryptParameter();
                commonModel.contentTypeDDL = Model.contentTypeDDL.DecryptParameter();
                commonModel.contentCategoryDDL = Model.contentCategoryDDL.DecryptParameter();
                commonModel.Id = !string.IsNullOrEmpty(Model.Id) ? Model.Id.DecryptParameter() : null;
                var dbResponse = _BUSS.ManageTemplate(commonModel);
                if (dbResponse != null && dbResponse.Code == 0)
                {

                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
                    });
                    return RedirectToAction("MessageTemplateList", "TemplateManagement");
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = NotificationMessage.INFORMATION.ToString()
                    });
                    TempData["ManageTemplateModel"] = Model;
                    TempData["RenderId"] = "Manage";
                    return RedirectToAction("MessageTemplateList", "TemplateManagement");
                }
            }
            var errorMessages = ModelState.Where(x => x.Value.Errors.Count > 0)
                                  .SelectMany(x => x.Value.Errors.Select(e => $"{x.Key}: {e.ErrorMessage}"))
                                  .ToList();

            var notificationModels = errorMessages.Select(errorMessage => new NotificationModel
            {
                NotificationType = NotificationMessage.INFORMATION,
                Message = errorMessage,
                Title = NotificationMessage.INFORMATION.ToString(),
            }).ToArray();
            AddNotificationMessage(notificationModels);
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key }).ToList();
            TempData["ManageTemplateModel"] = Model;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("MessageTemplateList", "TemplateManagement");
        }


        [HttpGet]
        public ActionResult ManageTemplate(string Id = "", string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            ManageTemplateModel model = new ManageTemplateModel();
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
            Id = !string.IsNullOrEmpty(Id) ? Id.DecryptParameter() : null;
            if (string.IsNullOrEmpty(Id))
            {

                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("MessageTemplateList", "TemplateManagement", new
                {
                    SearchFilter = SearchFilter,
                    StartIndex = StartIndex,
                    PageSize = PageSize
                });
            }
            var dbResponse = _BUSS.GetTemplateDetails(Id);
            model = dbResponse.MapObject<ManageTemplateModel>();
            model.Id = Id.EncryptParameter();
            model.contentCategoryDDL = model.contentCategoryDDL.EncryptParameter();
            model.contentTypeDDL = model.contentTypeDDL.EncryptParameter();
            model.userTypeDDL = model.userTypeDDL.EncryptParameter();
            TempData["ManageTemplateModel"] = model;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("MessageTemplateList", "TemplateManagement", new
            {
                SearchFilter = SearchFilter,
                StartIndex = StartIndex,
                PageSize = PageSize
            });
        }

    }
}




