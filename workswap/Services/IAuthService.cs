using workswap.Common;
using workswap.DTOs;

namespace workswap.Services;

/// <summary>
/// Service for user authentication, including registration and login.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user.
    /// </summary>
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
}
