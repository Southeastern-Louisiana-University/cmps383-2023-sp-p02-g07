using Microsoft.AspNetCore.Identity;

namespace SP23.P02.Web.Features.Roles;

public class Role : IdentityRole<int>
{
    public ICollection<UserRole.UserRole> Users { get; } = new List<UserRole.UserRole>();
}