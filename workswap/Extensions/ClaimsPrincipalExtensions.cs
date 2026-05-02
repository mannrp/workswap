using System.Security.Claims;

namespace workswap.Extensions;

/// <summary>
/// Extension methods for ClaimsPrincipal to simplify access to user information.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Safely extracts the user ID from the NameIdentifier or sub claim.
    /// Returns 0 if the claim is missing or invalid.
    /// </summary>
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var idClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("sub");
        
        if (idClaim == null || !int.TryParse(idClaim.Value, out int userId))
        {
            return 0;
        }

        return userId;
    }
}
