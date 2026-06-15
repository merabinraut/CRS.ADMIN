using CRS.ADMIN.APPLICATION.CustomHelpers;
using CRS.ADMIN.APPLICATION.Library;
using CRS.ADMIN.APPLICATION.Models.ClubManagement;
using CRS.ADMIN.APPLICATION.Models.PlanManagement;
using CRS.ADMIN.BUSINESS.ClubPlanManagement;
using CRS.ADMIN.BUSINESS.PlanManagement;
using CRS.ADMIN.SHARED;
using CRS.ADMIN.SHARED.ClubManagement;
using CRS.ADMIN.SHARED.PlanManagement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    public class ClubPlanManagementController : BaseController
    {
        private readonly IClubPlanManagementBusiness _BUSS;
        private readonly IPlanManagementBusiness _planBusiness;
        public ClubPlanManagementController(IClubPlanManagementBusiness BUSS, IPlanManagementBusiness planBusiness)
        {
            _BUSS = BUSS;
            _planBusiness = planBusiness;
        }
        [HttpGet]
        public ActionResult ClubPlanList(string AgentId, string TapValue = "", string SearchFilter = "", int StartIndex = 0, int PageSize = 10, string clubId = "", string planId = "", string Sno = "", string type = "")
        {
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            ViewBag.AgentId = AgentId;
            ViewBag.TapValue = TapValue;
            ViewBag.TabValue = TapValue;
            ViewBag.SearchFilter = SearchFilter;
            ViewBag.IsBackAllowed = true;
            ViewBag.BackButtonURL = "/ClubManagement/ClubList";
            string RenderId = "";
            var aId = !string.IsNullOrEmpty(AgentId) ? AgentId.DecryptParameter() : null;
            if (!string.IsNullOrEmpty(Sno) && !string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(planId))
            {
                var planRequestResponse = _planBusiness.ApprovePlanRequest(Sno, type, planId);
                if (planRequestResponse != null && planRequestResponse.Code == 0)
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.SUCCESS,
                        Message = planRequestResponse.Message ?? "Saved successfully",
                        Title = NotificationMessage.SUCCESS.ToString()
                    });
                    string apiUrl = ConfigurationManager.AppSettings["RevalidateApiUrl"];
                    ExternalApiCallHelpers.CallApi(apiUrl, HttpMethod.Get);
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.ERROR,
                        Message = planRequestResponse?.Message ?? "Bad request",
                        Title = NotificationMessage.ERROR.ToString()
                    });
                }
                return RedirectToAction("ClubPlanList", "ClubPlanManagement", new { AgentId, TapValue = "02" });
            }
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
            if (TempData.ContainsKey("RenderId")) RenderId = TempData["RenderId"].ToString();
            ViewBag.PopUpRenderValue = !string.IsNullOrEmpty(RenderId) ? RenderId : null;
            response.ClubId = aId;
            response.ManageClubPlanModel.ClubId = aId;     

            ViewBag.TimeIntervalList = Dropdown(ApplicationUtilities.LoadDropdownValuesList("PLANTIMEINTERVAL", aId, culture) as List<MultipleItemCommon>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.EntryTimeList = ViewBag.TimeIntervalList;
            ViewBag.TimeList = ApplicationUtilities.SetDDLValue(GetDictionaryFromResponse(_planBusiness.GetDDL("8"), culture), null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            ViewBag.PlansList = ApplicationUtilities.LoadDropdownList("CLUBPLANS") as Dictionary<string, string>;

            List<ClubplanListCommon> ClubplanListCommon = _BUSS.GetClubPlanList(culture, aId);
            response.planList = ClubplanListCommon.MapObjects<ClubplanListModel>();
            if (response.planList.Count > 0)
            {
                response.planList.ForEach(planIdentity =>
                {
                    //planIdentity.PlanListId = !string.IsNullOrEmpty(planIdentity.PlanListId) ? planIdentity.PlanListId.EncryptParameter() : planIdentity.PlanListId;
                    planIdentity.PlanId = !string.IsNullOrEmpty(planIdentity.PlanId) ? planIdentity.PlanId.EncryptParameter() : planIdentity.PlanId; // Call your encryption method here
                    planIdentity.Id = !string.IsNullOrEmpty(planIdentity.Id) ? planIdentity.Id.EncryptParameter() : planIdentity.Id; ; // Call your encryption method here

                });
            }

            List<PlanRequesResponseListCommon> planRequestList = _BUSS.GetClubOwnPlanList(culture, aId);
            response.ClubPlanResponseModel = planRequestList.MapObjects<PlanRequesResponseListModel>();

            if (response.ClubPlanResponseModel != null && response.ClubPlanResponseModel.Any())
            {
                response.ClubPlanResponseModel.ForEach(item =>
                {
                    item.clubId = !string.IsNullOrEmpty(item.clubId)
                        ? item.clubId.EncryptParameter() : item.clubId;
                    item.planId = !string.IsNullOrEmpty(item.planId)
                        ? item.planId.EncryptParameter() : item.planId;
                });
            }

            string planRenderId = "";
            if (TempData.ContainsKey("ClubPlanManagementModel"))
                response.clubPlanManageModel = TempData["ClubPlanManagementModel"] as PlanRequesResponseListModel;
            if (TempData.ContainsKey("PlanRenderId")) planRenderId = TempData["PlanRenderId"].ToString();

            if (string.IsNullOrEmpty(planRenderId) && !string.IsNullOrEmpty(clubId) && !string.IsNullOrEmpty(planId))
            {
                var decryptedClubId = clubId.DecryptParameter();
                var decryptedPlanId = planId.DecryptParameter();
                if (!string.IsNullOrEmpty(decryptedClubId) && !string.IsNullOrEmpty(decryptedPlanId))
                {
                    var getClbPlanDetails = _planBusiness.GetPlanRequestDetails(decryptedClubId, decryptedPlanId);
                    var resp = getClbPlanDetails.MapObject<PlanRequesResponseListModel>();
                    resp.planId = resp.planId.EncryptParameter();
                    resp.planTime = getClbPlanDetails.planTime;
                    resp.lastEntryTime = getClbPlanDetails.lastEntryTime;
                    resp.plantype = resp.plantype.EncryptParameter();
                    resp.numberOfPeople = resp.numberOfPeople;
                    resp.nomination = resp.nomination;
                    resp.clubId = resp.clubId.EncryptParameter();
                    response.clubPlanManageModel = resp;
                    planRenderId = "ManageClubPlan";
                    if (string.IsNullOrEmpty(TapValue))
                    {
                        TapValue = "02";
                        ViewBag.TapValue = TapValue;
                        ViewBag.TabValue = TapValue;
                    }
                }
            }

            ViewBag.PopUpClubManageValue = !string.IsNullOrEmpty(planRenderId) ? planRenderId : null;

            bool isexception = false;
            if (string.IsNullOrEmpty(ViewBag.PopUpRenderValue))
            {
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
                        try
                        {
                            planIdentity.StaticDataValue = planIdentity.StaticDataValue.EncryptParameter(); // Call your encryption method here
                            if (planIdentity.name.ToLower() == "plan" ||
                                 //planIdentity.name.ToLower() == "lastordertime" ||
                                 planIdentity.name.ToLower() == "lastentrytime")
                            {
                                planIdentity.IdentityDescription = planIdentity.IdentityDescription.EncryptParameter();
                            }
                            //planIdentity.IdentityDescription = planIdentity.name.ToLower() == "plan" ? planIdentity.IdentityDescription.EncryptParameter() : planIdentity.IdentityDescription; // Call your encryption method here
                            planIdentity.PlanId = planIdentity.name.ToLower() == "plan" ? ViewBag.PlansList[planIdentity.IdentityDescription] : planIdentity.IdentityDescription;  // Call your encryption method here
                            planIdentity.PlanListId = planIdentity.PlanListId.EncryptParameter(); // Call your encryption method here
                        }
                        catch (Exception ex)
                        {
                            this.AddNotificationMessage(new NotificationModel()
                            {
                                NotificationType = NotificationMessage.INFORMATION,
                                Message = ex.Message,
                                Title = NotificationMessage.INFORMATION.ToString(),
                            });
                            isexception = true;
                        }
                    });
                });
            }
            if (isexception == true)
            {
                return RedirectToAction("ClubList", "ClubManagement");
            }
          
            ViewBag.StartIndex = StartIndex;
            ViewBag.PageSize = PageSize;
            if(TapValue == "02")
            {
                ViewBag.TotalData = response.ClubPlanResponseModel != null ? response.ClubPlanResponseModel.Count : 0;
                if(response.ClubPlanResponseModel !=null && response.ClubPlanResponseModel.Any())
                {
                    response.ClubPlanResponseModel = response.ClubPlanResponseModel
                        .Skip(StartIndex)
                        .Take(PageSize)
                        .ToList();
                }
            }
            else 
            {
                ViewBag.TotalData = response.planList.Count;
                response.planList = response.planList
                    .Skip(StartIndex)
                    .Take(PageSize)
                    .ToList();
            }
            response.ClubId = response.ClubId.EncryptParameter();
            response.ManageClubPlanModel.ClubId = response.ClubId;
            return View(response);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageClubPlan(ManageClubPlanModel Model)
        {
            string ErrorMessage = string.Empty;
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            ViewBag.PlansList = ApplicationUtilities.LoadDropdownList("CLUBPLANS") as Dictionary<string, string>;
            // ViewBag.TimeIntervalList = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("PLANTIMEINTERVAL", Model.ClubId.DecryptParameter()) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---"); ;
            ViewBag.TimeIntervalList = Dropdown(ApplicationUtilities.LoadDropdownValuesList("PLANTIMEINTERVAL", Model.ClubId.DecryptParameter(), culture) as List<MultipleItemCommon>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
            string concatenateplanvalue = string.Empty;
            bool isexception = false;
            bool isduplicate = false;
            Model.ClubPlanDetailList.ForEach(planList =>
            {
                concatenateplanvalue += ", ";
                planList.PlanIdentityList.ForEach(planIdentity =>
                {
                    try
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
                    }
                    catch (Exception ex)
                    {
                        this.AddNotificationMessage(new NotificationModel()
                        {
                            NotificationType = NotificationMessage.INFORMATION,
                            Message = ex.Message,
                            Title = NotificationMessage.INFORMATION.ToString(),
                        });
                        isexception = true;
                    }

                });
            });

            if (isexception == true)
            {
                TempData["ManageClubPlanModel"] = Model;
                TempData["RenderId"] = "Manage";
                return RedirectToAction("ClubPlanList", "ClubPlanManagement", new
                {
                    AgentId = Model.ClubId
                });
            }
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
                    return RedirectToAction("ClubPlanList", "ClubPlanManagement", new
                    {
                        AgentId = Model.ClubId
                    });
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
                        return RedirectToAction("ClubPlanList", "ClubPlanManagement", new
                        {
                            AgentId = Model.ClubId
                        });
                    }
                }

                commonModel.ClubPlanDetailList.ForEach(planList =>
                {
                    planList.PlanIdentityList.ForEach(planIdentity =>
                    {
                        try
                        {
                            // string decryptedDescription = planIdentity.name.ToLower() == "plan" ? planIdentity.IdentityDescription.DecryptParameter() : planIdentity.IdentityDescription;
                            planIdentity.StaticDataValue = planIdentity.StaticDataValue.DecryptParameter();
                            if (planIdentity.name.ToLower() == "plan" ||
                                 planIdentity.name.ToLower() == "lastordertime" ||
                                 planIdentity.name.ToLower() == "lastentrytime")
                            {
                                planIdentity.IdentityDescription = planIdentity.IdentityDescription.DecryptParameter();
                            }
                            //planIdentity.IdentityDescription = planIdentity.name.ToLower() == "plan" ? decryptedDescription : planIdentity.IdentityDescription;
                            planIdentity.PlanListId = planIdentity.PlanListId.DecryptParameter(); // Call your encryption method here
                        }
                        catch (Exception ex)
                        {
                            this.AddNotificationMessage(new NotificationModel()
                            {
                                NotificationType = NotificationMessage.INFORMATION,
                                Message = ex.Message,
                                Title = NotificationMessage.INFORMATION.ToString(),
                            });
                            isexception = true;
                        }

                    });
                });
                if (isexception == true)
                {
                    return RedirectToAction("ClubPlanList", "ClubPlanManagement", new
                    {
                        AgentId = Model.ClubId
                    });
                }
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
                    return RedirectToAction("ClubPlanList", "ClubPlanManagement", new
                    {
                        AgentId = Model.ClubId
                    });
                }
                else
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = NotificationMessage.INFORMATION.ToString()
                    });

                    return RedirectToAction("ClubPlanList", "ClubPlanManagement", new
                    {
                        AgentId = Model.ClubId
                    });
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
            return RedirectToAction("ClubPlanList", "ClubPlanManagement", new
            {
                AgentId = Model.ClubId
            });
        }

        [HttpGet]
        public ActionResult ManageClubPlan(string AgentId = "", string Id = "")
        {
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            var response = new ManageClubPlanCommonModel();
            response.ManageClubPlanModel = new ManageClubPlanModel();
            ViewBag.PlansList = ApplicationUtilities.LoadDropdownList("CLUBPLANS") as Dictionary<string, string>;
            if (!string.IsNullOrEmpty(AgentId))
            {
                var agentids = AgentId.DecryptParameter();
                var planlistid = Id.DecryptParameter();
                if (string.IsNullOrEmpty(agentids))
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Invalid club plan details",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    return RedirectToAction("ClubPlanList", "ClubPlanManagement", new
                    {
                        AgentId = AgentId
                    });
                }
                if (string.IsNullOrEmpty(planlistid))
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Invalid club plan details",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    return RedirectToAction("ClubPlanList", "ClubPlanManagement", new
                    {
                        AgentId = AgentId
                    });
                }
                ViewBag.TimeIntervalList = Dropdown(ApplicationUtilities.LoadDropdownValuesList("PLANTIMEINTERVAL", agentids, culture) as List<MultipleItemCommon>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---");
                //ViewBag.TimeIntervalList = ApplicationUtilities.SetDDLValue(ApplicationUtilities.LoadDropdownList("PLANTIMEINTERVAL", agentids) as Dictionary<string, string>, null, culture.ToLower() == "ja" ? "--- 選択 ---" : "--- Select ---"); ;
                bool isexception = false;
                List<PlanListCommon> planlist = _BUSS.EditClubPlanIdentityList(culture, agentids, planlistid);
                response.ManageClubPlanModel.ClubPlanDetailList = planlist.MapObjects<PlanList>();
                response.ManageClubPlanModel.ClubPlanDetailList.ForEach(planList =>
                {
                    planList.PlanIdentityList.ForEach(planIdentity =>
                    {
                        try
                        {
                            planIdentity.StaticDataValue = planIdentity.StaticDataValue.EncryptParameter(); // Call your encryption method here
                            if (planIdentity.name.ToLower() == "plan" ||
                                 planIdentity.name.ToLower() == "lastordertime" ||
                                 planIdentity.name.ToLower() == "lastentrytime")
                            {
                                planIdentity.IdentityDescription = planIdentity.IdentityDescription.EncryptParameter();
                            }
                            //planIdentity.IdentityDescription = planIdentity.name.ToLower() == "plan" ? planIdentity.IdentityDescription.EncryptParameter() : planIdentity.IdentityDescription; // Call your encryption method here                            
                            planIdentity.PlanId = planIdentity.name.ToLower() == "plan" ? ViewBag.PlansList[planIdentity.IdentityDescription] : planIdentity.IdentityDescription;  // Call your encryption method here
                            planIdentity.PlanListId = planIdentity.PlanListId.EncryptParameter();
                        }
                        catch (Exception ex)
                        {
                            this.AddNotificationMessage(new NotificationModel()
                            {
                                NotificationType = NotificationMessage.INFORMATION,
                                Message = ex.Message,
                                Title = NotificationMessage.INFORMATION.ToString(),
                            });
                            isexception = true;
                        }

                    });
                });
                if (isexception == true)
                {
                    return RedirectToAction("ClubPlanList", "ClubPlanManagement", new
                    {
                        AgentId = AgentId
                    });
                }
            }
            response.ClubId = AgentId;
            response.ManageClubPlanModel.ClubId = AgentId;
            TempData["ManageClubPlanModel"] = response.ManageClubPlanModel;
            TempData["RenderId"] = "Manage";
            TempData["EditPlan"] = response.ManageClubPlanModel;
            return RedirectToAction("ClubPlanList", "ClubPlanManagement", new
            {
                AgentId = AgentId
            });
        }

        [HttpGet]
        public ActionResult BlockUnblockPlan(string AgentId = "", string Id = "", string Status = "")
        {
            var culture = Request.Cookies["culture"]?.Value;
            culture = string.IsNullOrEmpty(culture) ? "ja" : culture;
            var response = new ManageClubPlanCommonModel();
            response.ManageClubPlanModel = new ManageClubPlanModel();
            ViewBag.PlansList = ApplicationUtilities.LoadDropdownList("CLUBPLANS") as Dictionary<string, string>;
            if (!string.IsNullOrEmpty(AgentId))
            {
                var agentids = AgentId.DecryptParameter();
                var planlistid = Id.DecryptParameter();
                if (string.IsNullOrEmpty(agentids))
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Invalid club plan details",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    return RedirectToAction("ClubPlanList", "ClubPlanManagement", new
                    {
                        AgentId = AgentId
                    });
                }
                if (string.IsNullOrEmpty(planlistid))
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = NotificationMessage.INFORMATION,
                        Message = "Invalid club plan details",
                        Title = NotificationMessage.INFORMATION.ToString(),
                    });
                    return RedirectToAction("ClubPlanList", "ClubPlanManagement", new
                    {
                        AgentId = AgentId
                    });
                }
                ClubplanListCommon objClubplanListCommon = new ClubplanListCommon();
                objClubplanListCommon.PlanId = planlistid;
                objClubplanListCommon.ClubId = agentids;
                objClubplanListCommon.Status = Status;
                var ActionUser = ApplicationUtilities.GetSessionValue("Username").ToString();
                var ActionIP = ApplicationUtilities.GetIP();
                var dbResponse = _BUSS.BlockUnblockPlan(objClubplanListCommon, ActionUser, ActionIP);
                if (dbResponse != null && dbResponse.Code == 0)
                {
                    this.AddNotificationMessage(new NotificationModel()
                    {
                        NotificationType = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS : NotificationMessage.INFORMATION,
                        Message = dbResponse.Message ?? "Failed",
                        Title = dbResponse.Code == ResponseCode.Success ? NotificationMessage.SUCCESS.ToString() : NotificationMessage.INFORMATION.ToString()
                    });
                    return RedirectToAction("ClubPlanList", "ClubPlanManagement", new
                    {
                        AgentId = AgentId
                    });
                }

            }
            return RedirectToAction("ClubPlanList", "ClubPlanManagement", new
            {
                AgentId = AgentId
            });

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ClubPlanList(PlanRequesResponseListModel request, string AgentId = "")
        {
            if (string.IsNullOrEmpty(request.clubId) || string.IsNullOrEmpty(request.planId) || request.numberOfPeople <= 0 || string.IsNullOrEmpty(request.nomination) || string.IsNullOrEmpty(request.planTime))
            {
                this.AddNotificationMessage(new NotificationModel()
                {
                    NotificationType = NotificationMessage.INFORMATION,
                    Message = "Invalid request",
                    Title = NotificationMessage.INFORMATION.ToString(),
                });
                return RedirectToAction("ClubPlanList", "ClubPlanManagement", new { AgentId = AgentId, TapValue = "02" });
            }

            var requestMapped = request.MapObject<PlanRequesRequestCommon>();
            requestMapped.clubId = request.clubId.DecryptParameter();
            requestMapped.planId = request.planId.DecryptParameter();
            var dbResponse = _planBusiness.ManageClubPlan(requestMapped);
            this.AddNotificationMessage(new NotificationModel()
            {
                NotificationType = NotificationMessage.SUCCESS,
                Message = dbResponse.Message ?? "Success",
                Title = NotificationMessage.SUCCESS.ToString(),
            });
            string apiUrl = ConfigurationManager.AppSettings["RevalidateApiUrl"];
            ExternalApiCallHelpers.CallApi(apiUrl, HttpMethod.Get);
            return RedirectToAction("ClubPlanList", "ClubPlanManagement", new { AgentId = AgentId, TapValue = "02" });
        }

        Dictionary<string, string> GetDictionaryFromResponse(List<StaticDataCommon> response, string culture)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (var item in response)
            {
                dictionary.Add(item.StaticValue.EncryptParameter(), culture == "en" ? item.StaticLabelEnglish : item.StaticLabelJapanese);
            }
            return dictionary;
        }

        public static List<SelectListItem> Dropdown(List<MultipleItemCommon> obj, string selectedVal, string defLabel = "", bool isTextAsValue = false,string status="")
        {
            List<SelectListItem> items = new List<SelectListItem>();
            
            if (!string.IsNullOrWhiteSpace(defLabel))
            {
                items.Add(new SelectListItem { Text = defLabel, Value = "", Disabled = true });
            }
            if (obj.Count > 0)
            {

                foreach (var item in obj)
                {
                    string Value = item.Value;
                    string Name = item.Text;
                    string Status = item.Item1;
                    if (isTextAsValue)
                        Value = Name;
                    bool disabled = !string.IsNullOrWhiteSpace(Status) && Status.ToLower() == "inactive";
                    if (Value == selectedVal)
                    {
                        items.Add(new SelectListItem { Text = Name, Value = Value,  Selected = true, Disabled = disabled });
                    }
                    
                    else
                    {
                        items.Add(new SelectListItem { Text = Name, Value = Value ,  Disabled = disabled });
                    }
                }
            }
            return items;
        }
    }

}
