using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Selu383.SP26.Api.Data;
using Selu383.SP26.Api.Features.Users;
using System.Text.RegularExpressions;

namespace Selu383.SP26.Api.Controllers;

[Route("api/users")]
[ApiController]
[Authorize(Roles = "Admin")]
public class UsersController(
    DataContext dataContext
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto dto)
    {
        // Validate username
        if (string.IsNullOrWhiteSpace(dto.UserName))
        {
            return BadRequest("Username is required");
        }

        // Check for duplicate username
        var existingUser = await dataContext.Set<User>()
            .FirstOrDefaultAsync(x => x.UserName == dto.UserName);
        if (existingUser != null)
        {
            return BadRequest("Username already exists");
        }

        // Validate password
        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest("Password is required");
        }

        if (!IsValidPassword(dto.Password))
        {
            return BadRequest("Password must be at least 8 characters and contain uppercase, lowercase, digit, and special character");
        }

        // Validate roles
        if (dto.Roles == null || dto.Roles.Length == 0)
        {
            return BadRequest("At least one role is required");
        }

        var roles = new List<Role>();
        var roleNames = dto.Roles.Distinct().ToList();
        var existingRoles = await dataContext.Set<Role>()
            .Where(x => roleNames.Contains(x.Name))
            .ToListAsync();

        if (existingRoles.Count != roleNames.Count)
        {
            return BadRequest("One or more roles do not exist");
        }

        roles = existingRoles;

        // Create user
        var user = new User
        {
            UserName = dto.UserName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Roles = roles
        };

        dataContext.Set<User>().Add(user);
        await dataContext.SaveChangesAsync();

        return Ok(new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Roles = user.Roles.Select(x => x.Name).ToArray()
        });
    }

    private static bool IsValidPassword(string password)
    {
        if (password.Length < 8)
            return false;

        return Regex.IsMatch(password, @"[a-z]") &&  // lowercase
               Regex.IsMatch(password, @"[A-Z]") &&  // uppercase
               Regex.IsMatch(password, @"\d") &&     // digit
               Regex.IsMatch(password, @"[\W_]");    // special character
    }
}
