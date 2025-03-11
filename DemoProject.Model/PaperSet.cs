using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Model
{
    public class PaperSet
    {
        [Key]
        public int Id { get; set; }
        public string PaperSetName { get; set; }
        public int TotalMarks { get; set; }
        public int DurationInMinutes { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public int DeletedBy { get; set; }
    } 

    public class PaperSetGridModel
    {
        public int Id { get; set; }
        public string PaperSetName { get; set; }
        public int TotalMarks { get; set; }
        public int DurationInMinutes { get; set; }
        public string Status { get; set; }
        public string BadgeCode { get; set; }
        public bool IsActive { get; set; }
    }
}
