﻿using DemoProject.Filters;
using System.Web;
using System.Web.Mvc;

namespace DemoProject
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthenticationFilter());
            
        }
    }
}
