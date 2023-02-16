using Microsoft.AspNetCore.Identity;

namespace SP23.P02.Web.Features.Users;

public class User : IdentityUser<int>
{
    public ICollection<UserRole.UserRole> Roles { get; set; } = new List<UserRole.UserRole>();
}