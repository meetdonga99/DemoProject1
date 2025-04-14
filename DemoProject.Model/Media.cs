using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Model
{
    public class Media
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string MediaName { get; set; }
        public string MediaType { get; set; }
       
    }
}
