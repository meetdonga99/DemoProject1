using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DemoProject.Models
{
	public class UserExamRecordViewModel
	{
		public int PaperSetId { get; set; }
		[Required]
		public List<string> UserEmails { get; set; }

	}
}