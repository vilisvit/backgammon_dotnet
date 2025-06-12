using System.Security.Claims;
using Backgammon.Core.Entities;
using Backgammon.WebAPI.Dtos.Auth;
using Backgammon.WebAPI.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backgammon.WebAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    JwtTokenService tokenService
) : ControllerBase
{

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto requestDto)
    {
        var user = await userManager.FindByNameAsync(requestDto.UserName);
        if (user == null || !await userManager.CheckPasswordAsync(user, requestDto.Password))
            return Unauthorized("Invalid credentials");

        var token = tokenService.GenerateToken(user.Id);
        return Ok(new AuthResponseDto(token));
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto requestDto)
    {
        if (await userManager.FindByNameAsync(requestDto.UserName) != null)
            return Conflict("Username is taken!");

        var user = new User
        {
            UserName = requestDto.UserName
        };

        var result = await userManager.CreateAsync(user, requestDto.Password);
        Console.WriteLine(result);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("User registered successfully!");
    }

    [HttpGet("whoami")]
    public async Task<IActionResult> WhoAmI()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var user = await userManager.FindByIdAsync(userIdClaim.Value);
        if (user == null)
            return Unauthorized();

        return Ok(new { username = user.UserName });
    }
}