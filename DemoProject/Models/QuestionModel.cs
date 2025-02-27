using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoProject.Models
{
	public class QuestionModel
	{
        public QuestionModel()
        {
            _SubjectList = new List<SelectListItem>();
            _QuestionTypeList = new List<SelectListItem>();
            options = new List<OptionModel>();

        }
        public int Id { get; set; }
        [Required]
        public int SubjectId { get; set; }
        public List<SelectListItem> _SubjectList { get; set; }
        [Required]
        public int QuestionTypeId { get; set; }
        public List<SelectListItem> _QuestionTypeList { get; set; }
        [Required]
        [AllowHtml]
        [Display(Name = "Question")]
        public string QuestionText { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "DefaultMarks must be a positive number.")]
        public int DefaultMarks { get; set; }
        [Required]
        public string DifficultyLevel { get; set; }
        public string Image { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [AllowHtml]
        public List<OptionModel> options { get; set; }

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
        public string BadgeCode { get; set; }
    }
} 