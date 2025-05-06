using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Model
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ArticleContent { get; set; }
        public DateTime PublicationDate { get; set; }
    }

    public class ArticleGridModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ArticleContent { get; set; }
        public DateTime PublicationDate { get; set; }
    }
}
