using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.ClubManagement;
using CRS.ADMIN.APPLICATION.Models.PointSetup;
using CRS.ADMIN.APPLICATION.Models.PointsManagement;
using CRS.ADMIN.BUSINESS.PointSetup;
using CRS.ADMIN.BUSINESS.PointsManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PointsManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Google.Apis.Requests.BatchRequest;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class PointsManagementController : BaseController
    {
        private readonly IPointsManagementBusiness _BUSS;
        public PointsManagementController(IPointsManagementBusiness BUSS)
        {
            _BUSS = BUSS;
        }

        [HttpGet]
        public ActionResult PointsTransferList(string UserType = "",string UserName = "",string TransferTypeId = "",string FromDate = "",string ToDate = "", string SearchFilter = "", string value = "", int StartIndex = 0, int PageSize = 10, int StartIndex2 = 0, int PageSize2 = 10, int StartIndex3 = 0, int PageSize3 = 10, int StartIndex4 = 0, int PageSize4 = 10)
        {
            ViewBag.SearchFilter = SearchFilter;
            Session["CurrentURL"] = "/PointsManagement/PointsTransferList";
            string RenderId = "";
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;

            var objPointsManagementModel = new PointsManagementModel();
            PaginationFilterCommon dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(SearchFilter) ? SearchFilter : null
            };
            ViewBag.UserTypeList = ApplicationUtilities.SetDDLValue(DDLHelper.LoadDropdownList("USERTYPELIST") as Dictionary<string, string>, null, "--- Select ---");
            ViewBag.TransferTypeIdList = ApplicationUtilities.SetDDLValue(DDLHelper.LoadDropdownList("TRANSACTIONTYPE") as Dictionary<string, string>, null, "--- Select ---");
            var dictionaryempty = new Dictionary<string, string>();
            if (string.IsNullOrEmpty( UserType))
            {
                ViewBag.FilterUserList = ApplicationUtilities.SetDDLValue(dictionaryempty, null, "--- Select ---");
            }
            else
            {
                ViewBag.FilterUserList = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("USERTYPENAME", UserType.DecryptParameter()) as Dictionary<string, string>, null,"--- Select ---");
            }
            
            ViewBag.UserList = ApplicationUtilities.SetDDLValue(dictionaryempty, null, "--- Select ---");
            PointsManagementCommon Commonmodel = new PointsManagementCommon();
            Commonmodel.UserType=!string.IsNullOrEmpty( UserType)? UserType.DecryptParameter():null;
            Commonmodel.UserName = !string.IsNullOrEmpty(UserName) ? UserName.DecryptParameter(): null;
            Commonmodel.TransferTypeId = !string.IsNullOrEmpty(TransferTypeId) ? TransferTypeId.DecryptParameter() : null; 
            Commonmodel.FromDate = !string.IsNullOrEmpty(FromDate) ? FromDate : null;
            Commonmodel.ToDate = !string.IsNullOrEmpty(ToDate) ? ToDate : null;
            ViewBag.UserTypeIdKey= !string.IsNullOrEmpty( UserType)? UserType : null;
            ViewBag.UsernameIdKey = !string.IsNullOrEmpty(UserName) ? UserName : null;
            ViewBag.TransferTypeIdKey = !string.IsNullOrEmpty(TransferTypeId) ? TransferTypeId : null;

            var dbResponse = _BUSS.GetPointTransferList(Commonmodel,dbRequest);
            objPointsManagementModel.PointsTansferReportList = dbResponse.MapObjects<PointsTansferReportModel>();
            //var filteredItems = dbResponse
            //.Where(item => item.RoleTypeId == "3" || item.RoleTypeId == "4" || item.RoleTypeId == "6")
            //.ToList();
            //objPointSetupModel.UserTypeList = filteredItems.MapObjects<UserTypeModel>();

            //if (dbResponse.Count > 0)
            //{

            //    objPointSetupModel.UserTypeList.ForEach(x => x.RoleTypeId = !string.IsNullOrEmpty(x.RoleTypeId) ? x.RoleTypeId.EncryptParameter() : x.RoleTypeId);

            //}
            //var dictionary = new Dictionary<string, string>();
            //objPointSetupModel.UserTypeList.ForEach(item => { dictionary.Add(item.RoleTypeId, item.RoleTypeName); });
            //ViewBag.RoleTypeList = ApplicationUtilities.SetDDLValue(dictionary, null, "--- Select ---");
            //var dictionaryempty = new Dictionary<string, string>();
            //ViewBag.UserList = ApplicationUtilities.SetDDLValue(dictionaryempty, null, "--- Select ---");
            if (TempData.ContainsKey("ManagePointsModel")) objPointsManagementModel.ManagePointsTansfer = TempData["ManagePointsModel"] as PointsTansferModel;
            else objPointsManagementModel.ManagePointsTansfer = new PointsTansferModel();
            if (TempData.ContainsKey("ManagePointsRequestModel")) objPointsManagementModel.ManagePointsRequest = TempData["ManagePointsRequestModel"] as PointsRequestModel;
            else objPointsManagementModel.ManagePointsRequest = new PointsRequestModel();
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            //ViewBag.PointsCategoryList = ApplicationUtilities.SetDDLValue(dictionaryempty, null, "--- Select ---");
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            int defaultPageSize = 10;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.StartIndex2 = StartIndex2;
            ViewBag.PageSize2 = PageSize2;
            ViewBag.StartIndex3 = StartIndex3;
            ViewBag.PageSize3 = PageSize3;
            ViewBag.StartIndex4 = StartIndex4;
            ViewBag.PageSize4 = PageSize4;
            objPointsManagementModel.ListType = value;
            ViewBag.TotalData = dbResponse != null && dbResponse.Any() ? dbResponse[0].TotalRecords : 0;
            ViewBag.TotalData2 =  0;
            ViewBag.TotalData3 =0;
            ViewBag.TotalData4 =  0;
            return View(objPointsManagementModel);  
        }

        //[HttpGet]
        //public ActionResult ManagePoints( string Id = "")
        //{
        //    CategoryModel model = new CategoryModel();
        //    var culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
        //    TempData["ManagePointsModel"] = model;
        //    TempData["RenderId"] = "Manage";

        //    return RedirectToAction("PointsTransferList", "PointsManagement");

        //}
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManagePoints(PointsTansferModel objPointsTansferModel, HttpPostedFileBase Image_Certificate)
        {
           
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
            string ErrorMessage = string.Empty;
            string ImageCertificatePath = "";
            if (ModelState.IsValid)
            {
                if (Image_Certificate == null )
                {
                    bool allowRedirect = false;

                    if (Image_Certificate == null && string.IsNullOrEmpty(objPointsTansferModel.Image))
                    {
                        ErrorMessage = "Image required";

                        allowRedirect = true;
                    }
                    if (allowRedirect)
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = ErrorMessage ?? "Something went wrong. Please try again later.",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        TempData["ManagePointsModel"] = objPointsTansferModel;
                        TempData["RenderId"] = "Manage";
                        return RedirectToAction("PointsTransferList", "PointsManagement");
                    }
                }
                string dateTime = "";
                  bool allowRedirectfile = false;
                var allowedContentType = AllowedImageContentType();
                if (Image_Certificate != null)
                {
                    var contentType = Image_Certificate.ContentType;
                    var ext = Path.GetExtension(Image_Certificate.FileName);
                    if (allowedContentType.Contains(contentType.ToLower()))
                    {

                        dateTime = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                        string fileName = "ImageCertificate_" + dateTime + ext.ToLower();
                        ImageCertificatePath = Path.Combine(Server.MapPath("~/Content/UserUpload/PointsManagement"), fileName);
                        objPointsTansferModel.Image = "/Content/UserUpload/PointsManagement/" + fileName;
                    }
                    else
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid image format.",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        allowRedirectfile = true;

                    }
                }
                PointsTansferCommon commonModel = objPointsTansferModel.MapObject<PointsTansferCommon>();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Userid").ToString().DecryptParameter();
                commonModel.ActionIP = ApplicationUtilities.GetIP();
                commonModel.UserTypeId = objPointsTansferModel.UserTypeId.DecryptParameter();
                commonModel.UserId = objPointsTansferModel.UserId.DecryptParameter();
                var dbResponse = _BUSS.ManagePoints(commonModel);
                if (dbResponse != null && dbResponse.Code == 0)
                {
                    if (Image_Certificate != null) ApplicationUtilities.ResizeImage(Image_Certificate, ImageCertificatePath);
                   
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
                    });

                    return RedirectToAction("PointsTransferList", "PointsManagement");
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = NotificationMessage.INFORMATION.ToString()
                    });

                    TempData["ManagePointsModel"] = objPointsTansferModel;
                    TempData["RenderId"] = "Manage";
                    return RedirectToAction("PointsTransferList", "PointsManagement");
                }
            }
            TempData["ManagePointsModel"] = objPointsTansferModel;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("PointsTransferList", "PointsManagement");
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ManagePointsRequest(PointsRequestModel objPointsRequestModel)
        {

            var culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
            string ErrorMessage = string.Empty;
            string ImageCertificatePath = "";
            if (ModelState.IsValid)
            {                                             
                PointsRequestCommon commonModel = objPointsRequestModel.MapObject<PointsRequestCommon>();
                //commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Userid").ToString().DecryptParameter();
                commonModel.ActionIp = ApplicationUtilities.GetIP();
                var dbResponse = _BUSS.ManagePointsRequest(commonModel);
                if (dbResponse != null && dbResponse.Code == 0)
                {
                    
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
                    });

                    return RedirectToAction("PointsTransferList", "PointsManagement");
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = NotificationMessage.INFORMATION.ToString()
                    });

                    TempData["ManagePointsRequestModel"] = objPointsRequestModel;
                    TempData["RenderId"] = "ManagePoints";
                    return RedirectToAction("PointsTransferList", "PointsManagement");
                }
            }
            TempData["ManagePointsRequestModel"] = objPointsRequestModel;
            TempData["RenderId"] = "ManagePoints";
            return RedirectToAction("PointsTransferList", "PointsManagement");
        }
    }
}