using DemoProject.Model;
using DemoProject.Models;
using DemoProject.Service;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoProject.Controllers
{
    public class ConfigurationController : BaseController
    {
        string formCode = AuthorizeFormAccess.FormAccessCode.CONFIGURATION.ToString();
        private readonly ConfigurationService _configurationService;
        public ConfigurationController()
        {
            _configurationService = new ConfigurationService();
        }

        public ActionResult Index()
        {
            if (!CheckPermission(formCode, AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            return View();
        }

        public ActionResult Create(int? id)
        {
            string actionPermission = "";
            if (id == null)
            {
                actionPermission = AccessPermission.IsAdd;
            }
            else if ((id ?? 0) > 0)
            {
                actionPermission = AccessPermission.IsEdit;
            }

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.CONFIGURATION.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            int userId = SessionHelper.UserId;
            ConfigurationModel model = new ConfigurationModel();
            if (id.HasValue)
            {
                var configurationDetails = _configurationService.GetConfigurationById(id.Value);
                if (configurationDetails != null)
                {
                    model.Id = configurationDetails.Id;
                    model.ConfigurationKey = configurationDetails.ConfigurationKey;
                    model.Value = configurationDetails.Value;
                    model.Comment = configurationDetails.Comment;

                }
            }
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Create(ConfigurationModel model)
        {
            string actionPermission = "";
            if (model.Id == 0)
            {
                actionPermission = AccessPermission.IsAdd;
            }
            else if (model.Id > 0)
            {
                actionPermission = AccessPermission.IsEdit;
            }

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.CONFIGURATION.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            int userId = SessionHelper.UserId;
            if (ModelState.IsValid)
            {
                SaveUpdatedConfiguration(model);
            }
            return RedirectToAction("Index");
        }

        public ConfigurationModel SaveUpdatedConfiguration(ConfigurationModel model)
        {
            int userId = SessionHelper.UserId;
            Configuration obj = new Configuration();
            if (model.Id > 0)
            {
                obj = _configurationService.GetConfigurationById(model.Id);
            }
            obj.Id = model.Id;
            obj.ConfigurationKey = model.ConfigurationKey;
            obj.Value = model.Value;
            obj.Comment = model.Comment;
            if (obj.Id == 0)
            {
                obj.CreatedBy = userId;
                obj.CreatedOn = DateTime.UtcNow;
                model.Id = _configurationService.CreateConfiguration(obj);
            }
            else
            {
                obj.UpdatedBy = userId;
                obj.UpdatedOn = DateTime.UtcNow;
                _configurationService.UpdateConfiguration(obj);
            }
            return model;
        }

        [HttpPost]
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request, string searchTerm)
        {
            if (!CheckPermission(formCode, AccessPermission.IsView))
                return AccessDenied();


            var configurationData = _configurationService.GetAllConfigurations();

            var materializedData = configurationData.ToList().AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                materializedData = materializedData.Where(x =>
                    (x.ConfigurationKey != null && x.ConfigurationKey.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (x.Value != null && x.Value.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (x.Comment != null && x.Comment.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)

                );
            }

            return Json(materializedData.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

    }
}