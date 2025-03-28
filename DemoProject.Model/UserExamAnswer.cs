using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Model
{
    public class UserExamAnswer
    {
        [Key]
        public int Id { get; set; }
        public int UserExamRecordId { get; set; }
        public int QuestionId { get; set; }
        public string SelectedOptions { get; set; }
        public string DescriptiveAnswer { get; set; }
        public int? ObtainedMarks { get; set; }
    }
}
