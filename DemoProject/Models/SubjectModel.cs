using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoProject.Models
{
    public class SubjectModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public string _SubjectCode { get; set; }


        [Required]
        [Display(Name = "Subject Code")]
        [Remote("CheckDuplicateSubjectCode", "Subject", HttpMethod = "Post", AdditionalFields = "Id")]
        public string Code
        {
            get
            {
                if (string.IsNullOrEmpty(_SubjectCode))
                {
                    return _SubjectCode;
                }
                return _SubjectCode.ToUpper();
            }
            set
            {
                _SubjectCode = value;
            }
        }

        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    public class SubjectGridModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
    }

}