using System.ComponentModel.DataAnnotations;

namespace workswap.DTOs;

/// <summary>
/// Data Transfer Objects (DTOs) are used to move data between the API and the client.
/// We use them instead of our Database Models (Entities) to only expose what is necessary.
/// </summary>

public record RegisterRequest(
    [Required][EmailAddress] string Email,
    [Required][MinLength(6)] string Password,
    [Required] string FirstName,
    [Required] string LastName
);

public record LoginRequest(
    [Required][EmailAddress] string Email,
    [Required] string Password
);

public record AuthResponse(
    bool Success,
    string? Token = null,
    string? Error = null,
    DateTime? Expiration = null
);

public record UserInfoResponse(
    int Id,
    string Email,
    string FirstName,
    string LastName,
    string[] Roles
);
