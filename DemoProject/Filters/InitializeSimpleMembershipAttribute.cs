﻿using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using WebMatrix.WebData;
using DemoProject.Model;
using System.Web.Security;
using DemoProject.Models;

namespace DemoProject.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    [DbConfigurationType(typeof(DemoProjectContextConfiguration))]
    public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute
    {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Ensure ASP.NET Simple Membership is initialized only once per app start
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        private class SimpleMembershipInitializer
        {
            public SimpleMembershipInitializer()
            {
                Database.SetInitializer<UsersContext>(null);

                try
                {
                    using (var context = new UsersContext())
                    {
                        if (!context.Database.Exists())
                        {
                            // Create the SimpleMembership database without Entity Framework migration schema
                            ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                        }
                    }

                    WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);

                    string role = "sadmin";
                    string user = "sadmin";
                    string password = "sadmin@123";


                    if (!Roles.RoleExists(role))
                    {
                        Roles.CreateRole(role);
                    }

                    if (!WebSecurity.UserExists(user))
                    {
                        WebSecurity.CreateUserAndAccount(user, password, propertyValues: new { Name = "sadmin", Email = "sadmin", IsActive = 1, IsDeleted = 0, CreatedOn = DateTime.Now, CreatedBy = 1 });
                        Roles.AddUserToRole(user, role);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("", ex);
                }
            }
        }
    }
}
