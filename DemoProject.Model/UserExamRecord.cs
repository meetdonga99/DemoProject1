using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Model
{
    public class UserExamRecord
    {
        [Key]
        public int Id { get; set; }
        public int PaperSetId { get; set; }
        public int UserId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int Score { get; set; }
        public decimal Percentage { get; set; }
        public string ExamStatus { get; set; }
        public string Token { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }

        [ForeignKey("PaperSetId")]
        public virtual PaperSet PaperSet { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile User { get; set; }
    }

    public class UserExamRecordGridModel
    {
        public int Id { get; set; }
        public string PaperSetName { get; set; }
        public string UserEmail { get; set; }
        public string ExamStatus { get; set; }
        public DateTime? ExpiryDate { get; set; }

    }
}
