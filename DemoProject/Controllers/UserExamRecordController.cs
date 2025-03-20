using DemoProject.Model;
using DemoProject.Service;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;


namespace DemoProject.Controllers
{
    public class UserExamRecordController : BaseController
    {
        private readonly UserExamRecordService _userExamRecordService;

        public UserExamRecordController()
        {
            _userExamRecordService = new UserExamRecordService();
        }

        // GET: UserExamRecord
        public ActionResult Index()
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.USEREXAMRECORD.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            return View();
        }

        [HttpPost]
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request)
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.USEREXAMRECORD.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            var data = _userExamRecordService.GetAllUserExamRecordGrid();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}