using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoProject.Controllers
{
    public class QuestionController : BaseController
    {
        // GET: Question
        public ActionResult Index()
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.USER.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            return View();
        }


    }
}