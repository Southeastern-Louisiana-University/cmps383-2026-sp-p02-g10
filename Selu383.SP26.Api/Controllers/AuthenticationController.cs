using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Selu383.SP26.Api.Features.Authentication;
using Selu383.SP26.Api.Features.Users;

namespace Selu383.SP26.Api.Controllers;

[Route("api/authentication")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthenticationController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }


    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto dto)
    {

        var user = await _userManager.FindByNameAsync(dto.UserName);
        if (user == null)
        {
     
            return BadRequest();
    }
        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
        {

            return BadRequest();

        }

        await _signInManager.SignInAsync(user, false);

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new UserDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            Roles = roles.ToArray()
        });
    }
    [HttpGet("me")]
    [Authorize] 
    public async Task<ActionResult<UserDto>> Me()
    {

        var username = User.Identity?.Name;
        if (username == null)
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
        {
            return Unauthorized(); 

        }

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new UserDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            Roles = roles.ToArray()
        });
    }
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok ();
    }
}