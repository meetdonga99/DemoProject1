using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoProject.Model
{
	public class LeaderBoardGridModel
	{
		public int UserExamRecordId { get; set; }
		public string Email { get; set; }
		public string PaperSetName { get; set; }
		public int Score { get; set; }
		public DateTime Date { get; set; }
	}
}