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
using System.Web.Security;
using System.Web;
using System.Web.ApplicationServices;
using System.Web.Mvc;
using WebMatrix.WebData;

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
        private readonly UserProfileService _userProfileService;
        private readonly UserExamRecordService _userExamRecordService;



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
            _userProfileService = new UserProfileService();
            _userExamRecordService = new UserExamRecordService();
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
            var mappingsToBeUpdate = new List<PaperSetQuestionMapping>();
            var mappingsToBeCreate = new List<PaperSetQuestionMapping>();
            foreach (var qm in model.QuestionMappings)
            {
                var mapping = _allMappings.Where(q => q.QuestionId == qm.QuestionId).FirstOrDefault();
                if(mapping != null)
                {
                    mapping.CustomMarks = qm.CustomMarks;
                    mappingsToBeUpdate.Add(mapping);
                    _allMappings.Remove(mapping);
                }
                else
                {
                    
                    mappingsToBeCreate.Add(new PaperSetQuestionMapping
                    {
                        PaperSetId = model.Id,
                        QuestionId = qm.QuestionId,
                        CustomMarks = qm.CustomMarks
                    });
                }

            }
           
                _paperSetQuestionMappingService.RemoveQuestionsFromPaper(_allMappings);
            _paperSetQuestionMappingService.AddQuestionsInPaper(mappingsToBeCreate);
            _paperSetQuestionMappingService.UpdateMappings(mappingsToBeUpdate);
            return model;
        }

        [HttpPost]
        public JsonResult Clone(int paperSetId)
        {
            try
            {
                bool success = _paperSetService.ClonePaperSet(paperSetId);
                return Json(new { success = success }); 
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        public JsonResult CheckIfUsed(int id)
        {
            
            bool isUsed = _paperSetService.IsUsedForUserExam(id);

            return Json(isUsed, JsonRequestBehavior.AllowGet);
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
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request, string searchTerm)
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.PAPERSET.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            var data = _paperSetService.GetAllPaperSetsGrid();
            var materializedData = data.ToList().AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                materializedData = materializedData.Where(x =>
                    (x.PaperSetName != null && x.PaperSetName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (x.Status != null && x.Status.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    x.TotalMarks.ToString().Contains(searchTerm) ||
                    x.DurationInMinutes.ToString().Contains(searchTerm) 
                );
            }

            return Json(materializedData.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public ActionResult GetLeaderBoardGridData([DataSourceRequest] DataSourceRequest request, string paperSetName, string searchTerm)
        //{
        //    if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.LEADERBOARD.ToString(), AccessPermission.IsView))
        //    {
        //        return RedirectToAction("AccessDenied", "Base");
        //    }

        //    var data = _leaderBoardService.GetLeaderBoardGrid();

        //    if (!string.IsNullOrEmpty(paperSetName))
        //    {
        //        data = data.Where(x => x.PaperSetName == paperSetName);
        //    }
        //    var materializedData = data.ToList().AsQueryable();

        //    if (!string.IsNullOrEmpty(searchTerm))
        //    {
        //        materializedData = materializedData.Where(x =>
        //            (x.Email != null && x.Email.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
        //            (x.PaperSetName != null && x.PaperSetName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
        //            x.Score.ToString().Contains(searchTerm) ||
        //            (x.Date != null && x.Date.ToString("yyyy-MM-dd").IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
        //        );
        //    }
        //    return Json(materializedData.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //}



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
                                   IsActive = question.IsActive,
                                   options = _optionService.GetOptionsByQuestionId(id).Select(o => new OptionModel { Id = o.Id, QuestionId = o.QuestionId, OptionText = o.OptionText, IsCorrect = o.IsCorrect }).ToList()
                               }
                               ).ToList();
            

            return PartialView("_ViewPaperSet", model);
        }

        public ActionResult GetUserExamRecord(int paperSetId)
        {
            var model = new UserExamRecordViewModel { PaperSetId = paperSetId };
            return PartialView("_UserExamRecord", model);
        }

        [HttpPost]
        public JsonResult SubmitUserExam(UserExamRecordViewModel model)
        {
            if (model.UserEmails == null || !model.UserEmails.Any())
            {
                return Json(new { success = false, message = "At least one email is required." });
            }
            var userId = SessionHelper.UserId;
            List<object> examRecords = new List<object>();

            foreach (var email in model.UserEmails)
            {
                var existingUser = _userProfileService.GetUserByEmailId(email);

                if (existingUser == null)
                {
                    string defaultPassword = "CANDIDATE@123";
                    WebSecurity.CreateUserAndAccount(email, defaultPassword, propertyValues: new
                    {
                        Email = email,
                        IsActive = 1,
                        IsDeleted = 0,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = userId,
                        UpdatedOn = DateTime.UtcNow,
                        UpdatedBy = userId
                    });
                    Roles.AddUserToRole(email, "CANDIDATE");
                    existingUser = _userProfileService.GetUserByEmailId(email);
                }

                var existingRecord = _userExamRecordService.GetRecordByPaperSetIdAndUserId(model.PaperSetId, existingUser.UserId);
                if(existingRecord != null && existingRecord.ExpiryDate < DateTime.UtcNow)
                {
                    existingRecord.ExpiryDate = DateTime.UtcNow.AddDays(30);
                    existingRecord.UpdatedOn = DateTime.UtcNow;
                    existingRecord.UpdatedBy = userId;
                    _userExamRecordService.UpdateUserExamRecord(existingRecord);
                }
                else if(existingRecord == null){
                    string tokenData = $"{existingUser.UserId}|{model.PaperSetId}";
                    string encryptedToken = EncryptionHelper.Encrypt(tokenData);

                    var examRecord = new UserExamRecord
                    {
                        UserId = existingUser.UserId,
                        PaperSetId = model.PaperSetId,
                        Token = encryptedToken,
                        ExamStatus = Constants.ExamStatus.PENDING,
                        ExpiryDate = DateTime.UtcNow.AddDays(30),
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = userId,
                    };
                    _userExamRecordService.CreateUserExamRecord(examRecord);

                    examRecords.Add(new { email, token = encryptedToken });
                }
                
            }

            return Json(new { success = true, records = examRecords });
        }

    }
}