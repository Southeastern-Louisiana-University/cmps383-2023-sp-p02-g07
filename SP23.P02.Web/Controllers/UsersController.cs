using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SP23.P02.Web.Features.Roles;
using SP23.P02.Web.Features.Users;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace SP23.P02.Web.Controllers

{


    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        public UsersController(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpPost]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult<UserDto>> Create(CreateUserDto createDto)
        {
            if (createDto.UserName == null)
            {
                return BadRequest();
            }

            if (!createDto.Roles.Any())
            {
                return BadRequest("ERROR: There should be at least one role provided. ");
            }

            var allRoles = await roleManager.Roles.Select(x => x.Name).ToListAsync();
            foreach (var role in createDto.Roles)
            {
                if (!allRoles.Contains(role))
                {
                    return BadRequest($"ERROR: This role '{role}' does not exist.");
                }
            }

            var alreadyexistingUser = await userManager.FindByNameAsync(createDto.UserName);
            if (alreadyexistingUser != null)
            {
                return BadRequest("ERROR: This username is already taken. Please create a different one.");
            }
            // above: checks that the username isnt already taken. alerts error if it is
            var newUser = new User
            {
                UserName = createDto.UserName,
            };

            var createResult = await userManager.CreateAsync(newUser, createDto.Password);
            if (!createResult.Succeeded)
            {
                return BadRequest();
            }
            // password creation validation above




            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            if (createDto.Roles.IsNullOrEmpty())
            {
                return BadRequest();
            }

            try
            {
                var roleResult = await userManager.AddToRolesAsync(newUser, createDto.Roles);
                if (!roleResult.Succeeded)
                {
                    return BadRequest();
                }
            }
            catch (InvalidOperationException e) when (e.Message.StartsWith("Role") && e.Message.EndsWith("does not exist."))
            {
                return BadRequest();
            }


            return Ok(new UserDto
            {
                Id = newUser.Id,
                Roles = createDto.Roles,
                UserName = newUser.UserName,
            });

            

        }
    }
}






