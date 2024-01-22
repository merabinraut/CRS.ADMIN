using CRS.ADMIN.APPLICATION.Filters;
using System.Web.Mvc;

namespace CRS.ADMIN.APPLICATION
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new SessionExpiryFilterAttribute());
            ////filters.Add(new HandleErrorAttribute());
            //filters.Add(new ActivityLogFilter());
        }
    }
}
