using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DemoProject.Models
{
	public class OptionModel
	{
        public int Id { get; set; }
        [Required]
        public int QuestionId { get; set; }
        [Required]
        [Display(Name = "Option Text")]
        public string OptionText { get; set; }
        [Required]
        public bool IsCorrect { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}