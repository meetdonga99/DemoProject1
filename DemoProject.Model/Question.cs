using System;
using System.ComponentModel.DataAnnotations;

namespace DemoProject.Model
{
    public class Question
    {
        [Key]
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public int QuestionTypeId { get; set; }
        public string QuestionText { get; set; }
        public int DefaultMarks { get; set; }
        public string DifficultyLevel { get; set; }
        public string Image { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public int DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
    
    public class QuestionGridModel
    {
        public int Id { get; set; }
        public string SubjectName { get; set; }
        public string QuestionType { get; set; }
        public string QuestionText { get; set; }
        public int DefaultMarks { get; set; }
        public string DifficultyLevel { get; set; }
        public bool IsActive { get; set; }
    }
}