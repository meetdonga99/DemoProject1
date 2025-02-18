using DemoProject.Filters;
using DemoProject.Helper;
using System.Web;
using System.Web.Mvc;

namespace DemoProject
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ExceptionHandlingFilter());
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthenticationFilter());
            filters.Add(new ActivityLogFilter());
        }
    }
}
