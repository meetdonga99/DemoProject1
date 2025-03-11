using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Model
{
    public class PaperSetQuestionMapping
    {
        [Key]
        public int Id { get; set; }
        public int PaperSetId { get; set; }
        public int QuestionId { get; set; }
        public int CustomMarks { get; set; }
        [ForeignKey("PaperSetId")]
        public virtual PaperSet PaperSet { get; set; }
        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }
    }
}
