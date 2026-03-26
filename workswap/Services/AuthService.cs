using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using workswap.DTOs;
using workswap.Models;

namespace workswap.Services;

/// <summary>
/// The AuthService handles the actual logic of registering and logging in users.
/// It interacts with ASP.NET Identity's UserManager and generates JWT tokens.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return new AuthResponse(false, Error: "User with this email already exists.");
        }

        // Create the user object
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        // Attempt to create user in database
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var firstError = result.Errors.FirstOrDefault()?.Description ?? "Registration failed.";
            return new AuthResponse(false, Error: firstError);
        }

        // Add to default role "Employee" (we'll implement roles properly in Step 7)
        // For now, let's just generate the token
        var token = GenerateJwtToken(user, new List<string> { "Employee" });

        return new AuthResponse(true, Token: token, Expiration: DateTime.UtcNow.AddMinutes(60));
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return new AuthResponse(false, Error: "Invalid credentials.");
        }

        // Check password
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            return new AuthResponse(false, Error: "Invalid credentials.");
        }

        // Get user roles (we'll need these for the token)
        var roles = await _userManager.GetRolesAsync(user);

        var token = GenerateJwtToken(user, roles.ToList());

        return new AuthResponse(true, Token: token, Expiration: DateTime.UtcNow.AddMinutes(60));
    }

    private string GenerateJwtToken(ApplicationUser user, List<string> roles)
    {
        var jwtSecret = _configuration["JWT_SECRET"];
        if (string.IsNullOrEmpty(jwtSecret)) throw new Exception("JWT_SECRET is missing!");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Claims are pieces of info about the user encoded in the token
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("firstName", user.FirstName),
            new Claim("lastName", user.LastName)
        };

        // Add role claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JWT_DURATION_MINUTES"] ?? "60")),
            Issuer = _configuration["JWT_ISSUER"],
            Audience = _configuration["JWT_AUDIENCE"],
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
