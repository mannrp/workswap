using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using workswap.DTOs;
using workswap.Extensions;
using workswap.Services;

namespace workswap.Controllers;

/// <summary>
/// Endpoints for user authentication and profile information.
/// </summary>
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return HandleResult(result);
    }

    /// <summary>
    /// Authenticates a user and returns a access token.
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves the profile information for the currently authenticated user.
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public ActionResult<UserInfoResponse> GetMe()
    {
        var userId = User.GetUserId();
        if (userId == 0) return Unauthorized();

        var email = User.FindFirstValue(ClaimTypes.Email);
        var firstName = User.FindFirstValue("firstName");
        var lastName = User.FindFirstValue("lastName");
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();

        return Ok(new UserInfoResponse(
            userId,
            email ?? "",
            firstName ?? "",
            lastName ?? "",
            roles
        ));
    }
}
