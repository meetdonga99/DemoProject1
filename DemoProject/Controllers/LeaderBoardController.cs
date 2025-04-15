using DemoProject.Model;
using DemoProject.Models;
using DemoProject.Service;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoProject.Controllers
{
    public class LeaderBoardController : BaseController
    {

        private readonly LeaderBoardService _leaderBoardService;
        private readonly UserExamRecordService _userExamRecordService;
        private readonly PaperSetService _paperSetService;
        private readonly QuestionService _questionService;
        private readonly PaperSetQuestionMappingService _paperSetQuestionMappingService;
        private readonly OptionService _optionService;
        private readonly UserExamAnswerService _userExamAnswerService;
        private readonly MediaService _mediaService;

        public LeaderBoardController()
        {
            _leaderBoardService = new LeaderBoardService();
            _userExamRecordService = new UserExamRecordService();
            _paperSetService = new PaperSetService();
            _questionService = new QuestionService();
            _paperSetQuestionMappingService = new PaperSetQuestionMappingService();
            _optionService = new OptionService();
            _userExamAnswerService = new UserExamAnswerService();
            _mediaService = new MediaService();
        }

        public ActionResult Index()
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.LEADERBOARD.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            return View();
        }

        //[HttpPost]
        //public ActionResult GetLeaderBoardGridData([DataSourceRequest] DataSourceRequest request)
        //{
        //    if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.LEADERBOARD.ToString(), AccessPermission.IsView))
        //    {
        //        return RedirectToAction("AccessDenied", "Base");
        //    }

        //    var data = _leaderBoardService.GetLeaderBoardGrid();
        //    return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public ActionResult GetLeaderBoardGridData([DataSourceRequest] DataSourceRequest request, string paperSetName, string searchTerm)
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.LEADERBOARD.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }

            var data = _leaderBoardService.GetLeaderBoardGrid();

            if (!string.IsNullOrEmpty(paperSetName))
            {
                data = data.Where(x => x.PaperSetName == paperSetName);
            }
            var materializedData = data.ToList().AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                materializedData = materializedData.Where(x =>
                    (x.Email != null && x.Email.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (x.PaperSetName != null && x.PaperSetName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    x.Score.ToString().Contains(searchTerm) ||
                    (x.Date != null && x.Date.ToString("yyyy-MM-dd").IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                );
            }
            return Json(materializedData.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaperSetNames()
        {
            var paperSets = _leaderBoardService.GetLeaderBoardGrid()
                              .Select(x => x.PaperSetName)
                              .Distinct()
                              .ToList();
            return Json(paperSets, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewExam(int userExamRecordId)
        {
            var record = _userExamRecordService.GetRecordByUserExamRecordId(userExamRecordId);
            var getPaperSet = _paperSetService.GetPaperSetById(record.PaperSetId);
            var mappings = _paperSetQuestionMappingService.GetMappingsByPaperSetId(record.PaperSetId);
            var questions = _questionService.GetAllQuestions();

            UserExamViewModel model = new UserExamViewModel();
            model.StartTime = record.StartTime;
            model.EndTime = record.EndTime;
            model.Score = record.Score;

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
                                   options = _optionService.GetOptionsByQuestionId(question.Id).Select(o => new OptionModel { Id = o.Id, QuestionId = o.QuestionId, OptionText = o.OptionText, IsCorrect = o.IsCorrect }).ToList(),
                                   mediaFiles = _mediaService.GetMediaByQuestionId(question.Id).Select(m => new MediaModel
                                   {
                                       Id = m.Id,
                                       QuestionId = m.QuestionId,
                                       MediaName = m.MediaName,
                                       MediaType = m.MediaType,
                                       IsDeleted = false
                                   }).ToList()
                               }
                               ).ToList();

            var data = from i in model.Questions
                       select new CorrectAnswer()
                       {
                           QuestionId = i.Id,
                           CorrectOptions = i.options.Where(o => o.IsCorrect).Select(o => o.Id).ToList()

                       };

            model.Answers = _userExamAnswerService.GetAnswersByExamId(userExamRecordId).Select(a => new SaveAnswerModel
            {
                UserExamRecordId = a.UserExamRecordId,
                QuestionId = a.QuestionId,
                SelectedOptions = a.SelectedOptions.Split(',').Where(s => !string.IsNullOrEmpty(s)).Select(int.Parse).ToList(),
                DescriptiveAnswer = a.DescriptiveAnswer,
                ObtainedMarks = a.ObtainedMarks.Value,
            }).ToList();

            
            TempData["CorrectAnswers"] = data;

            return PartialView("_ViewExam", model);
        }

    }
}