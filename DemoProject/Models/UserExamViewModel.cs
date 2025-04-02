using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoProject.Models
{
    public class UserExamViewModel
    {
        public int UserExamRecordId { get; set; }
        public string ExamStatus { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int PaperSetId { get; set; }
        public string PaperSetName { get; set; }
        public int TotalMarks { get; set; }
        public int Score { get; set; }
        public int DurationInMinutes { get; set; }
        public List<QuestionModel> Questions { get; set; }
        public List<SaveAnswerModel> Answers { get; set; }
    }

    public class SaveAnswerModel
    {
        public int UserExamRecordId { get; set; }
        public int QuestionId { get; set; }
        public List<int> SelectedOptions { get; set; } = new List<int>();
        public string DescriptiveAnswer { get; set; }
        public int ObtainedMarks { get; set; }
        public bool IsEvaluated { get; set; }
    }

    public class CorrectAnswer
    {
        public int QuestionId { get; set; }
        public List<int> CorrectOptions { get; set; }
    }
}