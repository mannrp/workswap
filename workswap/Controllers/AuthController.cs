using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using workswap.DTOs;
using workswap.Services;

namespace workswap.Controllers;

/// <summary>
/// The AuthController is the entry point for authentication requests.
/// It uses the AuthService to perform registration and login.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// This is a protected endpoint. 
    /// Only logged-in users with a valid JWT token can access it.
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public ActionResult<UserInfoResponse> GetMe()
    {
        // We get the user info from the Claims in the token
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var firstName = User.FindFirstValue("firstName");
        var lastName = User.FindFirstValue("lastName");
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();

        if (userId == null) return Unauthorized();

        return Ok(new UserInfoResponse(
            int.Parse(userId),
            email ?? "",
            firstName ?? "",
            lastName ?? "",
            roles
        ));
    }
}
