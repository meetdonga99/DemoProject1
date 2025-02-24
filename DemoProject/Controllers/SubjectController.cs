using DemoProject.Model;
using DemoProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DemoProject.Service;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using DemoProject.Helper;
using DemoProject.Controllers;

namespace DemoProject.Controllers
{
    [Authorize]
    public class SubjectController : BaseController
    {
        // GET: Roles

        private readonly SubjectService _subjectService;
        private readonly MessageService _messageService;
        public SubjectController()
        {
            _subjectService = new SubjectService();
            _messageService = new MessageService();
        }
        public ActionResult Index()
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.SUBJECT.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }

            return View();
        }

        public ActionResult Create(int? id)
        {
            string actionPermission = "";
            if (id == null)
            {
                actionPermission = AccessPermission.IsAdd;
            }
            else if ((id ?? 0) > 0)
            {
                actionPermission = AccessPermission.IsEdit;
            }

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.SUBJECT.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }

            int userId = SessionHelper.UserId;
            SubjectModel model = new SubjectModel();
            if (id.HasValue)
            {
                var subjectDetail = _subjectService.GetSubjectById(id.Value);
                if (subjectDetail != null)
                {
                    model.Id = id.Value;
                    model.Name = subjectDetail.Name;
                    model.Code = subjectDetail.Code;
                    model.IsActive = subjectDetail.IsActive;
                    model.CreatedBy = userId;
                    model.CreatedOn = DateTime.UtcNow;
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(SubjectModel model)
        {
            string actionPermission = "";
            if (model.Id == 0)
            {
                actionPermission = AccessPermission.IsAdd;
            }
            else if (model.Id > 0)
            {
                actionPermission = AccessPermission.IsEdit;
            }

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.SUBJECT.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            int userId = SessionHelper.UserId;
            if (ModelState.IsValid)
            {
                SaveUpdateSubjects(model);
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }

        public SubjectModel SaveUpdateSubjects(SubjectModel model)
        {
            Subject obj = new Subject();
            if (model.Id > 0)
            {
                obj = _subjectService.GetSubjectById(model.Id);
            }
            int userId = SessionHelper.UserId;
            obj.Id = model.Id;
            obj.Name = model.Name;
            obj.IsActive = model.IsActive;
            obj.UpdatedBy = userId;
            obj.UpdatedOn = DateTime.UtcNow;
            if (obj.Id == 0)
            {
                obj.Code = model.Code;
                model.Id = _subjectService.CreateSubject(obj);
            }
            else
            {
                _subjectService.UpdateSubject(obj);
            }
            return model;
        }

        [HttpPost]
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request)
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.SUBJECT.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            var data = _subjectService.GetAllSubjectsGrid();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckDuplicateSubjectCode(string SubjectCode, int Id)
        {
            var getSubjectDetails = _subjectService.CheckDuplicateSubjectCode(SubjectCode);
            if (Id > 0)
            {
                getSubjectDetails = getSubjectDetails.Where(a => a.Id != Id).ToList();
            }
            if (getSubjectDetails.Count() > 0)
            {
                var message = _messageService.GetMessageByCode(Constants.MessageCode.CODEEXIST);
                return Json(message, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
    }
}