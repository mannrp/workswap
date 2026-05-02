using workswap.Common;
using workswap.DTOs;

namespace workswap.Services;

/// <summary>
/// Service for managing shift swap requests between users.
/// </summary>
public interface ISwapService
{
    /// <summary>
    /// Retrieves all swap requests involving the specified user.
    /// </summary>
    Task<Result<IEnumerable<SwapRequestResponse>>> GetMySwapsAsync(int userId);

    /// <summary>
    /// Creates a new shift swap request.
    /// </summary>
    Task<Result<SwapRequestResponse>> CreateSwapAsync(int userId, CreateSwapDto dto);

    /// <summary>
    /// Responds to an incoming swap request.
    /// </summary>
    Task<Result> RespondToSwapAsync(int swapId, int userId, bool accepted);
}
