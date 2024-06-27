using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models;
using CRS.ADMIN.APPLICATION.Models.PromotionManagement;
using CRS.ADMIN.BUSINESS.PromotionManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED.PromotionManagement;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class PromotionManagementController : BaseController
    {
        private readonly IPromotionManagementBusiness _business;
        public PromotionManagementController(IPromotionManagementBusiness business) => this._business = business;
        public ActionResult GetPromotionalImages(PromotionManagementCommonModel Request,  int StartIndex = 0, int PageSize = 10,int StartIndex2 = 0, int PageSize2 = 10)
        {
            Session["CurrentUrl"] = "/PromotionManagement/GetPromotionalImages";
            PromotionManagementCommonModel ResponseModel = new PromotionManagementCommonModel();
            PaginationFilterCommon dbRequest = new PaginationFilterCommon()
            {
                Skip = StartIndex,
                Take = PageSize,
                SearchFilter = !string.IsNullOrEmpty(Request.SearchFilter) ? Request.SearchFilter : null
            };            
            var promotionalImages = _business.GetPromotionalImageLists(dbRequest);         
            ResponseModel.PromotionManagementListModel = promotionalImages.MapObjects<PromotionManagementListModel>();            
            foreach (var item in ResponseModel.PromotionManagementListModel)
            {
                item.Id = item.Id?.EncryptParameter();
                item.IsDeleted = item.IsDeleted.Trim().ToUpper() == "FALSE" ? "A" : "B";
                item.ImagePath = ImageHelper.ProcessedImage(item.ImagePath);
            }
            if (TempData.ContainsKey("AdvertisementManagementModel")) ResponseModel = TempData["AdvertisementManagementModel"] as PromotionManagementCommonModel;

            PaginationFilterCommon dbRequestadvertise = new PaginationFilterCommon()
            {
                Skip = StartIndex2,
                Take = PageSize2,
                SearchFilter = !string.IsNullOrEmpty(Request.SearchFilter2) ? Request.SearchFilter2 : null
            };
            var advertisementImages = _business.GetAdvertisementImageLists(dbRequestadvertise);
            ResponseModel.AdvertisementManagementListModel = advertisementImages.MapObjects<AdvertisementManagementListModel>();
            foreach (var item in ResponseModel.AdvertisementManagementListModel)
            {
                item.Id = item.Id?.EncryptParameter();
                item.IsDeleted = item.IsDeleted.Trim().ToUpper() == "FALSE" ? "A" : "B";
                item.ImagePath = ImageHelper.ProcessedImage(item.ImagePath);
            }
            string RenderId = "";
            if (TempData.ContainsKey("PromotionManagementModel")) ResponseModel.PromotionManagementModel = TempData["PromotionManagementModel"] as PromotionManagementModel;
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            if (TempData.ContainsKey("AdvertisementModel")) ResponseModel.AdvertisementManagementModel = TempData["AdvertisementModel"] as AdvertisementManagementModel;
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.StartIndex2 = StartIndex2;
            ViewBag.PageSize2 = PageSize2;
            ViewBag.TotalData = promotionalImages != null && promotionalImages.Any() ? promotionalImages[0].TotalRecords : 0;
            ViewBag.TotalData2 = advertisementImages != null && advertisementImages.Any() ? advertisementImages[0].TotalRecords : 0;
            ResponseModel.SearchFilter = !string.IsNullOrEmpty(Request.SearchFilter) ? Request.SearchFilter : null;
            ResponseModel.SearchFilter2 = !string.IsNullOrEmpty(Request.SearchFilter2) ? Request.SearchFilter2 : null;
            return View(ResponseModel);
        }
        [HttpGet]
        public ActionResult ManagePromotionalImage(string Id)
        {
            var i = !string.IsNullOrEmpty(Id) ? Id.DecryptParameter() : null;
            if (string.IsNullOrEmpty(i))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("GetPromotionalImages", "PromotionManagement");
            }
            var dbResponse = _business.GetPromotionalImageById(i);
            var model = dbResponse.MapObject<PromotionManagementModel>();
            model.Id = model.Id.EncryptParameter();
            TempData["PromotionManagementModel"] = model;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("GetPromotionalImages", "PromotionManagement");
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ManagePromotionalImage(PromotionManagementModel promotionManagementModel, HttpPostedFileBase ImagePathFile)
        {
            System.IO.Stream stream = ImagePathFile.InputStream;

            System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
            //int width = img.Width;
            //int height = img.Height;
            //int size=img.Size.Width;
            //int sizeh=img.Size.Height;
            if (!ModelState.IsValid)
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Please fill all required fields",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                TempData["PromotionManagementModel"] = promotionManagementModel;
                TempData["RenderId"] = "Manage";
                return RedirectToAction("GetPromotionalImages", "PromotionManagement");
            }
            var id = !string.IsNullOrEmpty(promotionManagementModel.Id) ? promotionManagementModel.Id.DecryptParameter() : null;
            if (!string.IsNullOrEmpty(promotionManagementModel.Id) && string.IsNullOrEmpty(id))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid promotion details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("GetPromotionalImages", "PromotionManagement");
            }

            if (ImagePathFile == null)
            {
                if (string.IsNullOrEmpty(promotionManagementModel.ImagePath))
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Image required",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    TempData["PromotionManagementModel"] = promotionManagementModel;
                    TempData["RenderId"] = "Manage";
                    return RedirectToAction("GetPromotionalImages", "PromotionManagement");
                }
            }
            var promoImageCommon = promotionManagementModel.MapObject<PromotionManagementCommon>();
            string fileName = string.Empty;
            var allowedContenttype = AllowedImageContentType();
            if (ImagePathFile != null)
            {
                var contentType = ImagePathFile.ContentType;
                var ext = Path.GetExtension(ImagePathFile.FileName);
                if (allowedContenttype.Contains(contentType.ToLower()))
                {
                    fileName = $"{AWSBucketFolderNameModel.ADMIN}/PromotionalImage_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{ext.ToLower()}";
                    promoImageCommon.ImagePath = $"/{fileName}";
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "File Must be .jpg, .png, .jpeg, .heif",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    TempData["PromotionManagementModel"] = promotionManagementModel;
                    TempData["RenderId"] = "Manage";
                    return RedirectToAction("GetPromotionalImages", "PromotionManagement");
                }
            }
            promoImageCommon.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
            promoImageCommon.ActionIP = ApplicationUtilities.GetIP();
            promoImageCommon.Id = id;
            var serviceResp = _business.EditPromotionalImage(promoImageCommon);
            if (serviceResp != null && serviceResp.Code == ResponseCode.Success)
            {
                if (ImagePathFile != null) await ImageHelper.ImageUpload(fileName, ImagePathFile);
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = serviceResp.Message ?? "Success",
                    Title = NotificationMessage.SUCCESS.ToString(),
                });
                return RedirectToAction("GetPromotionalImages", "PromotionManagement");
            }
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = NotificationMessage.INFORMATION,
                Message = serviceResp.Message ?? "Failed",
                Title = NotificationMessage.INFORMATION.ToString()
            });
            TempData["PromotionManagementModel"] = promotionManagementModel;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("GetPromotionalImages", "PromotionManagement");
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult BlockUnblockPromotionalImage(string Id)
        {
            var i = !string.IsNullOrEmpty(Id) ? Id.DecryptParameter() : null;
            if (string.IsNullOrEmpty(i))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return Json(new { Code = 1, Message = "Invalid Request." });
            }
            var promoImageCommon = new PromotionManagementCommon()
            {
                Id = i,
                ActionUser = ApplicationUtilities.GetSessionValue("Username")?.ToString(),
                ActionIP = ApplicationUtilities.GetIP()
            };
            var dbResponse = _business.DeletePromotionalImage(promoImageCommon);
            if (dbResponse.Code == ResponseCode.Success)
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = dbResponse.Message ?? "Success",
                    Title = NotificationMessage.SUCCESS.ToString(),
                });
            }
            else
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = dbResponse.Message ?? "Failed",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
            }
            return Json(dbResponse.Message);
        }

        #region Advertisement Image

        [HttpGet]
        public ActionResult ManageAdvertisementImage(string Id,string SearchFilter2, int StartIndex2 = 0, int PageSize2 = 10)
        {
            var i = !string.IsNullOrEmpty(Id) ? Id.DecryptParameter() : null;
            var models = new PromotionManagementCommonModel
            {
                SearchFilter2 = SearchFilter2,
                ListType = "a"
            };
            
            if (string.IsNullOrEmpty(i))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("GetPromotionalImages", "PromotionManagement",new { StartIndex2 = StartIndex2 ,PageSize2 = PageSize2 });
            }
            var dbResponse = _business.GetAdvertisementImageById(i);
            var model = dbResponse.MapObject<AdvertisementManagementModel>();
            model.Id = model.Id.EncryptParameter();
            TempData["AdvertisementModel"] = model;
            TempData["RenderId"] = "ManageAdvertisement";
            TempData["AdvertisementManagementModel"] = models;
            return RedirectToAction("GetPromotionalImages", "PromotionManagement", new {  StartIndex2 = StartIndex2, PageSize2 = PageSize2 });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageAdvertisementImage(AdvertisementManagementModel promotionManagementModel, HttpPostedFileBase ImagePathFile)
        {
            //System.IO.Stream stream = ImagePathFile.InputStream;

            //System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
            //int width = img.Width;
            //int height = img.Height;
            //int size = img.Size.Width;
            //int sizeh = img.Size.Height;

            if (!ModelState.IsValid)
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Please fill all required fields",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                TempData["AdvertisementModel"] = promotionManagementModel;
                TempData["RenderId"] = "ManageAdvertisement";
                return RedirectToAction("GetPromotionalImages", "PromotionManagement");
            }
            var id = !string.IsNullOrEmpty(promotionManagementModel.Id) ? promotionManagementModel.Id.DecryptParameter() : null;
            if (!string.IsNullOrEmpty(promotionManagementModel.Id) && string.IsNullOrEmpty(id))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid promotion details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("GetPromotionalImages", "PromotionManagement");
            }

            if (ImagePathFile == null)
            {
                if (string.IsNullOrEmpty(promotionManagementModel.ImagePath))
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Image required",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    TempData["AdvertisementModel"] = promotionManagementModel;
                    TempData["RenderId"] = "ManageAdvertisement";
                    return RedirectToAction("GetPromotionalImages", "PromotionManagement");
                }
            }
            var promoImageCommon = promotionManagementModel.MapObject<AdvertisementDetailCommon>();
            string fileName = string.Empty;
            var allowedContenttype = AllowedImageContentType();
            if (ImagePathFile != null)
            {
                var contentType = ImagePathFile.ContentType;
                var ext = Path.GetExtension(ImagePathFile.FileName);
                if (allowedContenttype.Contains(contentType.ToLower()))
                {
                    fileName = $"{AWSBucketFolderNameModel.ADMIN}/AdvertisementImage_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}{ext.ToLower()}";
                    promoImageCommon.ImagePath = $"/{fileName}";
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "File Must be .jpg, .png, .jpeg, .heif",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    TempData["AdvertisementModel"] = promotionManagementModel;
                    TempData["RenderId"] = "ManageAdvertisement";
                    return RedirectToAction("GetPromotionalImages", "PromotionManagement");
                }
            }
            promoImageCommon.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
            promoImageCommon.ActionIP = ApplicationUtilities.GetIP();
            promoImageCommon.Id = id;
            var serviceResp = _business.UpdateAdvertisementImage(promoImageCommon);
            if (serviceResp != null && serviceResp.Code == ResponseCode.Success)
            {
                if (ImagePathFile != null) await ImageHelper.ImageUpload(fileName, ImagePathFile);
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.SUCCESS,
                    Message = serviceResp.Message ?? "Success",
                    Title = NotificationMessage.SUCCESS.ToString(),
                });
                return RedirectToAction("GetPromotionalImages", "PromotionManagement");
            }
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = NotificationMessage.INFORMATION,
                Message = serviceResp.Message ?? "Failed",
                Title = NotificationMessage.INFORMATION.ToString()
            });
            TempData["AdvertisementModel"] = promotionManagementModel;
            TempData["RenderId"] = "ManageAdvertisement";
            return RedirectToAction("GetPromotionalImages", "PromotionManagement");
        }
        #endregion
    }
}