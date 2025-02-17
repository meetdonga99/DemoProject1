using DemoProject.Helper;
using DemoProject.Model;
using DemoProject.Models;
using DemoProject.Service;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DemoProject.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        private readonly FormRoleMappingService _formService;
        //private readonly CommentService _commentService;
        public BaseController()
        {
            _formService = new FormRoleMappingService();
            //_commentService = new CommentService();
        }
        [AllowAnonymous]
        public ActionResult AccessDenied()
        {
            return View();
        }

        public bool CheckPermission(string formCode, string formAction)
        {
            int roleId = SessionHelper.RoleId;

            var checkPermission = _formService.CheckFormAccess(formCode, roleId);
            if (checkPermission != null)
            {
                if (formAction == AccessPermission.IsAdd)
                {
                    return checkPermission.AllowInsert;
                }
                else if (formAction == AccessPermission.IsEdit)
                {
                    return checkPermission.AllowUpdate;
                }
                else if (formAction == AccessPermission.IsDelete)
                {
                    return checkPermission.AllowDelete;
                }
                else if (formAction == AccessPermission.IsMenu)
                {
                    return checkPermission.AllowMenu;
                }
                else if (formAction == AccessPermission.IsView)
                {
                    return checkPermission.AllowView;
                }
                else if (string.IsNullOrWhiteSpace(formAction))
                {
                    if (checkPermission.AllowInsert || checkPermission.AllowUpdate)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public string RenderPartialViewToString(Controller controller, string viewName, object model = null)
        {
            if (model != null)
                controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult;
                viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);

                ViewContext viewContext;
                viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.ToString();
            }
        }

        //public ActionResult GetCommentView(string sourceType, int sourceId)
        //{
        //    CommentModel model = new CommentModel();
        //    model.SourceType = sourceType;
        //    model.SourceId = sourceId;
        //    return PartialView("_CommentThread", model);
        //}

        //public ActionResult GetCommentGridData([DataSourceRequest] DataSourceRequest request, string sourceType, int sourceId)
        //{
        //    var data = _commentService.GetCommentList(sourceId, sourceType).ToList();
        //    foreach (var item in data)
        //    {
        //        item.CreatedOn = CommonUtility.ConvertToApplicationDateTime(item.CreatedOn.GetValueOrDefault());
        //    }

        //    return FormattedJson(data.ToDataSourceResult(request), true);
        //}


        //public ActionResult AddComment(CommentModel model)
        //{
        //    if (model.SourceId > 0)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            CommentService _commentService = new CommentService();
        //            model.CreatedBy = SessionHelper.UserId;
        //            model.CreatedOn = DateTime.UtcNow;
        //            int result = _commentService.CreateUpdateComment(model);
        //            return Json(new { msg = "Success" }, JsonRequestBehavior.AllowGet);

        //        }
        //        else
        //        {
        //            var errors = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
        //            string errorMsg = errors[0][0].ErrorMessage.ToString();
        //            return Json(new { msg = errorMsg });
        //        }
        //    }
        //    else
        //    {
        //        return Json(new { msg = "Error" });
        //    }
        //}

        //public ActionResult AddCommentDialogue(int sourceId, string sourceType)
        //{
        //    CommentModel model = new CommentModel();
        //    model.SourceType = sourceType;
        //    model.SourceId = sourceId;
        //    return PartialView("_AddComment", model);
        //}

        //public ActionResult FormattedJson(object listModel, bool DateColumnIncluded = false)
        //{
        //    if (DateColumnIncluded)
        //        return Content(JsonConvert.SerializeObject(listModel, Formatting.None, new JsonSerializerSettings { DateFormatString = CommonUtility.GetDateFormat(DateColumnIncluded) }), "application/json");
        //    else
        //        return Json(listModel, JsonRequestBehavior.AllowGet);
        //}
    }
}
