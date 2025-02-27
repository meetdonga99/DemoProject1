using DemoProject.Model;
using DemoProject.Models;
using DemoProject.Service;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ApplicationServices;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using Kendo.Mvc.Extensions;
using DemoProject.Helper;
using System.Xml.Linq;

namespace DemoProject.Controllers
{
    public class QuestionController : BaseController
    {

        private readonly QuestionService _questionService;
        private readonly SubjectService _subjectService;
        private readonly QuestionTypeService _questionTypeService;
        private readonly CommonLookupService _lookupService;
        private readonly OptionService _optionService;


        public QuestionController()
        {
            _questionService = new QuestionService();
            _questionTypeService = new QuestionTypeService();
            _subjectService = new SubjectService();
            _lookupService = new CommonLookupService();
            _optionService = new OptionService();
        }

        // GET: Question
        public ActionResult Index()
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTION.ToString(), AccessPermission.IsView))
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

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTION.ToString(), actionPermission))
                return RedirectToAction("AccessDenied", "Base");


            QuestionModel model = new QuestionModel();
            if (id > 0)
            {
                var getQuestion = _questionService.GetQuestionById(id.Value);
                if (getQuestion != null)
                {
                    model.Id = id.Value;
                    model.SubjectId = getQuestion.Subjects.Id;
                    model.QuestionTypeId = getQuestion.QuestionTypes.Id;
                    model.QuestionText = getQuestion.QuestionText;
                    model.DefaultMarks = getQuestion.DefaultMarks;
                    model.DifficultyLevel = getQuestion.DifficultyLevel;
                    model.Image = getQuestion.Image;
                    model.IsActive = getQuestion.IsActive;
                    model.options = _optionService.GetOptionsByQuestionId(id.Value).Select(o => new OptionModel { Id = o.Id, QuestionId = o.QuestionId,OptionText = o.OptionText, IsCorrect = o.IsCorrect }).ToList();
                }
            } 
            BindSubject(ref model);
            BindQuestionType(ref model);
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(QuestionModel model)
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

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTION.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            //if (!ModelState.IsValid)
            //{
            //    var errors = ModelState.Values.SelectMany(v => v.Errors);
            //    foreach (var error in errors)
            //    {
            //        System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
            //    }
            //}
            if (ModelState.IsValid)
            {
                SaveUpdateQuestion(model);
                return RedirectToAction("Index");
            }
            else
            {
                BindSubject(ref model);
                BindQuestionType(ref model);
                return View(model);
            }
        }



        public QuestionModel BindSubject(ref QuestionModel model)
        {
            var getsubjects = _subjectService.GetAllSubjects().OrderBy(a => a.Name);
            model._SubjectList.Add(new SelectListItem() { Text = "Select Subject", Value = "" });
            foreach (var item in getsubjects)
            {
                model._SubjectList.Add(new SelectListItem() { Text = item.Name.Trim(), Value = item.Id.ToString() });
            }
            return model;
        }

        public QuestionModel BindQuestionType(ref QuestionModel model)
        {
            var getQuestionTypes = _questionTypeService.GetAllQuestionTypes().OrderBy(a => a.TypeCode);
            model._QuestionTypeList.Add(new SelectListItem() { Text = "Select Question Type", Value = "" });
            foreach(var item in getQuestionTypes)
            {
                model._QuestionTypeList.Add(new SelectListItem() { Text = item.TypeName.Trim(), Value = item.Id.ToString() });
            }
            return model;
        }

        public JsonResult GetDiffiCultyLevelData([DataSourceRequest] DataSourceRequest request)
        {
            List<SelectListItem> _DifficultyLevel = new List<SelectListItem>();
            var data = _lookupService.GetLookupByType(LookupType.DifficultyLevel);
            if(data.Count > 0)
            {
                data = data.OrderBy(x => x.Name).ToList();
            }
            _DifficultyLevel.Add(new SelectListItem() { Text = "Select Difficulty Level", Value = "" });
            foreach(var item in data)
            {
                _DifficultyLevel.Add(new SelectListItem() { Text = item.Name, Value = item.Code.ToString() });
            }
            return Json(_DifficultyLevel, JsonRequestBehavior.AllowGet);
        }

        public QuestionModel SaveUpdateQuestion(QuestionModel model)
        {
            int userId = SessionHelper.UserId;
            Question obj = new Question();
            if(model.Id > 0)
            {
                obj = _questionService.GetQuestionById(model.Id);
            }
            obj.Id = model.Id;
            obj.SubjectId = model.SubjectId;
            obj.QuestionTypeId = model.QuestionTypeId;
            obj.QuestionText = model.QuestionText;
            obj.DefaultMarks = model.DefaultMarks;
            obj.DifficultyLevel = model.DifficultyLevel;
            obj.Image = model.Image;
            obj.IsActive = model.IsActive;
            if (obj.Id == 0)
            {
                obj.CreatedBy = SessionHelper.UserId;
                obj.CreatedOn = DateTime.UtcNow;
                model.Id = _questionService.CreateQuestion(obj);
              }
            else
            {
                obj.UpdatedBy = SessionHelper.UserId;
                obj.UpdatedOn = DateTime.UtcNow;
                _questionService.UpdateQuestion(obj);
            }

            if (model.options != null && model.options.Any())
            {
                var existingOptions = _optionService.GetOptionsByQuestionId(obj.Id).ToList();
                foreach (var option in model.options)
                {
                    if (option.Id == 0)
                    {
                        // Add new option
                        Option newOption = new Option
                        {
                            QuestionId = obj.Id,
                            OptionText = option.OptionText,
                            IsCorrect = option.IsCorrect,
                            CreatedBy = userId,
                            CreatedOn = DateTime.Now,
                        };
                        _optionService.CreateOption(newOption);
                    }
                    else
                    {
                        // Update existing option
                        Option existingOption = existingOptions.FirstOrDefault(o => o.Id == option.Id);
                        if (existingOption != null)
                        {
                            existingOption.OptionText = option.OptionText;
                            existingOption.IsCorrect = option.IsCorrect;
                            existingOption.UpdatedBy = userId;
                            existingOption.UpdatedOn = DateTime.Now;
                            _optionService.UpdateOption(existingOption);
                        }
                    }
                }
            }

            return model;
        }

        [HttpPost]
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request)
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTION.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            var data = _questionService.GetAllQuestionsGrid();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}