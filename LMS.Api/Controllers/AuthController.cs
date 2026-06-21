using LMS.Infrastructure.Data;
using LMS.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthController(ApplicationDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.PasswordHash == request.Password); // Simplified pass check
        if (user == null || !user.IsActive)
            return Unauthorized("Invalid credentials or inactive user.");

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var user = await _context.Users.Include(u => u.EmployeeProfile).FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return NotFound();

        return Ok(new
        {
            user.Id,
            user.Email,
            Role = user.Role.ToString(),
            Profile = user.EmployeeProfile != null ? new
            {
                user.EmployeeProfile.EmployeeCode,
                user.EmployeeProfile.FirstName,
                user.EmployeeProfile.LastName
            } : null
        });
    }
}

public class LoginRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
