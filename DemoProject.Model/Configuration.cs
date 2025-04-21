using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Model
{
    public class Configuration
    {
        public int Id { get; set; }
        public string ConfigurationKey { get; set; }
        public string Value { get; set; }
        public string Comment { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }

    public class ConfigurationGridModel
    {
        public int Id { get; set; }
        public string ConfigurationKey { get; set; }
        public string Value { get; set; }
        public string Comment { get; set; }

    }
}
