using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Data
{
    public class ArticleProvider : BaseProvider
    {
        public ArticleProvider()
        {

        }

        public int CreateArticle(Article article)
        {
            try
            {
                _db.Article.Add(article);
                _db.SaveChanges();
                return article.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdateArticle(Article article)
        {
            try
            {
                _db.Entry(article).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return article.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IQueryable<ArticleGridModel> GetAllArticlesGrid()
        {
         
            return (from i in _db.Article select new ArticleGridModel()
            {
                Id = i.Id,
                Title = i.Title,
                ArticleContent = i.ArticleContent,
                PublicationDate = i.PublicationDate
            } ).AsQueryable();
        }

        public Article GetArticleById(int id)
        {
            return (from i in _db.Article where i.Id == id select i).FirstOrDefault();
        }

        public List<Article> GetAllArticles()
        {
            return (from i in _db.Article select i).ToList();
        }
    }
}
