using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DemoProject.Helper;
using DemoProject.Models;
using DemoProject.Service;
using Kendo.Mvc.Extensions;

namespace DemoProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserExamRecordService _userExamRecordService;
        private readonly UserProfileService _userProfileService;
        private readonly SubjectService _subjectService;
        private readonly PaperSetService _paperSetService;
        public HomeController()
        {
            _userExamRecordService = new UserExamRecordService();
            _userProfileService = new UserProfileService();
            _subjectService = new SubjectService();
            _paperSetService = new PaperSetService();
        }

        public ActionResult Index()
        {
            var allRecords = _userExamRecordService.GetAllRecords();
            var examsPerMonth = allRecords.Where(a => a.StartTime.HasValue).GroupBy(a => a.StartTime.Value.Month).Select(a => new { Month = a.Key, Count = a.Count() }).ToList();
            var examsPerMonthLabels = examsPerMonth.Select(e => $"Month {e.Month}").ToList();
            var examsPerMonthData = examsPerMonth.Select(e => e.Count).ToList();


            var passedCount = allRecords.Where(a => a.ExamStatus == Constants.ExamStatus.RESULT_PUBLISHED && a.Percentage >= 33).Count();
            var failedCount = allRecords.Where(a => a.ExamStatus == Constants.ExamStatus.RESULT_PUBLISHED && a.Percentage < 33).Count();

            var examsPerPaperSet = allRecords
        .GroupBy(a => a.PaperSetId)
        .Select(g => new { PaperSetId = g.Key, Count = g.Count() })
        .ToList();

            var paperSets = _paperSetService.GetAllPaperSets();
            var paperSetLabels = (from i in examsPerPaperSet
                                  join j in paperSets on i.PaperSetId equals j.Id
                                  select j.PaperSetName).ToList();
            var paperSetData = examsPerPaperSet.Select(p => p.Count).ToList();

            var model = new DashboardViewModel
            {
                ExamsPerMonthLabels = examsPerMonthLabels,
                ExamsPerMonthData = examsPerMonthData,
                PassedCount = passedCount,
                FailedCount = failedCount,
                TotalExams = allRecords.Count(),
                TotalCandidates = _userProfileService.GetAllUserProfile("CANDIDATE").Count(),
                TotalSubjects = _subjectService.GetAllSubjects().Count(),
                AvgScore = allRecords.Where(a => a.ExamStatus == Constants.ExamStatus.RESULT_PUBLISHED).Select(a => a.Percentage).Average(),
                PaperSetLabels = paperSetLabels,
                PaperSetData = paperSetData
            };

            return View(model);
        }

        
    }
}