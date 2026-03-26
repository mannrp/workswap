using workswap.DTOs;

namespace workswap.Services;

public interface ISwapService
{
    Task<IEnumerable<SwapRequestResponse>> GetMySwapsAsync(int userId);
    Task<SwapRequestResponse> CreateSwapAsync(int userId, CreateSwapDto dto);
    Task<bool> RespondToSwapAsync(int swapId, int userId, bool accepted);
}
