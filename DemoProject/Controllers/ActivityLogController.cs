﻿using DemoProject.Model;
using DemoProject.Service;
using DemoProject.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoProject.Controllers
{
    public class ActivityLogController : BaseController
    {
        // GET: ActivityLog
        private readonly ActivityLogService _activityLogService;
        private readonly UserProfileService _userProfileService;
        public ActivityLogController()
        {
            _activityLogService = new ActivityLogService();
            _userProfileService = new UserProfileService();
        }
        public ActionResult Index()
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.ACTIVITYLOG.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            return View();
        }
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request)
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.ACTIVITYLOG.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            var data = _activityLogService.GetAllActivityLogs();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult ActivityLogDetails(int Id)
        {
            ActivityLog model = new ActivityLog();
            var data = _activityLogService.GetActivityLogById(Id);
            if (data.UserId.HasValue)
            {
                data.UserName = data.UserProfile.UserName;
            }
            return Json(RenderPartialViewToString(this, "_ActivityLogPopUp", data), JsonRequestBehavior.AllowGet);
        }
    }
}