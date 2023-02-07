using Microsoft.AspNetCore.Identity;
using SP23.P02.Web.Features.Roles;
using SP23.P02.Web.Features.Users;

namespace SP23.P02.Web.Features.UserRole
{
    public class UserRole : IdentityUserRole<int>
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
