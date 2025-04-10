using DemoProject.Model;
using DemoProject.Service;
using Kendo.Mvc.UI;
using DemoProject.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using DemoProject.Models;


namespace DemoProject.Controllers
{
    public class UserExamRecordController : BaseController
    {
        private readonly UserExamRecordService _userExamRecordService;
        private readonly PaperSetService _paperSetService;
        private readonly QuestionService _questionService;
        private readonly PaperSetQuestionMappingService _paperSetQuestionMappingService;
        private readonly UserProfileService _userProfileService;
        private readonly OptionService _optionService;
        private readonly UserExamAnswerService _userExamAnswerService;

        public UserExamRecordController()
        {
            _userExamRecordService = new UserExamRecordService();
            _paperSetService = new PaperSetService();
            _questionService = new QuestionService();
            _paperSetQuestionMappingService = new PaperSetQuestionMappingService();
            _userProfileService = new UserProfileService();
            _optionService = new OptionService();
            _userExamAnswerService = new UserExamAnswerService();
        }

        // GET: UserExamRecord
        public ActionResult Index()
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.USEREXAMRECORDLIST.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            return View();
        }

        [HttpPost]
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request, string searchTerm)
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.USEREXAMRECORDLIST.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            var data = _userExamRecordService.GetAllUserExamRecordGrid();

            var materializedData = data.ToList().AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                materializedData = materializedData.Where(x =>
                    (x.Token != null && x.Token.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (x.PaperSetName != null && x.PaperSetName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (x.UserEmail != null && x.UserEmail.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (x.ExamStatus != null && x.ExamStatus.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    x.Score.ToString().Contains(searchTerm) ||
                    (x.ExpiryDate != null && x.ExpiryDate.ToString("yyyy-MM-dd").IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                );
            }

            return Json(materializedData.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        public ActionResult GenerateLinkByToken(string token)
        {
            var link = Url.Action("ViewByToken", "UserExamRecord", new { token = token }, protocol: Request.Url.Scheme);
            return Json(new { link = link }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult SendLinkByEmail(string token)
        {
            var link = Url.Action("ViewByToken", "UserExamRecord", new { token = token }, protocol: Request.Url.Scheme);
            var record = _userExamRecordService.GetRecordByToken(token);
            var userEmail = record.User.Email;

            if (!string.IsNullOrEmpty(userEmail))
            {
                bool emailSent = EmailService.Send(userEmail, "Your Exam Link", $"Here is your exam link: <a href='{link}'>{link}</a>");

                if (emailSent)
                    return Json(new { success = true });
                else
                    return Json(new { success = false, message = "Email sending failed." });
            }
            return Json(new { success = false, message = "User email not found." });
        }


        [AllowAnonymous]
        public ActionResult ViewByToken(string token)
        {
            var record = _userExamRecordService.GetRecordByToken(token);
            if (record == null || (record.ExpiryDate.HasValue && record.ExpiryDate < DateTime.UtcNow))
            {
                return View("InvalidLink");
            }
            if(record.ExamStatus == "COMPLETED")
            {
                return RedirectToAction("SubmissionSuccess", new { examId = record.Id });
            }

            var getPaperSet = _paperSetService.GetPaperSetById(record.PaperSetId);
            var getUser = _userProfileService.GetUserById(record.UserId);
            var mappings = _paperSetQuestionMappingService.GetMappingsByPaperSetId(record.PaperSetId);
            var questions = _questionService.GetAllQuestions();

            UserExamViewModel model = new UserExamViewModel();
            model.UserExamRecordId = record.Id;
            model.ExamStatus = record.ExamStatus;
            model.StartTime = record.StartTime;
            model.EndTime = record.EndTime;


            model.UserId = record.UserId;
            model.UserName = getUser.UserName;
            model.Name = getUser.Name;
            model.Email = getUser.Email;

            model.PaperSetId = record.PaperSetId;
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
            model.Answers = _userExamAnswerService.GetAnswersByExamId(model.UserExamRecordId).Select(a => new SaveAnswerModel
            {
                UserExamRecordId = a.UserExamRecordId,
                QuestionId = a.QuestionId,
                SelectedOptions = a.SelectedOptions.Split(',').Where(s => !string.IsNullOrEmpty(s)).Select(int.Parse).ToList(),
                DescriptiveAnswer = a.DescriptiveAnswer,
            }).ToList();
            return View("ExamView", model);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult GetRemainingMinutes(int userExamRecordId)
        {
            var record = _userExamRecordService.GetRecordByUserExamRecordId(userExamRecordId);
            TimeSpan diff = DateTime.UtcNow.Subtract(record.StartTime.Value);
            var usedMinutes = diff.TotalMinutes;

            
            return Json(new { usedMinutes = usedMinutes });
        }


        [AllowAnonymous]
        [HttpPost]
        public JsonResult StartExam(UserExamViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Invalid data received." });
            }

            try
            {
                var user = _userProfileService.GetUserById(model.UserId);
                if (user != null)
                {
                    user.Name = model.Name;
                    user.UserName = model.UserName;
                    _userProfileService.UpdateUserProfile(user);
                }

                var existingRecord = _userExamRecordService.GetRecordByPaperSetIdAndUserId(model.PaperSetId, model.UserId);
                if (existingRecord != null)
                {
                    existingRecord.StartTime = DateTime.UtcNow;
                    existingRecord.EndTime = DateTime.UtcNow.AddMinutes(model.DurationInMinutes);
                    existingRecord.ExamStatus = Constants.ExamStatus.INPROGRESS;
                    _userExamRecordService.UpdateUserExamRecord(existingRecord);
                }


                return Json(new { success = true, message = "Exam started successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [AllowAnonymous]
        [HttpPost]
        public JsonResult SaveAnswer(UserExamViewModel model)
        {
            try
            {
                foreach (var answer in model.Answers)
                {
                    var existingAnswer = _userExamAnswerService.GetAnswerByExamIdAndQuestionId(model.UserExamRecordId, answer.QuestionId);
                    if (existingAnswer == null)
                    {
                        var newAnswer = new UserExamAnswer
                        {
                            UserExamRecordId = model.UserExamRecordId,
                            QuestionId = answer.QuestionId,
                            SelectedOptions = string.Join(",", answer.SelectedOptions),
                            DescriptiveAnswer = answer.DescriptiveAnswer,
                            ObtainedMarks = 0
                        };
                        _userExamAnswerService.CreateUserExamAnswer(newAnswer);
                    }
                    else
                    {
                        existingAnswer.SelectedOptions = string.Join(",", answer.SelectedOptions);
                        existingAnswer.DescriptiveAnswer = answer.DescriptiveAnswer;
                        _userExamAnswerService.UpdateUserExamAnswer(existingAnswer);
                    }
                }

                model.Answers = _userExamAnswerService.GetAnswersByExamId(model.UserExamRecordId).Select(a => new SaveAnswerModel
                {
                    UserExamRecordId = a.UserExamRecordId,
                    QuestionId = a.QuestionId,
                    SelectedOptions = a.SelectedOptions.Split(',').Where(s => !string.IsNullOrEmpty(s)).Select(int.Parse).ToList(),
                    DescriptiveAnswer = a.DescriptiveAnswer,
                }).ToList();

                return Json(new { success = true, updatedModel = model });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult FinishExam(UserExamViewModel model)
        {
            if (model == null || model.Answers == null || !model.Answers.Any())
            {
                return Json(new { success = false, message = "No answers submitted." });
            }

            try
            {
                foreach (var answer in model.Answers)
                {
                    var existingAnswer = _userExamAnswerService.GetAnswerByExamIdAndQuestionId(model.UserExamRecordId, answer.QuestionId);
                    if (existingAnswer == null)
                    {
                        var newAnswer = new UserExamAnswer
                        {
                            UserExamRecordId = model.UserExamRecordId,
                            QuestionId = answer.QuestionId,
                            SelectedOptions = string.Join(",", answer.SelectedOptions),
                            DescriptiveAnswer = answer.DescriptiveAnswer,
                            ObtainedMarks = 0
                        };
                        _userExamAnswerService.CreateUserExamAnswer(newAnswer);
                    }
                }


                var record = _userExamRecordService.GetRecordByPaperSetIdAndUserId(model.PaperSetId, model.UserId);
                if (record == null)
                {
                    return Json(new { success = false, message = "Exam record not found." });
                }
                record.EndTime = DateTime.UtcNow;
                record.ExamStatus = Constants.ExamStatus.COMPLETED; 
                _userExamRecordService.UpdateUserExamRecord(record);

                return Json(new { success = true, redirectUrl = Url.Action("SubmissionSuccess", "UserExamRecord", new { examId = record.Id }) });
                //return Json(new { success = true, message = "Exam finished successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error finishing exam: " + ex.Message });
            }

        }

        [AllowAnonymous]
        public ActionResult SubmissionSuccess(int examId)
        {
            ViewBag.ExamId = examId;
            return View();
        }

        public ActionResult CompletedExamGrid()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetCompletedExamGridData([DataSourceRequest] DataSourceRequest request, string searchTerm)
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.COMPLETEDUSEREXAM.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            var data = _userExamRecordService.GetAllUserExamRecordGrid().Where(a => (a.ExamStatus != "PENDING" && a.ExamStatus != "INPROGRESS"));

            var materializedData = data.ToList().AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                materializedData = materializedData.Where(x =>
                    (x.PaperSetName != null && x.PaperSetName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (x.UserEmail != null && x.UserEmail.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (x.ExamStatus != null && x.ExamStatus.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    x.Score.ToString().Contains(searchTerm) ||
                    (x.ExpiryDate != null && x.ExpiryDate.ToString("yyyy-MM-dd").IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                );
            }

            return Json(materializedData.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult EvaluateTestPaper(string token)
        {
            var record = _userExamRecordService.GetRecordByToken(token);
            if (record == null || (record.ExpiryDate.HasValue && record.ExpiryDate < DateTime.UtcNow))
            {
                return View("InvalidLink");
            }

            var getPaperSet = _paperSetService.GetPaperSetById(record.PaperSetId);
            var getUser = _userProfileService.GetUserById(record.UserId);
            var mappings = _paperSetQuestionMappingService.GetMappingsByPaperSetId(record.PaperSetId);
            var questions = _questionService.GetAllQuestions();

            UserExamViewModel model = new UserExamViewModel();
            model.UserExamRecordId = record.Id;
            model.ExamStatus = record.ExamStatus;
            model.StartTime = record.StartTime;
            model.EndTime = record.EndTime;
            model.Score = record.Score;


            model.UserId = record.UserId;
            model.UserName = getUser.UserName;
            model.Name = getUser.Name;
            model.Email = getUser.Email;

            model.PaperSetId = record.PaperSetId;
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

            var data = from i in model.Questions
                       select new CorrectAnswer()
                       {
                           QuestionId = i.Id,
                           CorrectOptions = i.options.Where(o => o.IsCorrect).Select(o => o.Id).ToList()

                       };

            model.Answers = _userExamAnswerService.GetAnswersByExamId(model.UserExamRecordId).Select(a => new SaveAnswerModel
            {
                UserExamRecordId = a.UserExamRecordId,
                QuestionId = a.QuestionId,
                SelectedOptions = a.SelectedOptions.Split(',').Where(s => !string.IsNullOrEmpty(s)).Select(int.Parse).ToList(),
                DescriptiveAnswer = a.DescriptiveAnswer,
                ObtainedMarks = (int)(a.ObtainedMarks != 0 ? a.ObtainedMarks :
    (from i in data
     where i.QuestionId == a.QuestionId
     select i.CorrectOptions)
    .FirstOrDefault() 
    .OrderBy(x => x) 
    .SequenceEqual(
        a.SelectedOptions.Split(',')
        .Where(s => !string.IsNullOrEmpty(s))
        .Select(int.Parse) 
        .OrderBy(x => x) 
    )
    ? model.Questions.Where(o => o.Id == a.QuestionId && (o.QuestionTypeId == 1 || o.QuestionTypeId == 2))
                     .Select(o => o.DefaultMarks)
                     .FirstOrDefault()
    : 0) ,

                IsEvaluated =  a.IsEvaluated == false? (model.Questions.Where(o => o.Id == a.QuestionId).Select(o => o.QuestionTypeId == 1 || o.QuestionTypeId == 2).FirstOrDefault() ? true : false) : a.IsEvaluated,

            }).ToList();

            
            TempData["CorrectAnswers"] = data;

            return View("EvaluateTestPaper", model);
        }

        [HttpPost]
        public JsonResult SubmitEvaluation(UserExamViewModel model)
        {
            try
            {
                var record = _userExamRecordService.GetRecordByPaperSetIdAndUserId(model.PaperSetId, model.UserId);
                if (record == null)
                {
                    return Json(new { success = false, message = "Exam record not found." });
                }
                record.ExamStatus = Constants.ExamStatus.RESULT_PUBLISHED;
                record.Score = model.Score;
                record.Percentage = (decimal)(model.Score * 100) / (decimal)model.TotalMarks;
                _userExamRecordService.UpdateUserExamRecord(record);

                foreach (var answer in model.Answers)
                {
                    var existingAnswer = _userExamAnswerService.GetAnswerByExamIdAndQuestionId(model.UserExamRecordId, answer.QuestionId);
                    if(existingAnswer != null)
                    {
                        existingAnswer.ObtainedMarks = answer.ObtainedMarks;
                        existingAnswer.IsEvaluated = answer.IsEvaluated;
                        _userExamAnswerService.UpdateUserExamAnswer(existingAnswer);
                    }
                }

                    return Json(new
                    {
                        success = true,
                        message = "Evaluation submitted successfully",
                        redirectUrl = Url.Action("CompletedExamGrid", "UserExamRecord") 
                    });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while processing your request" });
            }
        }

        }
}