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
    public class QuestionTypeController : BaseController
    {
        // GET: Roles

        private readonly QuestionTypeService _questionTypeService;
        private readonly MessageService _messageService;
        public QuestionTypeController()
        {
            _questionTypeService = new QuestionTypeService();
            _messageService = new MessageService();
        }
        public ActionResult Index()
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTIONTYPE.ToString(), AccessPermission.IsView))
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

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTIONTYPE.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }

            int userId = SessionHelper.UserId;
            QuestionTypeModel model = new QuestionTypeModel();
            if (id.HasValue)
            {
                var questionTypeDetail = _questionTypeService.GetQuestionTypeById(id.Value);
                if (questionTypeDetail != null)
                {
                    model.Id = id.Value;
                    model.TypeName = questionTypeDetail.TypeName;
                    model.TypeCode = questionTypeDetail.TypeCode;
                    model.IsActive = questionTypeDetail.IsActive;
                    model.CreatedBy = userId;
                    model.CreatedOn = DateTime.UtcNow;
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(QuestionTypeModel model)
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

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTIONTYPE.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            int userId = SessionHelper.UserId;
            if (ModelState.IsValid)
            {
                SaveUpdateQuestionTypes(model);
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }

        public QuestionTypeModel SaveUpdateQuestionTypes(QuestionTypeModel model)
        {
            QuestionType obj = new QuestionType();
            if (model.Id > 0)
            {
                obj = _questionTypeService.GetQuestionTypeById(model.Id);
            }
            int userId = SessionHelper.UserId;
            obj.Id = model.Id;
            obj.TypeName = model.TypeName;
            obj.IsActive = model.IsActive;
            obj.UpdatedBy = userId;
            obj.UpdatedOn = DateTime.UtcNow;
            if (obj.Id == 0)
            {
                obj.TypeCode = model.TypeCode;
                model.Id = _questionTypeService.CreateQuestionType(obj);
            }
            else
            {
                _questionTypeService.UpdateQuestionType(obj);
            }
            return model;
        }

        [HttpPost]
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request)
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTIONTYPE.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            var data = _questionTypeService.GetAllQuestionTypesGrid();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckDuplicateQuestionTypeCode(string TypeCode, int Id)
        {
            var getQuestionTypeDetails = _questionTypeService.CheckDuplicateQuestionTypeCode(TypeCode);
            if (Id > 0)
            {
                getQuestionTypeDetails = getQuestionTypeDetails.Where(a => a.Id != Id).ToList();
            }
            if (getQuestionTypeDetails.Count() > 0)
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