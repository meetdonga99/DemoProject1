using DemoProject.Helper;
using DemoProject.Model;
using DemoProject.Models;
using DemoProject.Service;
using DemoProject.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace DemoProject.Controllers
{
    [Authorize]
    public class UserProfileController : BaseController
    {
        // GET: UserProfile
        private readonly UserProfileService _userProfileService;
        private readonly RoleService _rolesService;
        private readonly FormsService _formsService;
        private readonly FormRoleMappingService _formRoleMapping;
        //private readonly OrganizationService _organizationService;
        //private readonly UserOrganizationMappingService _userOrganizationMappingService;
        //private readonly CandidateUtility _candidateUtility;
        //private readonly MessageService _messageService;
        public UserProfileController()
        {
            _userProfileService = new UserProfileService();
            _rolesService = new RoleService();
            _formsService = new FormsService();
            _formRoleMapping = new FormRoleMappingService();
            //_organizationService = new OrganizationService();
            //_userOrganizationMappingService = new UserOrganizationMappingService();
            //_candidateUtility = new CandidateUtility();
            //_messageService = new MessageService();
        }
        public ActionResult Index()
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.USER.ToString(), AccessPermission.IsView))
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

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.USER.ToString(), actionPermission))
                return RedirectToAction("AccessDenied", "Base");

            UserProfileModel model = new UserProfileModel();
            if (id > 0)
            {
                var getuser = _userProfileService.GetUserById(id.Value);
                if (getuser != null)
                {
                    model.UserId = id.Value;
                    model.Name = getuser.Name;
                    model.UserName = getuser.UserName;
                    model.Email = getuser.Email;
                    //model.EmailSignature = getuser.EmailSignature;
                    //model.Designation = getuser.Designation;
                    model.PhoneNo = getuser.PhoneNo;
                    model.MobileNo = getuser.MobileNo;
                    model.IsActive = getuser.IsActive;
                    int roleId = _userProfileService.GetRoleIdByUserId(id.Value).RoleId;
                    model.Role = _rolesService.GetRolesById(roleId).RoleName;
                    //model.DefaultFormId = getuser.DefaultPageId;
                    //model.OrganizationId = _userOrganizationMappingService.GetOrgIdByUserId(model.UserId);
                }
            }
            BindRole(ref model);
            //BindForm(ref model);
            //model._OrganizationList = _candidateUtility.BindOrganization();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(UserProfileModel model)
        {
            string actionPermission = "";
            if (model.UserId == 0)
            {
                actionPermission = AccessPermission.IsAdd;
            }
            else if (model.UserId > 0)
            {
                actionPermission = AccessPermission.IsEdit;
            }


            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.USER.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            if (model.UserId > 0)
            {
                ModelState.Remove("Password");
            }
            if (ModelState.IsValid)
            {
                SaveUpdateUserProfile(model);
                return RedirectToAction("Index");
            }
            else
            {
                BindRole(ref model);
                //BindForm(ref model);
                //model._OrganizationList = _candidateUtility.BindOrganization();
                return View(model);
            }
        }

        public void SaveUpdateUserProfile(UserProfileModel model)
        {
            int userId = SessionHelper.UserId;

            if (model.UserId > 0)
            {
                UserProfile user = new UserProfile();
                user.Name = model.Name;
                user.UserName = model.UserName;
                user.UserId = model.UserId;
                user.Email = model.Email;
                //user.EmailSignature = model.EmailSignature;
                //user.Designation = model.Designation;
                user.PhoneNo = model.PhoneNo;
                user.MobileNo = model.MobileNo;
                user.IsActive = model.IsActive;
                user.UpdatedOn = DateTime.UtcNow;
                user.UpdatedBy = userId;
                //user.DefaultPageId = (model.DefaultFormId == null ? 0 : model.DefaultFormId.Value);

                int id = _userProfileService.UpdateUserProfile(user);
                int flag = 0;
                //foreach (int org in model.OrganizationId)
                //{
                //    UserOrganizationMapping obj = new UserOrganizationMapping();
                //    obj.UserId = model.UserId;
                //    obj.OrganizationId = org;
                //    if (flag == 0)
                //    {
                //        _userOrganizationMappingService.RemoveUserOrganizationMapping(obj);
                //    }
                //    flag++;
                //    _userOrganizationMappingService.InsertUserOrganizationMapping(obj);
                //}
                foreach (var role in Roles.GetRolesForUser(model.UserName))
                {
                    Roles.RemoveUserFromRole(model.UserName, role);
                }
                Roles.AddUserToRole(model.UserName, model.Role);
            }
            else
            {
                WebSecurity.CreateUserAndAccount(model.UserName, model.Password, propertyValues: new { Name = model.Name, Email = model.Email, PhoneNo = model.PhoneNo, MobileNo = model.MobileNo, IsActive = 1, IsDeleted = 0, CreatedOn = DateTime.UtcNow, CreatedBy = userId, UpdatedOn = DateTime.UtcNow, UpdatedBy = userId });
                Roles.AddUserToRole(model.UserName, model.Role);
                //foreach (int org in model.OrganizationId)
                //{
                    int UserId = _userProfileService.GetIdByUserName(model.UserName);
                    //UserOrganizationMapping obj = new UserOrganizationMapping();
                    //obj.UserId = UserId;
                    //obj.OrganizationId = org;
                    //_userOrganizationMappingService.InsertUserOrganizationMapping(obj);
                //}
            }

        }
        public UserProfileModel BindRole(ref UserProfileModel model)
        {
            var getroles = _rolesService.GetAllRoles().Where(x => x.RoleName.Trim().ToLower() != DemoProject.Helper.Constants.RoleCode.SADMIN.ToLower());
            model._RoleList.Add(new SelectListItem() { Text = "Select Role", Value = "" });
            foreach (var item in getroles)
            {
                model._RoleList.Add(new SelectListItem() { Text = item.RoleName.Trim(), Value = item.RoleName.Trim() });
            }
            return model;
        }

        //public UserProfileModel BindForm(ref UserProfileModel model)
        //{
        //    model._DefaultFormList.Add(new SelectListItem() { Text = "Select Form", Value = "0" });
        //    if (model.UserId > 0)
        //    {
        //        int RoleId = _rolesService.GetRolesByName(model.Role).RoleId;
        //        List<int> getformId = _formRoleMapping.GetAllRoleRightsByRoleId(RoleId).Select(x => x.MenuId).ToList();

        //        var getchildform = _formsService.GetAllForms().Where(x => x.NavigateURL != "#" && getformId.Contains(x.Id)).Select(a => new FormModel { Id = a.Id, Name = a.Name }).OrderBy(a => a.Name);

        //        foreach (var item in getchildform)
        //        {
        //            model._DefaultFormList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
        //        }
        //    }
        //    return model;
        //}

        //public JsonResult CheckDuplicateUserEmail(string Email, int UserId)
        //{
        //    var checkduplicate = _userProfileService.CheckDuplicateUserEmail(Email).ToList();
        //    if (UserId > 0)
        //    {
        //        checkduplicate = checkduplicate.Where(x => x.UserId != UserId).ToList();
        //    }
        //    if (checkduplicate.Count() > 0)
        //    {
        //        var message = _messageService.GetMessageByCode(Constants.MessageCode.EMAILEXIST);
        //        return Json(message, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(true, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //public JsonResult CheckDuplicateUserName(string UserName, int UserId)
        //{
        //    var checkduplicate = _userProfileService.CheckDuplicateUserName(UserName).ToList();
        //    if (UserId > 0)
        //    {
        //        checkduplicate = checkduplicate.Where(x => x.UserId != UserId).ToList();
        //    }
        //    if (checkduplicate.Count() > 0)
        //    {
        //        var message = _messageService.GetMessageByCode(Constants.MessageCode.USERNAMEEXIST);
        //        return Json(message, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(true, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public ActionResult User_Read([DataSourceRequest] DataSourceRequest request)
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.USER.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            var getallusers = _userProfileService.GetAllUserProfileGrid();
            return Json(getallusers.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        public JsonResult CasCadeForm(string Role)
        {
            List<SelectListItem> _DefaultFormList = new List<SelectListItem>();
            _DefaultFormList.Add(new SelectListItem() { Text = "Select Form", Value = "" });
            if (!string.IsNullOrWhiteSpace(Role))
            {
                int RoleId = _rolesService.GetRolesByName(Role).RoleId;
                List<int> getformId = _formRoleMapping.GetAllRoleRightsByRoleId(RoleId).Select(x => x.MenuId).ToList();
                var getparentform = _formsService.GetAllForms().Where(x => x.NavigateURL != "#" && getformId.Contains(x.Id)).Select(a => new FormModel { Id = a.Id, Name = a.Name }).OrderBy(a => a.Name);

                foreach (var item in getparentform)
                {
                    _DefaultFormList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
                }
            }
            return Json(_DefaultFormList, JsonRequestBehavior.AllowGet);
        }
    }
}