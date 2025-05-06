using DemoProject.Data;
using DemoProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Service
{
    public class ArticleService
    {
        private readonly ArticleProvider _articleProvider;
        public ArticleService()
        {
            _articleProvider = new ArticleProvider();
        }

        public int CreateArticle(Article article)
        {
            return _articleProvider.CreateArticle(article);
        }

        public int UpdateArticle(Article article)
        {
            return _articleProvider.UpdateArticle(article);
        }

        public IQueryable<ArticleGridModel> GetAllArticlesGrid()
        {
            return _articleProvider.GetAllArticlesGrid();
        }

        public Article GetArticleById(int id)
        {
            return _articleProvider.GetArticleById(id);
        }

        public List<Article> GetAllArticles()
        {
            return _articleProvider.GetAllArticles();
        }
    }
}
