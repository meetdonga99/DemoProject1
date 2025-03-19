using DemoProject.Helper;
using DemoProject.Model;
using DemoProject.Models;
using DemoProject.Service;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.ApplicationServices;
using System.Web.Mvc;

namespace DemoProject.Controllers
{
    public class PaperSetController : BaseController
    {

        private readonly PaperSetService _paperSetService;
        private readonly PaperSetQuestionMappingService _paperSetQuestionMappingService;
        private readonly QuestionService _questionService;
        private readonly SubjectService _subjectService;
        private readonly QuestionTypeService _questionTypeService;
        private readonly CommonLookupService _lookupService;
        private readonly OptionService _optionService;
        private readonly PaperSetLinkService _paperSetLinkService;



        public PaperSetController()
        {
            _paperSetService = new PaperSetService();
            _paperSetQuestionMappingService = new PaperSetQuestionMappingService();
            _questionService = new QuestionService();
            _subjectService = new SubjectService();
            _questionTypeService = new QuestionTypeService();
            _lookupService = new CommonLookupService();
            _optionService = new OptionService();
            _paperSetLinkService = new PaperSetLinkService();
        }
        public ActionResult Index()
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.PAPERSET.ToString(), AccessPermission.IsView))
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

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.PAPERSET.ToString(), actionPermission))
                return RedirectToAction("AccessDenied", "Base");


            PaperSetModel model = new PaperSetModel();

            if (id > 0)
            {
                var currentMarks = _paperSetQuestionMappingService.GetMappingsByPaperSetId(id.Value).Select(a => a.CustomMarks).Sum();
                var getPaperSet = _paperSetService.GetPaperSetById(id.Value);
                if (getPaperSet != null)
                {
                    model.Id = id.Value;
                    model.PaperSetName = getPaperSet.PaperSetName;
                    model.TotalMarks = getPaperSet.TotalMarks;
                    model.DurationInMinutes = getPaperSet.DurationInMinutes;
                    model.Status = getPaperSet.Status;
                    model.IsActive = getPaperSet.IsActive;
                    model.CurrentMarks = currentMarks;
                }
            }
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(PaperSetModel model)
        {
            string questionMappingsJson = Request.Form["QuestionMappings"];
            int startIndex = questionMappingsJson.IndexOf("[",34);
            
             questionMappingsJson = questionMappingsJson.Substring(startIndex);

            if (!string.IsNullOrEmpty(questionMappingsJson))
            {
                model.QuestionMappings = Newtonsoft.Json.JsonConvert.DeserializeObject<List<QuestionMappingItem>>(questionMappingsJson);
            }

            string actionPermission = "";
            if (model.Id == 0)
            {
                actionPermission = AccessPermission.IsAdd;
            }
            else if (model.Id > 0)
            {
                actionPermission = AccessPermission.IsEdit;
            }

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.PAPERSET.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }

            //SaveUpdatePaperSets(model);
            //return RedirectToAction("Index");
            ModelState.Remove(nameof(model.QuestionMappings));
            if (ModelState.IsValid)
            {
                SaveUpdatePaperSets(model);
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }


        public PaperSetModel SaveUpdatePaperSets(PaperSetModel model)
        {
            PaperSet obj = new PaperSet();
            int userId = SessionHelper.UserId;
            if (model.Id > 0)
            {
                obj = _paperSetService.GetPaperSetById(model.Id);
            }
            
            obj.Id = model.Id;
            obj.PaperSetName = model.PaperSetName;
            obj.TotalMarks = model.TotalMarks;
            obj.DurationInMinutes = model.DurationInMinutes;
            obj.Status = model.Status;
            obj.IsActive = model.IsActive;
            if (obj.Id == 0)
            {
                obj.CreatedBy = SessionHelper.UserId;
                obj.CreatedOn = DateTime.UtcNow;
                model.Id = _paperSetService.CreatePaperSet(obj);
            }
            else
            {
                obj.UpdatedBy = SessionHelper.UserId;
                obj.UpdatedOn = DateTime.UtcNow;
                _paperSetService.UpdatePaperSet(obj);
            }
            var _allMappings = _paperSetQuestionMappingService.GetMappingsByPaperSetId(model.Id);
            foreach (var qm in model.QuestionMappings)
            {
                var mapping = _paperSetQuestionMappingService.GetMappingsByPaperSetId(model.Id).Where(q => q.QuestionId == qm.QuestionId).FirstOrDefault();
                if(mapping != null)
                {
                    mapping.CustomMarks = qm.CustomMarks;
                    _paperSetQuestionMappingService.UpdateMapping(mapping);
                    _allMappings.Remove(mapping);
                }
                else
                {
                    _paperSetQuestionMappingService.AddQuestionInPaper(new PaperSetQuestionMapping
                    {
                        PaperSetId = model.Id,
                        QuestionId = qm.QuestionId,
                        CustomMarks = qm.CustomMarks
                    });
                }
                foreach(var rm in _allMappings)
                {
                    _paperSetQuestionMappingService.RemoveQuestionFromPaper(rm.Id);
                }
            }
            return model;
        }

        public JsonResult GetPaperSetStatusData([DataSourceRequest] DataSourceRequest request)
        {
            List<SelectListItem> _Status = new List<SelectListItem>();
            var data = _lookupService.GetLookupByType(LookupType.PaperSetStatus);
            if (data.Count > 0)
            {
                data = data.OrderBy(x => x.Name).ToList();
            }
            _Status.Add(new SelectListItem() { Text = "Select Status", Value = "" });
            foreach (var item in data)
            {
                _Status.Add(new SelectListItem() { Text = item.Name, Value = item.Code.ToString() });
            }
            return Json(_Status, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request)
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.PAPERSET.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            var data = _paperSetService.GetAllPaperSetsGrid();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetUnselectedQuestionsGridData([DataSourceRequest] DataSourceRequest request, int paperSetId)
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.PAPERSET.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }

            Dictionary<int,int> mappings;
            if (paperSetId != 0)
            {
                 mappings = _paperSetQuestionMappingService.GetMappingsByPaperSetId(paperSetId).ToDictionary(q => q.QuestionId, q => q.CustomMarks);
            }
            else
            {
                 mappings = new Dictionary<int, int>();
            }
                var allQuestions = _questionService.GetAllQuestions()
            .Where(q => q.IsActive && !q.IsDeleted && !mappings.ContainsKey(q.Id))
            .Select(q => new QuestionViewModel
            {
                QuestionId = q.Id,
                SubjectName = _subjectService.GetSubjectById(q.SubjectId).Name,
                QuestionType = _questionTypeService.GetQuestionTypeById(q.QuestionTypeId).TypeName,
                QuestionText = q.QuestionText,
                DefaultMarks = mappings.ContainsKey(q.Id) && mappings[q.Id] != 0
                       ? mappings[q.Id]
                       : q.DefaultMarks,
                DifficultyLevel = q.DifficultyLevel,
                Image = q.Image,
                IsSelected = mappings.ContainsKey(q.Id)
            }).ToList().AsQueryable();


            return Json(allQuestions.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetSelectedQuestionsGridData([DataSourceRequest] DataSourceRequest request, int paperSetId)
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.PAPERSET.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            Dictionary<int, int> mappings;
            if (paperSetId != 0)
            {
                mappings = _paperSetQuestionMappingService.GetMappingsByPaperSetId(paperSetId).ToDictionary(q => q.QuestionId, q => q.CustomMarks);
            }
            else
            {
                mappings = new Dictionary<int, int>();
            }
            var allQuestions = _questionService.GetAllQuestions()
        .Where(q => q.IsActive && !q.IsDeleted && mappings.ContainsKey(q.Id))
        .Select(q => new QuestionViewModel
        {
            QuestionId = q.Id,
            SubjectName = _subjectService.GetSubjectById(q.SubjectId).Name,
            QuestionType = _questionTypeService.GetQuestionTypeById(q.QuestionTypeId).TypeName,
            QuestionText = q.QuestionText,
            DefaultMarks = mappings.ContainsKey(q.Id) && mappings[q.Id] != 0
                   ? mappings[q.Id]
                   : q.DefaultMarks,
            DifficultyLevel = q.DifficultyLevel,
            Image = q.Image,
            IsSelected = mappings.ContainsKey(q.Id)
        }).ToList().AsQueryable();

             
            return Json(allQuestions.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewPaperSet(int id)
        {
            var getPaperSet = _paperSetService.GetPaperSetById(id);
            var mappings = _paperSetQuestionMappingService.GetMappingsByPaperSetId(id);
            var questions = _questionService.GetAllQuestions();
            ViewPaperSetModel model = new ViewPaperSetModel();
            model.Id = id;
            model.PaperSetName = getPaperSet.PaperSetName;
            model.TotalMarks = getPaperSet.TotalMarks;
            model.DurationInMinutes = getPaperSet.DurationInMinutes;
            model.Questions = (from mapping in mappings
                               join question in questions
                               on mapping.QuestionId equals question.Id
                               select new QuestionModel()
                               {
                                   Id = question.Id,
                                   SubjectId = question.Subjects.Id,
                                   QuestionTypeId = question.QuestionTypes.Id,
                                   QuestionText = question.QuestionText,
                                   DefaultMarks = mapping.CustomMarks,
                                   DifficultyLevel = question.DifficultyLevel,
                                   Image = question.Image,
                                   IsActive = question.IsActive,
                                   options = _optionService.GetOptionsByQuestionId(id).Select(o => new OptionModel { Id = o.Id, QuestionId = o.QuestionId, OptionText = o.OptionText, IsCorrect = o.IsCorrect }).ToList()
                               }
                               ).ToList();
            

            return PartialView("_ViewPaperSet", model);
        }


        public ActionResult GenerateOrGetLink(int paperSetId)
        {
            var userId = SessionHelper.UserId;
            var existingLink = _paperSetLinkService.GetPaperSetLinkByPaperSetId(paperSetId);
            var currentTime = DateTime.UtcNow;
            if (existingLink == null || (existingLink.ExpiryDate.HasValue && existingLink.ExpiryDate <= currentTime))
            {
                var newToken = Guid.NewGuid().ToString();

                if (existingLink == null)
                {
                    existingLink = new PaperSetLink
                    {
                        PaperSetId = paperSetId,
                        Token = newToken,
                        IsActive = true,
                        ExpiryDate = currentTime.AddDays(30),
                        CreatedAt = currentTime,
                        CreatedBy = userId
                    };

                    _paperSetLinkService.CreatePaperSetLink(existingLink);
                }
                else
                {
                    existingLink.Token = newToken;
                    existingLink.ExpiryDate = currentTime.AddDays(30);
                    existingLink.IsActive = true;
                    existingLink.UpdatedAt = currentTime;
                    existingLink.UpdatedBy = userId;

                    _paperSetLinkService.UpdatePaperSetLink(existingLink);
                }

            }
            var linkUrl = Url.Action("ViewByToken", "PaperSet", new { token = existingLink.Token }, Request.Url.Scheme);
            return Json(new { success = true, link = linkUrl }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult ViewByToken(string token)
        {
            var link = _paperSetLinkService.GetPaperSetLinkByToken(token);
            if (link == null || (link.ExpiryDate.HasValue && link.ExpiryDate < DateTime.UtcNow))
            {
                return View("InvalidLink");
            }

            var getPaperSet = _paperSetService.GetPaperSetById(link.PaperSetId);
            var mappings = _paperSetQuestionMappingService.GetMappingsByPaperSetId(link.PaperSetId);
            var questions = _questionService.GetAllQuestions();
            ViewPaperSetModel model = new ViewPaperSetModel();
            model.Id = link.PaperSetId;
            model.PaperSetName = getPaperSet.PaperSetName;
            model.TotalMarks = getPaperSet.TotalMarks;
            model.DurationInMinutes = getPaperSet.DurationInMinutes;
            model.Questions = (from mapping in mappings
                               join question in questions
                               on mapping.QuestionId equals question.Id
                               select new QuestionModel()
                               {
                                   Id = question.Id,
                                   SubjectId = question.Subjects.Id,
                                   QuestionTypeId = question.QuestionTypes.Id,
                                   QuestionText = question.QuestionText,
                                   DefaultMarks = mapping.CustomMarks,
                                   DifficultyLevel = question.DifficultyLevel,
                                   Image = question.Image,
                                   IsActive = question.IsActive,
                                   options = _optionService.GetOptionsByQuestionId(question.Id).Select(o => new OptionModel { Id = o.Id, QuestionId = o.QuestionId, OptionText = o.OptionText, IsCorrect = o.IsCorrect }).ToList()
                               }
                               ).ToList();

            return View("ExamView",model);
        }

    }
}