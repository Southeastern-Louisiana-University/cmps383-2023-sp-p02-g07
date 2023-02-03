using System.ComponentModel.DataAnnotations;

namespace SP23.P02.Web.Features.Users;

public class CreateUserDto
{
    [Required]
    public string UserName { get; set; } = string.Empty;
    [Required]
    public string[] Roles { get; set; } = Array.Empty<string>();
    [Required]
    public string Password { get; set; } = string.Empty;
}