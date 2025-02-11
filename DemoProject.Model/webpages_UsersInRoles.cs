using System.ComponentModel.DataAnnotations;

namespace DemoProject.Model
{
    public class webpages_UsersInRoles
    {
        [Key]
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}