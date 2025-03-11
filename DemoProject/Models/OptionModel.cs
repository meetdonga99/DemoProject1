using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoProject.Models
{
	public class OptionModel
	{
        public int Id { get; set; }
        public int QuestionId { get; set; }
        [Required(ErrorMessage = "Option text is required.")]
        [Display(Name = "Option Text")]
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}