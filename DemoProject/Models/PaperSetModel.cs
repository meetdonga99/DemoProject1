using DemoProject.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DemoProject.Models
{
	public class PaperSetModel
	{
        public int Id { get; set; }
        [Required]
        public string PaperSetName { get; set; }
        [Required]
        [MinValueRequired(20)]
        public int TotalMarks { get; set; }
        [Required]
        [MinValueRequired(10)]
        [Display(Name = "Duration In Minutes")]
        public int DurationInMinutes { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public int CurrentMarks { get; set; }
        public List<QuestionMappingItem> QuestionMappings { get; set; }
    }
    public class QuestionMappingItem
    {
        public int PaperSetId { get; set; }
        public int QuestionId { get; set; }
        public int CustomMarks { get; set; }
    }

    public class QuestionViewModel
    {
        public int QuestionId { get; set; }
        public string SubjectName { get; set; }
        public string QuestionType { get; set; }
        public string QuestionText { get; set; }
        public int DefaultMarks { get; set; }
        public string DifficultyLevel { get; set; }
        public string Image { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ViewPaperSetModel
    {
        public int Id { get; set; }
        public string PaperSetName { get; set; }
        public int TotalMarks { get; set; }
        public int DurationInMinutes { get; set; }
        public List<QuestionModel> Questions { get; set; } 

    }
}