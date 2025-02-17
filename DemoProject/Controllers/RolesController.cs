using DemoProject.Model;
using DemoProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DemoProject.Service;
using DemoProject.Helper;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace DemoProject.Controllers
{
    [Authorize]
    public class RolesController : BaseController
    {
        // GET: Roles

        private readonly RoleService _rolesService;
        public RolesController()
        {
            _rolesService = new RoleService();
           
        }
        public ActionResult Index()
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.ROLES.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }

            return View();
        }

        public ActionResult Create(int? id)
        {
            //var a = 5;
            //var b = a / 0;
            string actionPermission = "";
            if (id == null)
            {
                actionPermission = AccessPermission.IsAdd;
            }
            else if ((id ?? 0) > 0)
            {
                actionPermission = AccessPermission.IsEdit;
            }

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.ROLES.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }

            int userId = SessionHelper.UserId;
            RolesModel model = new RolesModel();
            if (id.HasValue)
            {
                var rolesDetail = _rolesService.GetRolesById(id.Value);
                if (rolesDetail != null)
                {
                    model.Id = id.Value;
                    model.Name = rolesDetail.RoleName;
                    model.RoleCode = rolesDetail.RoleCode;
                    model.IsActive = rolesDetail.IsActive;
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(RolesModel model)
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

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.ROLES.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }

            int userId = SessionHelper.UserId;
            if (ModelState.IsValid)
            {
                SaveUpdateRoles(model);
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }

        public RolesModel SaveUpdateRoles(RolesModel model)
        {
            webpages_Roles obj = new webpages_Roles();
            if (model.Id > 0)
            {
                obj = _rolesService.GetRolesById(model.Id);
            }
            int userId = SessionHelper.UserId;
            obj.RoleId = model.Id;
            obj.IsActive = model.IsActive;
            obj.RoleName = model.Name;
            obj.UpdatedBy = userId;
            obj.UpdatedOn = DateTime.UtcNow;
            if (obj.RoleId == 0)
            {
                obj.RoleCode = model.RoleCode;
                model.Id = _rolesService.CreateRoles(obj);
            }
            else
            {
                _rolesService.UpdateRoles(obj);
            }
            return model;
        }

        [HttpPost]
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request)
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.ROLES.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            var data = _rolesService.GetAllRolesGrid();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        //public JsonResult CheckDuplicateRoleCode(string RoleCode, int Id)
        //{
        //    var getRoleDetails = _rolesService.CheckDuplicateRoleCode(RoleCode);
        //    if (Id > 0)
        //    {
        //        getRoleDetails = getRoleDetails.Where(a => a.RoleId != Id).ToList();
        //    }
        //    if (getRoleDetails.Count() > 0)
        //    {
        //        var message = _messageService.GetMessageByCode(Constants.MessageCode.CODEEXIST);
        //        return Json(message, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(true, JsonRequestBehavior.AllowGet);
        //    }
        //}
    }
}