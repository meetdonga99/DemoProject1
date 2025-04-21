using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoProject.Models
{
	public class DashboardViewModel
	{
		public int TotalExams { get; set; }
        public int TotalCandidates { get; set; }
        public int TotalSubjects { get; set; }
        public decimal AvgScore { get; set; }
        public List<string> ExamsPerMonthLabels { get; set; }
        public List<int> ExamsPerMonthData { get; set; }
        public int PassedCount { get; set; }
        public int FailedCount { get; set; }
        public List<string> PaperSetLabels { get; set; }
        public List<int> PaperSetData { get; set; }
    }
}