using Microsoft.AspNetCore.Mvc;
using QuizApp.API.DTOs;
using QuizApp.API.Services;

namespace QuizApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService) => _authService = authService;

    /// <summary>Register a new user</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(dto.Username) || dto.Username.Length < 3)
            return BadRequest(new { message = "Username must be at least 3 characters." });

        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
            return BadRequest(new { message = "Password must be at least 6 characters." });

        var result = await _authService.RegisterAsync(dto);
        if (result == null)
            return Conflict(new { message = "Email already in use." });

        return Ok(result);
    }

    /// <summary>Login with email and password</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (result == null)
            return Unauthorized(new { message = "Invalid email or password." });

        return Ok(result);
    }
}
