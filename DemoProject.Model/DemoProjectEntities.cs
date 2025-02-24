﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.Model
{
    public class DemoProjectEntities : DbContext
    {
        public DemoProjectEntities() : base("DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<webpages_Membership> webpages_Membership { get; set; }
        public DbSet<webpages_OAuthMembership> webpages_OAuthMembership { get; set; }
        public DbSet<webpages_Roles> webpages_Roles { get; set; }
        public DbSet<webpages_UsersInRoles> webpages_UsersInRoles { get; set; }
        public DbSet<Forms> Forms { get; set; }
        public DbSet<FormRoleMapping> FormRoleMapping { get; set; }
        public DbSet<ErrorLog> ErrorLog { get; set; }
        public DbSet<ActivityLog> ActivityLog { get; set; }
        public DbSet<Message_Mst> Message_Mst { get; set; }
        public DbSet<CommonLookup> CommonLookup { get; set; }
        public DbSet<Subject> Subject { get; set; }






    }
}
