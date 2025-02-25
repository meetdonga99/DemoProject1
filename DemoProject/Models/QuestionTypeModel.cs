using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoProject.Models
{
    public class QuestionTypeModel
    {
        public int Id { get; set; }
        [Required]
        public string TypeName { get; set; }

        public string _TypeCode { get; set; }


        [Required]
        [Display(Name = "Question Type Code")]
        [Remote("CheckDuplicateQuestionTypeCode", "QuestionType", HttpMethod = "Post", AdditionalFields = "Id")]
        public string TypeCode
        {
            get
            {
                if (string.IsNullOrEmpty(_TypeCode))
                {
                    return _TypeCode;
                }
                return _TypeCode.ToUpper();
            }
            set
            {
                _TypeCode = value;
            }
        }

        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    public class QuestionTypeGridModel
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public string TypeCode { get; set; }
        public bool IsActive { get; set; }
    }

}