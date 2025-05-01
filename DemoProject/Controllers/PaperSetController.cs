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
using DocumentFormat.OpenXml.Bibliography;
using System.Threading.Tasks;

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

        public JsonResult GetSubjects()
        {
            var subjects = _subjectService.GetAllSubjects()
                .Where(s => s.IsActive)
                .Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                })
                .ToList();

            return Json(subjects, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDifficultyLevels()
        {
            var difficultyLevels = _lookupService.GetLookupByType(LookupType.PaperSetDifficulty)
                .Select(l => new SelectListItem
                {
                    Text = l.Name,
                    Value = l.Code.ToString()
                })
                .ToList();

            return Json(difficultyLevels, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AutoGeneratePaperSet(int[] subjectId, string difficultyLevel, int totalMarks, int durationInMinutes)
        {
            try
            {
                if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.PAPERSET.ToString(), AccessPermission.IsAdd))
                {
                    return Json(new { success = false, message = "Access denied." });
                }

                if (subjectId == null || subjectId.Length == 0 || string.IsNullOrEmpty(difficultyLevel) || totalMarks <= 0 || durationInMinutes <= 0)
                {
                    return Json(new { success = false, message = "Invalid input parameters." });
                }

                var weightageConfig = CommonUtility.GetConfigurationValueByKey(ConfigurationKeys.Keys.MARKWEIGHTAGE);

                if (weightageConfig == null)
                {
                    return Json(new { success = false, message = "Difficulty weightage configuration not found." });
                }

                var weightageObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(weightageConfig);

                if (weightageObject == null || !weightageObject.ContainsKey(difficultyLevel))
                {
                    return Json(new { success = false, message = "Invalid difficulty level or configuration." });
                }

                var weightages = weightageObject[difficultyLevel];

                var subjects = subjectId.Select(id => _subjectService.GetSubjectById(id)).Where(s => s != null).ToList();
                if (subjects.Count == 0)
                {
                    return Json(new { success = false, message = "Invalid subjects." });
                }

                string subjectNames = string.Join("_", subjects.Select(s => s.Name));
                string timestamp = DateTime.Now.ToString("yyyy/MM/dd_HH:mm:ss");
                string paperSetName = $"{subjectNames}_{difficultyLevel}_{timestamp}";
                
                var allSubjectQuestions = _questionService.GetAllQuestions()
                    .Where(q => q.IsActive && !q.IsDeleted && subjectId.Contains(q.SubjectId))
                    .ToList();

                if (!allSubjectQuestions.Any())
                {
                    return Json(new { success = false, message = "No questions available for the selected subjects." });
                }

                
                var marksPerDifficulty = new Dictionary<string, int>();
                foreach (var kv in weightages)
                {
                    marksPerDifficulty[kv.Key] = (totalMarks * kv.Value) / 100;
                }

                var paperSet = new PaperSet
                {
                    PaperSetName = paperSetName,
                    TotalMarks = totalMarks,
                    DurationInMinutes = durationInMinutes,
                    IsActive = true,
                    CreatedBy = SessionHelper.UserId,
                    CreatedOn = DateTime.UtcNow
                };

                var selectedQuestions = new List<Question>();
                int currentMarks = 0;

               
                var questionsByDifficulty = allSubjectQuestions.GroupBy(q => q.DifficultyLevel)
                    .ToDictionary(g => g.Key, g => g.ToList());

             
                foreach (var kv in marksPerDifficulty)
                {
                    string diffLevel = kv.Key;
                    int targetMarks = kv.Value;
                    int difficultyMarks = 0;

                    if (!questionsByDifficulty.ContainsKey(diffLevel))
                        continue;

                    var difficultyQuestions = new List<Question>(questionsByDifficulty[diffLevel]);

                   
                    foreach (var question in difficultyQuestions.OrderBy(q => q.DefaultMarks).ToList())
                    {
                        if (difficultyMarks + question.DefaultMarks <= targetMarks)
                        {
                            selectedQuestions.Add(question);
                            difficultyMarks += question.DefaultMarks;
                            currentMarks += question.DefaultMarks;
                            difficultyQuestions.Remove(question);

                            if (difficultyMarks == targetMarks)
                                break;
                        }
                    }

                    if (difficultyMarks < targetMarks && difficultyQuestions.Any())
                    {
                        var bestQuestion = difficultyQuestions
                            .Where(q => difficultyMarks + q.DefaultMarks <= targetMarks)
                            .OrderByDescending(q => q.DefaultMarks)
                            .FirstOrDefault();

                        if (bestQuestion != null)
                        {
                            selectedQuestions.Add(bestQuestion);
                            difficultyMarks += bestQuestion.DefaultMarks;
                            currentMarks += bestQuestion.DefaultMarks;
                        }
                    }
                }

            
                if (selectedQuestions.Count == 0)
                {
                    return Json(new { success = false, message = "Could not find suitable questions to meet the marks criteria." });
                }

                
                paperSet.Status = currentMarks == totalMarks ? "COMPLETED" : "DRAFT";

               
                int paperSetId = _paperSetService.CreatePaperSet(paperSet);

                
                var mappings = selectedQuestions.Select(q => new PaperSetQuestionMapping
                {
                    PaperSetId = paperSetId,
                    QuestionId = q.Id,
                    CustomMarks = q.DefaultMarks
                }).ToList();

            
                _paperSetQuestionMappingService.AddQuestionsInPaper(mappings);

                return Json(new
                {
                    success = true,
                    message = $"Paper set generated with {selectedQuestions.Count} questions totaling {currentMarks} marks." +
                             (currentMarks < totalMarks ? $" (Requested: {totalMarks} marks) Set is in DRAFT status." : "")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
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
            // Get paper set and mappings in parallel to reduce wait time
            var paperSetTask = Task.Run(() => _paperSetService.GetPaperSetById(id));
            var mappingsTask = Task.Run(() => _paperSetQuestionMappingService.GetMappingsByPaperSetId(id));

            // Wait for both to complete
            Task.WaitAll(paperSetTask, mappingsTask);

            var paperSet = paperSetTask.Result;
            var mappings = mappingsTask.Result;

            // Get only the questions we need by their IDs instead of loading all questions
            var questionIds = mappings.Select(m => m.QuestionId).ToList();
            var questions = _questionService.GetQuestionsByIds(questionIds);

            // Create a dictionary for faster lookups
            var questionLookup = questions.ToDictionary(q => q.Id);
            var mappingLookup = mappings.ToDictionary(m => m.QuestionId);

            // Pre-fetch all options for these questions in a single query
            var allOptions = _optionService.GetOptionsByQuestionIds(questionIds);

            // Group options by question ID for efficient lookup
            var optionsByQuestion = allOptions.GroupBy(o => o.QuestionId)
                                              .ToDictionary(g => g.Key, g => g.ToList());

            // Create the model
            ViewPaperSetModel model = new ViewPaperSetModel
            {
                Id = id,
                PaperSetName = paperSet.PaperSetName,
                TotalMarks = paperSet.TotalMarks,
                DurationInMinutes = paperSet.DurationInMinutes,
                Questions = new List<QuestionModel>()
            };

            // Build the questions list without repeated queries
            foreach (var questionId in questionIds)
            {
                if (questionLookup.TryGetValue(questionId, out var question) &&
                    mappingLookup.TryGetValue(questionId, out var mapping))
                {
                    var questionModel = new QuestionModel
                    {
                        Id = question.Id,
                        SubjectId = question.Subjects.Id,
                        QuestionTypeId = question.QuestionTypes.Id,
                        QuestionText = question.QuestionText,
                        DefaultMarks = mapping.CustomMarks,
                        DifficultyLevel = question.DifficultyLevel,
                        IsActive = question.IsActive,
                        options = new List<OptionModel>()
                    };

                    // Add options if they exist for this question
                    if (optionsByQuestion.TryGetValue(questionId, out var options))
                    {
                        questionModel.options = options.Select(o => new OptionModel
                        {
                            Id = o.Id,
                            QuestionId = o.QuestionId,
                            OptionText = o.OptionText,
                            IsCorrect = o.IsCorrect
                        }).ToList();
                    }

                    model.Questions.Add(questionModel);
                }
            }

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