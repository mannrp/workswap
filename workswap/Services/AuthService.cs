using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using workswap.Common;
using workswap.DTOs;
using workswap.Models;

namespace workswap.Services;

/// <summary>
/// Implementation of IAuthService using ASP.NET Core Identity and JWT.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Result<AuthResponse>.Failure("User with this email already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var firstError = result.Errors.FirstOrDefault()?.Description ?? "Registration failed.";
            return Result<AuthResponse>.Failure(firstError);
        }

        // Add to default role "Employee"
        await _userManager.AddToRoleAsync(user, "Employee");

        var token = GenerateJwtToken(user, new List<string> { "Employee" });
        
        _logger.LogInformation("User {Email} registered successfully", request.Email);

        return Result<AuthResponse>.Success(new AuthResponse(true, Token: token, Expiration: GetTokenExpiration()));
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result<AuthResponse>.Unauthorized("Invalid credentials.");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            return Result<AuthResponse>.Unauthorized("Invalid credentials.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles.ToList());
        
        _logger.LogInformation("User {Email} logged in successfully", request.Email);

        return Result<AuthResponse>.Success(new AuthResponse(true, Token: token, Expiration: GetTokenExpiration()));
    }

    private DateTime GetTokenExpiration()
    {
        var duration = double.Parse(_configuration["JWT_DURATION_MINUTES"] ?? "60");
        return DateTime.UtcNow.AddMinutes(duration);
    }

    private string GenerateJwtToken(ApplicationUser user, List<string> roles)
    {
        var jwtSecret = _configuration["JWT_SECRET"];
        if (string.IsNullOrEmpty(jwtSecret)) throw new InvalidOperationException("JWT_SECRET is missing!");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("firstName", user.FirstName),
            new Claim("lastName", user.LastName)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = GetTokenExpiration(),
            Issuer = _configuration["JWT_ISSUER"],
            Audience = _configuration["JWT_AUDIENCE"],
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
