using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.PointsManagement;
using CRS.ADMIN.BUSINESS.AdminPointManagement;
using CRS.ADMIN.SHARED.PointsManagement;
using CRS.ADMIN.SHARED;
using System.Web.Mvc;
using CRS.ADMIN.BUSINESS.WithdrawSetup;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CRS.ADMIN.APPLICATION.Models.ClubManagement;
using CRS.ADMIN.APPLICATION.Models.Withdraw;
using CRS.ADMIN.APPLICATION.Models.PointSetup;
using CRS.ADMIN.SHARED.PointSetup;
using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class WithdrawSetupController : BaseController
    {
        private readonly IWithdrawSetupBusiness _withdrawBuss;
        public WithdrawSetupController(IWithdrawSetupBusiness withdrawBuss)
        {
            _withdrawBuss = withdrawBuss;
        }
        public ActionResult Index( int StartIndex = 0, int PageSize = 10)
        {
            var response = new WithdrawSetupModel();
            string RenderId = "";
            Session["CurrentURL"] = "/WithdrawSetup/Index";
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            var dbresponse = _withdrawBuss.GetWithdrawSetupList();
            response.WithdrawSetupList =dbresponse.MapObjects<WithdrawSetupList>();
            if (TempData.ContainsKey("ManageWithdrawSetupModel")) response.ManageWithdrawSetup = TempData["ManageWithdrawSetupModel"] as ManageWithdrawSetupModel;
            else response.ManageWithdrawSetup = new ManageWithdrawSetupModel();
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            var dictionaryempty = new Dictionary<string, string>();
            ViewBag.FromDate = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("FROMDATE", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            
            if (!string.IsNullOrEmpty(response.ManageWithdrawSetup.toDate))
            {
                ViewBag.ToDate = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("TODATE", response.ManageWithdrawSetup.fromDate.DecryptParameter(), "") as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            }
            else
            {
                ViewBag.ToDate = ApplicationUtilities.SetDDLValue(dictionaryempty, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            }
            if (!string.IsNullOrEmpty(response.ManageWithdrawSetup.withdrawDate))
            {
                ViewBag.WithdrawDate = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("WITHDRAWDAY", response.ManageWithdrawSetup.toDate.DecryptParameter(),"") as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            }
            else
            {
                ViewBag.WithdrawDate = ApplicationUtilities.SetDDLValue(dictionaryempty, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            }
            
           
            ViewBag.fromDateKey = response.ManageWithdrawSetup.fromDate;
            ViewBag.toDateKey = response.ManageWithdrawSetup.toDate;
            ViewBag.WithdrawDateKey = response.ManageWithdrawSetup.withdrawDate;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData =  0;
            return View(response);  
        }
        [HttpGet]
        public ActionResult ManageWithdrawSetup( )
        {
            var model = new ManageWithdrawSetupModel();
            Session["CurrentURL"] = "/WithdrawSetup/Index";
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            var dbresponse = _withdrawBuss.GetWithdrawSetupDetail();           
            model = dbresponse.MapObject<ManageWithdrawSetupModel>();
            var dictionaryempty = new Dictionary<string, string>();
            ViewBag.FromDate = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("FROMDATE", "", culture) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.ToDate = ApplicationUtilities.SetDDLValue(dictionaryempty, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.WithdrawDate = ApplicationUtilities.SetDDLValue(dictionaryempty, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            model.fromDate = !string.IsNullOrEmpty(model.fromDate) ? model.fromDate.EncryptParameter() : null;
            model.toDate = !string.IsNullOrEmpty(model.toDate) ? model.toDate.EncryptParameter() : null;
            model.withdrawDate = !string.IsNullOrEmpty(model.withdrawDate) ? model.withdrawDate.EncryptParameter() : null;
            model.InsertType = "UPDATE WITHDRAW SETUP";
            TempData["ManageWithdrawSetupModel"] = model;
            TempData["RenderId"] = "Manage";
            TempData["EditPlan"] = model;
            return RedirectToAction("Index", "WithdrawSetup");
        }

       
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManageWithdrawSetup(ManageWithdrawSetupModel Model)
        {
            string ErrorMessage = string.Empty;
           
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(Model.fromDate))
                {
                    ErrorMessage = "From Date is Required.";
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = ErrorMessage ?? "Something went wrong. Please try again later.",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    return RedirectToAction("Index", "WithdrawSetup");
                }
                if (string.IsNullOrEmpty(Model.toDate))
                {
                    ErrorMessage = "To Date is Required.";
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = ErrorMessage ?? "Something went wrong. Please try again later.",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    return RedirectToAction("Index", "WithdrawSetup");
                }
                CategoryCommon commonModel = Model.MapObject<CategoryCommon>();
                commonModel.RoleTypeId = Model.fromDate.DecryptParameter();
                commonModel.RoleTypeId = Model.toDate.DecryptParameter();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.ActionIP = ApplicationUtilities.GetIP();

                var ss = TempData["RenderId"] as string;
                string xmlData = GenerateXml(
                       ("minAmount", Model.minAmount),
                       ("maxAmount", Model.maxAmount),
                       ("fromDate", Model.fromDate.DecryptParameter()),
                       ("toDate", Model.toDate.DecryptParameter()),
                       ("withdrawDate",Model.withdrawDate.DecryptParameter())
                     );
                var dbResponse = _withdrawBuss.ManageWithdrawSetup(xmlData,Model.InsertType);
                if (dbResponse != null && dbResponse.Code == 0)
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
                    });
                    return RedirectToAction("Index", "WithdrawSetup");
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = NotificationMessage.INFORMATION.ToString()
                    });
                    TempData["ManageWithdrawSetupModel"] = Model;
                    TempData["RenderId"] = "Manage";
                    return RedirectToAction("Index", "WithdrawSetup");
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
            TempData["ManageWithdrawSetupModel"] = Model;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("Index", "WithdrawSetup");
        }
        public static string GenerateXml(params (string Key, string Value)[] configPairs)
        {
            var xmlString = "<Withdraw>";
            foreach (var pair in configPairs)
            {
                xmlString += $@"
              <Withdraw>
                  <ConfigKey>{pair.Key}</ConfigKey>
                  <ConfigValue>{pair.Value}</ConfigValue>
                  <Category>Withdraw Setup</Category>
              </Withdraw>";
            }

            xmlString += "\n</Withdraw>";
            return xmlString;
        }
        [HttpGet, OverrideActionFilters]
        public JsonResult GetToDate(string fromDate)
        {
            List<SelectListItem> ToDateList = new List<SelectListItem>();
            fromDate = !string.IsNullOrEmpty(fromDate) ? fromDate.DecryptParameter() : null;
            if (!string.IsNullOrEmpty(fromDate))
                ToDateList = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("TODATE", fromDate) as Dictionary<string, string>, "");
            var data = new
            {
                ToDateList = new SelectList(ToDateList, "Value", "Text"),
            };
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        [HttpGet, OverrideActionFilters]
        public JsonResult GetWithdrawDate(string toDate)
        {
            List<SelectListItem> WithdrawDateList = new List<SelectListItem>();
            toDate = !string.IsNullOrEmpty(toDate) ? toDate.DecryptParameter() : null;
            if (!string.IsNullOrEmpty(toDate))
                WithdrawDateList = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("WITHDRAWDAY", toDate) as Dictionary<string, string>, "");
            var data = new
            {
                WithdrawDateList = new SelectList(WithdrawDateList, "Value", "Text"),
            };
            return Json(data, JsonRequestBehavior.AllowGet);

        }

    }
}