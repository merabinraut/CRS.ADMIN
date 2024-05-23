using CRS.ADMIN.APPLICATION.Helper;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.HostManagement;
using CRS.ADMIN.SHARED.PaginationManagement;
using CRS.ADMIN.SHARED;
using System.Collections.Generic;
using System.Web.Mvc;
using CRS.ADMIN.APPLICATION.Models.ClubManagement;
using CRS.ADMIN.SHARED.ClubManagement;
using static Google.Apis.Requests.BatchRequest;
using CRS.ADMIN.BUSINESS.ClubManagement;
using System.Net.Http;
using CRS.ADMIN.BUSINESS.ClubPlanManagement;
using System.Net;
using System.IO;
using System.Xml.Serialization;
using System.Web.UI.WebControls;
using CRS.ADMIN.APPLICATION.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class ClubPlanManagementController : BaseController
    {
        private readonly IClubPlanManagementBusiness _BUSS;
        private readonly HttpClient _httpClient;
        public ClubPlanManagementController(IClubPlanManagementBusiness BUSS)
        {
            _BUSS = BUSS;
            
        }
        public ActionResult ClubPlanList(string AgentId, string SearchFilter = "", int StartIndex = 0, int PageSize = 10)
        {
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            ViewBag.AgentId = AgentId;
            ViewBag.SearchFilter = SearchFilter;
            string RenderId = "";
            var aId = !string.IsNullOrEmpty(AgentId) ? AgentId.DecryptParameter() : null;
            if (string.IsNullOrEmpty(aId))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid details",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("ClubList", "ClubManagement");
            }
        
            var response = new ManageClubPlanCommonModel();
            if (TempData.ContainsKey("ManageClubPlanModel")) response.ManageClubPlanModel = TempData["ManageClubPlanModel"] as ManageClubPlanModel;
            else response.ManageClubPlanModel = new ManageClubPlanModel();
            response.ClubId = aId; ;
            response.ManageClubPlanModel.ClubId = aId; ;
            ViewBag.PlansList = ApplicationUtilities.LoadDropdownList("CLUBPLANS") as Dictionary<string, string>;
            List<ClubplanListCommon> ClubplanListCommon = _BUSS.GetClubPlanList(culture, aId);
            response.planList = ClubplanListCommon.MapObjects<ClubplanListModel>();
            response.planList.ForEach(planIdentity =>
                {
                    planIdentity.PlanId =!string.IsNullOrEmpty(planIdentity.PlanId) ? planIdentity.PlanId.EncryptParameter(): planIdentity.PlanId; // Call your encryption method here
                    planIdentity.Id = !string.IsNullOrEmpty(planIdentity.Id) ? planIdentity.Id.EncryptParameter() : planIdentity.Id; ; // Call your encryption method here
                    
                });
            List<PlanListCommon> planlist = _BUSS.GetClubPlanIdentityList(culture, aId);          
            response.ManageClubPlanModel.ClubPlanDetailList = planlist.MapObjects<PlanList>();        
            var i = 0;
            List<PlanListCommon> planlists = new List<PlanListCommon>(planlist);
            foreach (var planDetail in planlists)
            {
                // Filter the PlanIdentityList based on the condition where PlanStatus is not equal to "B"
                var filteredPlanIdentityList = planDetail.PlanIdentityList
                      .Where(planIdentity => planIdentity.PlanStatus != "B")
                      .ToList();
                if (filteredPlanIdentityList.Count > 0)
                {
                    var distinctPlanListIds = filteredPlanIdentityList
                                            .Select(planIdentity => planIdentity.PlanListId)
                                            .Distinct()
                                            .ToList();

                    // Filter the list again to remove elements with PlanStatus equal to "B" and whose PlanListId matches any of the distinct PlanListId values
                    planDetail.PlanIdentityList = filteredPlanIdentityList
                        .Where(planIdentity => !planIdentity.PlanListId.Contains("B") || !distinctPlanListIds.Contains(planIdentity.PlanListId))
                        .ToList();
                    i++;
                }
                else if (planDetail.PlanIdentityList.Any(planIdentity => planIdentity.PlanStatus == "B"))
                {
                    response.ManageClubPlanModel.ClubPlanDetailList.RemoveAt(i);

                }


            }
            response.ManageClubPlanModel.ClubPlanDetailList.ForEach(planList =>
            {
                planList.PlanIdentityList.ForEach(planIdentity =>
                {
                    planIdentity.StaticDataValue = planIdentity.StaticDataValue.EncryptParameter(); // Call your encryption method here
                    planIdentity.IdentityDescription = planIdentity.name.ToLower() == "plan" ? planIdentity.IdentityDescription.EncryptParameter() : planIdentity.IdentityDescription; // Call your encryption method here
                    planIdentity.PlanId = planIdentity.name.ToLower() == "plan" ? ViewBag.PlansList[planIdentity.IdentityDescription] : planIdentity.IdentityDescription;  // Call your encryption method here
                    
                });
            });

            //string xmlData = SerializeToXml(response.ManageClubPlanModel.ClubPlanDetailList);
            //response.ClubListModel = dbResponse.MapObjects<ClubListModel>();
            ViewBag.BackButtonURL = "/ClubManagement/ClubList";
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalData = 0;
            return View(response);
        }
        
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageClubPlan(ManageClubPlanModel Model)
        {
            string ErrorMessage = string.Empty;
            ViewBag.PlansList = ApplicationUtilities.LoadDropdownList("CLUBPLANS") as Dictionary<string, string>;
            string concatenateplanvalue = string.Empty;

            bool isduplicate = false;
            Model.ClubPlanDetailList.ForEach(planList =>
            {
                concatenateplanvalue += ", ";
                planList.PlanIdentityList.ForEach(planIdentity =>
                {

                    planIdentity.PlanId = planIdentity.name.ToLower() == "plan" ? ViewBag.PlansList[planIdentity.IdentityDescription] : planIdentity.IdentityDescription;  // Call your encryption method here

                    if (planIdentity.name.ToLower() == "plan")
                    {

                        if (concatenateplanvalue.Contains(planIdentity.IdentityDescription.DecryptParameter()))
                        {
                            isduplicate = true;
                        }
                        concatenateplanvalue += planIdentity.IdentityDescription.DecryptParameter();
                    }

                });
            });

    
            if (ModelState.IsValid)
            {
                if (isduplicate == true)
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Duplicate plan name.",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });

                    TempData["ManageClubPlanModel"] = Model;
                    TempData["RenderId"] = "Manage";
                    return RedirectToAction("ClubPlanList", "ClubPlanManagement");
                }

                ManageClubPlan commonModel = Model.MapObject<ManageClubPlan>();
                commonModel.ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                commonModel.ActionIP = ApplicationUtilities.GetIP();
                if (!string.IsNullOrEmpty(commonModel.ClubId))
                {
                    commonModel.ClubId = commonModel.ClubId.DecryptParameter();
                    if (string.IsNullOrEmpty(commonModel.ClubId))
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = "Invalid club details.",
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });

                        TempData["ManageClubPlanModel"] = Model;
                        TempData["RenderId"] = "Manage";
                        return RedirectToAction("ClubPlanList", "ClubPlanManagement");
                    }
                }
              
                commonModel.ClubPlanDetailList.ForEach(planList =>
                {
                    planList.PlanIdentityList.ForEach(planIdentity =>
                    {
                        //string decryptedDescription = planIdentity.name.ToLower() == "plan" ? planIdentity.IdentityDescription.DecryptParameter() : planIdentity.IdentityDescription;
                        planIdentity.StaticDataValue = planIdentity.StaticDataValue.DecryptParameter();
                        //planIdentity.IdentityDescription = planIdentity.name.ToLower() == "plan" ? decryptedDescription : planIdentity.IdentityDescription;

                    });
                });

                var blockplanlistid = 1;                
                var dbResponse = _BUSS.ManageClubPlan(commonModel);
                if (dbResponse != null && dbResponse.Code == 0)
                {                   
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
                    });
                    return RedirectToAction("ClubPlanList", "ClubPlanManagement");
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = NotificationMessage.INFORMATION.ToString()
                    });

                    TempData["ManageClubPlanModel"] = Model;
                    TempData["RenderId"] = "Manage";
                    return RedirectToAction("ClubPlanList", "ClubPlanManagement");
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
            TempData["ManageClubPlanModel"] = Model;
            TempData["RenderId"] = "Manage";
            return RedirectToAction("ClubPlanList", "ClubPlanManagement");
        }

    }
}