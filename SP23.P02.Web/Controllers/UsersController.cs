using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SP23.P02.Web.Features.Users;

namespace SP23.P02.Web.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase {
    private readonly UserManager<User> userManager;

    public UsersController(UserManager<User> userManager) {
        this.userManager = userManager;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createDto) {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        if (createDto.Roles.IsNullOrEmpty()) {
            return BadRequest();
        }

        var newUser = new User {
            UserName = createDto.UserName,
        };

        var createResult = await userManager.CreateAsync(newUser, createDto.Password);

        if (!createResult.Succeeded) {
            return BadRequest();
        }

        try {
            var roleResult = await userManager.AddToRolesAsync(newUser, createDto.Roles);
            if (!roleResult.Succeeded) {
                return BadRequest();
            }
        }
        catch (InvalidOperationException e) when (e.Message.StartsWith("Role") && e.Message.EndsWith("does not exist.")) {
            return BadRequest();
        }

        transaction.Complete();

        return Ok(new UserDto {
            Id = newUser.Id,
            Roles = createDto.Roles,
            UserName = newUser.UserName,
        });
    }
}