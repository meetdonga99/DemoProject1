using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<webpages_Membership> webpages_Membership { get; set; }
        public DbSet<webpages_OAuthMembership> webpages_OAuthMembership { get; set; }
        public DbSet<webpages_Roles> webpages_Roles { get; set; }
        public DbSet<webpages_UsersInRoles> webpages_UsersInRoles { get; set; }
    }
}
