using System.ComponentModel.DataAnnotations;

namespace DemoProject.Model
{
    public partial class webpages_OAuthMembership
    {
        public string Provider { get; set; }
        public string ProviderUserId { get; set; }
        [Key]
        public int UserId { get; set; }
    }
}