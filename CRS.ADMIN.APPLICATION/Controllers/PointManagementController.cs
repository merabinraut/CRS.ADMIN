using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION.Controllers
{
    [OverrideActionFilters]
    public class PointManagementController : BaseController
    {

        // GET: PointManagement
        public ActionResult Index()
        {
            ViewBag.StartIndex = 1;
            ViewBag.StartIndex2 = 1;
            ViewBag.StartIndex3 = 1;
            ViewBag.StartIndex4 = 1;

            ViewBag.PageSize = 10;
            ViewBag.PageSize2 = 10;
            ViewBag.PageSize3 = 10;
            ViewBag.PageSize4 = 10;

            ViewBag.TotalData = 100;
            ViewBag.TotalData2 = 100;
            ViewBag.TotalData3= 100;
            ViewBag.TotalData4 = 100;

            return View();
        }
    }
}