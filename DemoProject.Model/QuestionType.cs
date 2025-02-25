using System;
using System.ComponentModel.DataAnnotations;

namespace DemoProject.Model
{
    public class QuestionType
    {
        [Key]
        public int Id { get; set; }
        public string TypeName { get; set; }
        public string TypeCode { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
    public class QuestionTypeGridModel
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public string TypeCode { get; set; }
        public bool IsActive { get; set; }
    }
}