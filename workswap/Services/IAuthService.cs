using workswap.DTOs;

namespace workswap.Services;

/// <summary>
/// This interface defines what our Auth Service must do.
/// We use interfaces for better testing and "Loose Coupling".
/// </summary>
public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}
