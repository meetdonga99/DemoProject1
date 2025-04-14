using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoProject.Models
{
	public class MediaModel
	{
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string MediaName { get; set; }
        public string MediaType { get; set; }
        public bool IsDeleted { get; set; }
    }
}