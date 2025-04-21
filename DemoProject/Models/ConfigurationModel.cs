using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoProject.Models
{
	public class ConfigurationModel
	{
        public int Id { get; set; }
        [Required(ErrorMessage = "Key is required.")]
        public string ConfigurationKey { get; set; }
        [Required(ErrorMessage = "Value is required.")]
        public string Value { get; set; }
        public string Comment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}