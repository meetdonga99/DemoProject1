using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using System.Threading.Tasks;
using DemoProject.Service;
using DemoProject.Model;

namespace DemoProject.Helper
{
	public class MyJob : IJob
	{
        private readonly UserExamRecordService _userExamRecordService;
        private readonly PaperSetQuestionMappingService _paperSetQuestionMappingService;
        private readonly UserExamAnswerService _userExamAnswerService;


        public MyJob()
        {
            _userExamRecordService = new UserExamRecordService();
            _paperSetQuestionMappingService = new PaperSetQuestionMappingService();
            _userExamAnswerService = new UserExamAnswerService();
        }
        public Task Execute(IJobExecutionContext context)
        {

            var records = _userExamRecordService.GetAllInprogressRecords().Where(a => DateTime.UtcNow > a.EndTime);
            foreach(var i in records)
            {
                var questionIds = _paperSetQuestionMappingService.GetMappingsByPaperSetId(i.PaperSetId).Select(o => o.QuestionId).ToList();
                var existingAnswerQuestionIds = _userExamAnswerService.GetAnswersByExamId(i.Id).Select(o => o.QuestionId).ToList();

               foreach(var j in questionIds)
                {
                    if (!existingAnswerQuestionIds.Contains(j))
                    {
                        var newAnswer = new UserExamAnswer
                        {
                            UserExamRecordId = i.Id,
                            QuestionId = j,
                            SelectedOptions = "",
                            DescriptiveAnswer =null,
                            ObtainedMarks = 0,
                            IsEvaluated = false
                        };
                        _userExamAnswerService.CreateUserExamAnswer(newAnswer);
                    }
                }

                i.ExamStatus = "COMPLETED";
                _userExamRecordService.UpdateUserExamRecord(i);
                System.Diagnostics.Debug.WriteLine("Job Executed " + i.Id);
            }

            return Task.CompletedTask;
        }
    }
}