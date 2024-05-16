using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models;
using CRS.ADMIN.APPLICATION.Models.UserProfileManagement;
using CRS.ADMIN.BUSINESS.ProfileManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ProfileManagement;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class ProfileManagementController : BaseController
    {
        private readonly IProfileManagementBusiness _business;

        public ProfileManagementController(IProfileManagementBusiness business) => this._business = business;
        public ActionResult Index()
        {
            Session["CurrentUrl"] = "/ProfileManagement/Index";
            var common = new UserProfileCommon()
            {
                UserId = ApplicationUtilities.GetSessionValue("UserId").ToString().DecryptParameter(),
                ActionUser = ApplicationUtilities.GetSessionValue("UserName").ToString(),
                RoleName = ApplicationUtilities.GetSessionValue("RoleId").ToString(),
                Session = Session.SessionID,
            };
            var data = _business.ShowUserProfile(common);
            var viewModel = data.MapObject<UserProfileModel>();
            viewModel.ProfileImage = ImageHelper.ProcessedImage(viewModel.ProfileImage);
            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult UpdateAdminDetail(UserProfileModel UPM)
        {
            if (ModelState.IsValid)
            {
                var common = UPM.MapObject<UserProfileCommon>();
                common.UserId = ApplicationUtilities.GetSessionValue("userid").ToString().DecryptParameter();
                common.IpAddress = ApplicationUtilities.GetIP();
                common.ActionUser = ApplicationUtilities.GetSessionValue("UserName").ToString();
                CommonDbResponse dbresp = _business.UpdateAdminUserDetails(common);
                if (dbresp.Code == ResponseCode.Success)
                {
                    Session["FullName"] = common.FullName;
                    AddNotificationMessage(new NotificationModel()
                    {
                        Message = dbresp.Message,
                        NotificationType = NotificationMessage.SUCCESS,
                        Title = NotificationMessage.SUCCESS.ToString()
                    });
                    return Json(new { dbresp.Message });
                }
            }
            var errorMessages = ModelState.Where(x => x.Value.Errors.Count > 0)
                                   .SelectMany(x => x.Value.Errors.Select(e => $"{e.ErrorMessage}"))
                                   .ToList();
            var notificationModels = errorMessages.Select(errorMessage => new NotificationModel
            {
                NotificationType = NotificationMessage.ERROR,
                Message = errorMessage,
                Title = NotificationMessage.ERROR.ToString(),
            }).ToArray();
            AddNotificationMessage(notificationModels);
            return Json(new CommonDbResponse { ErrorCode = 1, Message = "Unable to update." });

        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            Session["CurrentUrl"] = "/ProfileManagement/ChangePassword";
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel changePassword)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Where(x => x.Value.Errors.Count > 0)
                                   .SelectMany(x => x.Value.Errors.Select(e => $"{e.ErrorMessage}"))
                                   .ToList();
                var notificationModels = errorMessages.Select(errorMessage => new NotificationModel
                {
                    NotificationType = NotificationMessage.ERROR,
                    Message = errorMessage,
                    Title = NotificationMessage.ERROR.ToString(),
                }).ToArray();
                AddNotificationMessage(notificationModels);

                return View(changePassword);
            }

            var passwordCommon = changePassword.MapObject<PasswordCommon>();
            passwordCommon.UserId = ApplicationUtilities.GetSessionValue("UserId")?.ToString()?.DecryptParameter();
            passwordCommon.IPAddress = ApplicationUtilities.GetIP().ToString();
            passwordCommon.BrowserInfo = ApplicationUtilities.GetBrowserInfo().ToString();
            passwordCommon.ActionUser = ApplicationUtilities.GetSessionValue("UserName").ToString();

            var dbResp = _business.ChangePassword(passwordCommon);

            if (dbResp.Code != ResponseCode.Success)
            {
                AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = dbResp.Message,
                    Title = NotificationMessage.INFORMATION.ToString(),
                });

                return View();
            }

            AddNotificationMessage(new NotificationModel()
            {
                NotificationType = NotificationMessage.SUCCESS,
                Message = dbResp.Message,
                Title = NotificationMessage.SUCCESS.ToString(),
            });
            return RedirectToAction("LogOff", "Home");
        }

        [HttpPost]
        public async Task<JsonResult> ChangeProfileImage(HttpPostedFileBase file)
        {
            var common = new UserProfileCommon();
            string FileLocationPath = "/Content/userupload/admin/";
            ViewBag.FileLocation = "";

            for (int i = 0; i < Request.Files.Count; i++)
            {
                file = Request.Files[i];
            }
            if (file != null)
            {
                var contentType = file.ContentType;
                var allowedContentType = new[] { "image/png", "image/jpeg", "image/HEIF", "image/heif" };
                var ext = Path.GetExtension(file.FileName);
                string fileName = string.Empty;
                if (allowedContentType.Contains(contentType.ToLower()))
                {
                    fileName = $"{AWSBucketFolderNameModel.ADMIN}/AdminProfileImage_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{ext.ToLower()}";
                    common.AdminLogoPath = $"/{fileName}";
                }
                else
                {
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.ERROR,
                        Message = "Image must be in jpeg, jpg or png format",
                        Title = NotificationMessage.ERROR.ToString(),
                    });

                    return Json(new { Code = "0", Message = "File Must be .jpg,.png,.jpeg" });
                }
                common.UserId = ApplicationUtilities.GetSessionValue("UserId").ToString().DecryptParameter();
                common.ActionUser = ApplicationUtilities.GetSessionValue("UserName").ToString();
                var dbresp = _business.ChangeProfileImage(common);
                if (dbresp.Code == 0)
                {
                    await ImageHelper.ImageUpload(fileName, file);
                    Session["ProfileImage"] = ImageHelper.ProcessedImage(common.AdminLogoPath);
                    AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.SUCCESS,
                        Message = "Image uploaded successfully",
                        Title = NotificationMessage.SUCCESS.ToString(),
                    });

                    return Json(new { Code = "0", Message = "Uploaded successfully" });
                }
            }
            return Json(new { Code = "1", Message = "Something went wrong please try again" });
        }

        [OverrideActionFilters]
        public ActionResult SuccessPage(string Message = "")
        {
            Message = (Message == "") ? "This is Success" : Message;
            ViewBag.SuccessMessage = Message;
            AbandonSession();
            return View();
        }

        public void AbandonSession()
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }
        }

    }
}