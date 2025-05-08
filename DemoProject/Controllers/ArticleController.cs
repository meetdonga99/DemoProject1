using DemoProject.Model;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DemoProject.Service;
using Kendo.Mvc.Extensions;
using DemoProject.Models;

namespace DemoProject.Controllers
{
    public class ArticleController : BaseController
    {
        private readonly ArticleService _articleService;

        public ArticleController()
        {
            _articleService = new ArticleService();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(int? id)
        {
            int userId = SessionHelper.UserId;
            ArticleModel model = new ArticleModel();
            if (id.HasValue)
            {
                var articleDetail = _articleService.GetArticleById(id.Value);
                if (articleDetail != null)
                {
                    model.Id = id.Value;
                    model.Title = articleDetail.Title;
                    model.ArticleContent = articleDetail.ArticleContent;
                    model.PublicationDate = articleDetail.PublicationDate;
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ArticleModel model)
        {
            int userId = SessionHelper.UserId;
            if (ModelState.IsValid)
            {
                SaveUpdateArticle(model);
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }

        public ArticleModel SaveUpdateArticle(ArticleModel model) 
        {
            Article obj = new Article();
            if (model.Id > 0)
            {
                obj = _articleService.GetArticleById(model.Id);
            }
            int userId = SessionHelper.UserId; 
            obj.Id = model.Id;
            obj.Title = model.Title;
            obj.ArticleContent = model.ArticleContent;
            obj.PublicationDate = model.PublicationDate;
            if (obj.Id == 0)
            {
                model.Id = _articleService.CreateArticle(obj);
            }
            else
            {
                _articleService.UpdateArticle(obj);
            }
            return model;
        }

        [HttpPost]
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request, string selectedMonths = null)
         {
            var data = _articleService.GetAllArticlesGrid();

           
            if (!string.IsNullOrEmpty(selectedMonths))
            {
                var monthsList = selectedMonths.Split(',').ToList();

                if (monthsList.Any())
                {
                    var filteredData = new List<ArticleGridModel>();

                    foreach (var article in data)
                    {                       
                        string articleYearMonth = article.PublicationDate.ToString("yyyy-MM");                    
                        if (monthsList.Contains(articleYearMonth))
                        {
                            filteredData.Add(article);
                        }
                    }

                    data = filteredData.AsQueryable();
                }
            }
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}