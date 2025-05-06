using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DemoProject.Models
{
	public class ArticleModel
	{
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string ArticleContent { get; set; }
        [Required]
        public DateTime PublicationDate { get; set; }
    }
}