using DemoProject.Model;
using DemoProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using System.Web.Security;
using System.Threading;
using System.Configuration;
using static DemoProject.Model.SessionHelper;
using System.Runtime.Remoting.Messaging;
using System.Web.ApplicationServices;
using AIRecruitment.Filters;

namespace DemoProject.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
    
        public AccountController()
        {
           
        }
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            LoginModel model = new LoginModel();
            if (SessionHelper.UserId > 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName.Trim(), model.Password, persistCookie: model.RememberMe))
            {
                var userId = WebSecurity.GetUserId(model.UserName);
                SessionHelper.UserId = userId;
                SessionHelper.IsAdmin = true;
                SessionHelper.UserName = model.UserName;
                //SessionHelper.RoleName = Roles.GetRolesForUser(model.UserName).FirstOrDefault();
                //SessionHelper.RoleId = _roleService.GetRolesByName(SessionHelper.RoleName).RoleId;
                //SessionHelper.RoleCode = _roleService.GetRolesById(SessionHelper.RoleId).RoleCode;
                //SessionHelper.UserEmailId = _userProfileService.GetUserById(SessionHelper.UserId).Email;
                //SessionHelper.Name = _userProfileService.GetUserById(SessionHelper.UserId).Name;
                //SessionHelper.OrganizationIds = new List<int>();
                //if (SessionHelper.RoleCode != Constants.RoleCode.SADMIN && SessionHelper.RoleCode != Constants.RoleCode.ADMIN)
                //{
                //    SessionHelper.OrganizationIds = _userOrganizationService.GetOrgIdByUserId(SessionHelper.UserId).ToList();
                //    SessionHelper.IsAdmin = false;
                //}
                //if (!string.IsNullOrEmpty(model.TimeZone))
                //{
                //    TimeZoneInfo tzi = CommonUtility.OlsonTimeZoneToTimeZoneInfo(model.TimeZone);
                //    if (tzi != null)
                //    {
                //        SessionHelper.DefaultTimeZone = tzi.StandardName;
                //    }
                //}
                //Session["Menu"] = _formRoleService.GetMenu(userId);

                //if (returnUrl == null)
                //    return RedirectToAction("Index", "Home");
                //else
                //    return RedirectToLocal(returnUrl);
                return RedirectToAction("Index","Home");
            }
            else
            {
                ModelState.AddModelError("Password", "Invalid Username or Password");
                return View(model);
            }
        }

        //private ActionResult RedirectToLocal(string returnUrl)
        //{
        //    if (Url.IsLocalUrl(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //}


        [HttpPost]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }

        //[HttpGet]
        //[AllowAnonymous]
        //public ActionResult ForgotPassword()
        //{
        //    if (TempData["Error"] != null && TempData["Error"].ToString() != "")
        //    {
        //        ViewBag.Error = TempData["Error"].ToString();
        //    }

        //    return View();
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult ForgotPassword(ForgotPaswordModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var userexist = _userProfileService.GetUserByEmailId(model.EmailId);
        //        if (userexist != null)
        //        {
        //            ThreadStart thrdStart = new ThreadStart(() => SendForgotPasswordMail(userexist.Email, userexist.Name, userexist.UserName));
        //            Thread mailThread = new Thread(thrdStart);
        //            mailThread.IsBackground = true;
        //            mailThread.Start();
        //        }
        //    }
        //    string passwordResetEmailSent = _messageService.GetMessageByCode(Constants.MessageCode.PASSWORDRESETEMAILSENT);
        //    passwordResetEmailSent = passwordResetEmailSent.Replace("@@EmailId@@", model.EmailId);
        //    ViewBag.MailSentSuccess = passwordResetEmailSent;
        //    model.EmailId = "";
        //    return View(model);
        //}

        //public void SendForgotPasswordMail(string ToEmail, string name, string userName)
        //{
        //    MembershipUser user;

        //    var bodyVariables = new Dictionary<string, string>();
        //    var subjectVariables = new Dictionary<string, string>();

        //    bodyVariables.Add("@@User@@", name);
        //    try
        //    {
        //        var emailTemplate = _emailTemplateService.GetEmailTemplateByCode(Constants.EmailCodes.FORGOTPASSWORD.ToString());
        //        var newTemplate = _emailTemplateService.ReplaceParameterValuesInEmailTemplate(emailTemplate, subjectVariables, bodyVariables);
        //        var body = newTemplate.MailBody;
        //        var subject = newTemplate.Subject;
        //        int templateID = emailTemplate.Id;

        //        string navigateURL = "javascript://";

        //        string WebPath = Convert.ToString(ConfigurationManager.AppSettings["BaseUrl"]);

        //        string userencrypt = Helper.CommonUtility.Encrypt(userName);

        //        user = Membership.GetUser(userName);

        //        int PassExpHours = 120;
        //        var token = WebSecurity.GeneratePasswordResetToken(user.UserName, PassExpHours);

        //        navigateURL = "" + WebPath + "/Account/ManageChangePassword?uid=" + userencrypt + "&token=" + token;

        //        body = body.Replace("@@PasswordLink@@", navigateURL);

        //        List<string> To = new List<string>();
        //        List<string> CC = new List<string>();
        //        List<string> BCC = new List<string>();

        //        To.Add(ToEmail);

        //        bool isSuccess = true;
        //        isSuccess = Helper.EmailHelper.SendEmail(To, CC, BCC, subject, body);
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }

        //}
        //[HttpGet]
        //[AllowAnonymous]
        //public ActionResult ManageChangePassword(string uid, string uname, string token, string eid)
        //{
        //    LocalPasswordModel_CP model = new LocalPasswordModel_CP();
        //    model.Forgotuid = uid;

        //    if (uid != null)
        //    {
        //        string username = Helper.CommonUtility.Decrypt(model.Forgotuid);
        //        model.UserName = username;
        //        model.ReturnToken = token;

        //        var checktokenExists = _userProfileService.Getwebpages_MembershipByUserId(WebSecurity.GetUserId(model.UserName));
        //        if (checktokenExists != null)
        //        {
        //            if (!(checktokenExists.PasswordVerificationTokenExpirationDate != null && checktokenExists.PasswordVerificationToken == token && checktokenExists.PasswordVerificationTokenExpirationDate >= DateTime.UtcNow))
        //            {
        //                TempData["Error"] = _messageService.GetMessageByCode(Constants.MessageCode.PASSWORDRESETLINKEXPIRED);
        //                return RedirectToAction("ForgotPassword");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        model.UserName = Helper.CommonUtility.Decrypt(uname);
        //    }
        //    return View(model);
        //}
        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult ManageChangePassword(LocalPasswordModel_CP model)
        //{
        //    bool hasLocalAccount = false;
        //    if (model.Forgotuid != null)
        //    {
        //        hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(model.UserName));
        //    }
        //    else
        //    {
        //        hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //    }
        //    ViewBag.HasLocalPassword = hasLocalAccount;
        //    ViewBag.ReturnUrl = Url.Action("ManageChangePassword");
        //    if (hasLocalAccount)
        //    {
        //        if (ModelState.IsValid)
        //        {

        //            bool changePasswordSucceeded = false;
        //            try
        //            {
        //                if (model.Forgotuid != null)
        //                {
        //                    changePasswordSucceeded = WebSecurity.ResetPassword(model.ReturnToken, model.NewPassword);
        //                }
        //            }
        //            catch (Exception)
        //            {
        //                changePasswordSucceeded = false;
        //            }

        //            if (changePasswordSucceeded)
        //            {
        //                TempData["Success"] = _messageService.GetMessageByCode(Constants.MessageCode.PASSWORDCHANGESUCCESS);
        //                SessionHelper.UserId = 0;
        //                return RedirectToAction("LogIn");
        //            }
        //            else
        //            {
        //                ViewBag.Error = _messageService.GetMessageByCode(Constants.MessageCode.PASSWORDRESETLINKEXPIRED);
        //                return View(model);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        ModelState state = ModelState["OldPassword"];
        //        if (state != null)
        //        {
        //            state.Errors.Clear();
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            try
        //            {
        //                WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
        //                TempData["Success"] = _messageService.GetMessageByCode(Constants.MessageCode.PASSWORDCHANGESUCCESS);
        //                SessionHelper.UserId = 0;
        //                return RedirectToAction("LogIn");
        //            }
        //            catch (Exception)
        //            {
        //                TempData["ErrorMsg"] = _messageService.GetMessageByCode(Constants.MessageCode.PASSWORDERRORMESSAGE);
        //            }
        //        }
        //    }
        //    return View(model);
        //}
        //public ActionResult Manage()
        //{
        //    ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //    ViewBag.ReturnUrl = Url.Action("Manage");
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Manage(LocalPasswordModel model)
        //{
        //    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //    ViewBag.HasLocalPassword = hasLocalAccount;
        //    ViewBag.ReturnUrl = Url.Action("Manage");


        //    if (ModelState.IsValid)
        //    {
        //        bool changePasswordSucceeded;

        //        if (model.OldPassword == model.NewPassword)
        //        {
        //            TempData["SamePassword"] = _messageService.GetMessageByCode(Constants.MessageCode.SAMEPASSWORDMESSAGE);
        //            return RedirectToAction("Manage");
        //        }
        //        else
        //        {
        //            try
        //            {
        //                changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
        //            }
        //            catch (Exception)
        //            {
        //                changePasswordSucceeded = false;
        //            }
        //            if (changePasswordSucceeded)
        //            {
        //                TempData["Success"] = _messageService.GetMessageByCode(Constants.MessageCode.PASSWORDCHANGESUCCESS);
        //                return RedirectToAction("Manage");
        //            }
        //            else
        //            {
        //                TempData["Error"] = _messageService.GetMessageByCode(Constants.MessageCode.OLDPASSWORDINCORRECTMESSAGE);
        //            }
        //        }
        //    }
        //    return View(model);
        //}
    }
}