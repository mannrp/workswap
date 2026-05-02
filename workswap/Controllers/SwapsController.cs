using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using workswap.DTOs;
using workswap.Extensions;
using workswap.Services;

namespace workswap.Controllers;

/// <summary>
/// Endpoints for managing shift swap requests between users.
/// </summary>
[Authorize]
public class SwapsController : ApiControllerBase
{
    private readonly ISwapService _swapService;
    private readonly ILogger<SwapsController> _logger;

    public SwapsController(ISwapService swapService, ILogger<SwapsController> logger)
    {
        _swapService = swapService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all swap requests involving the current user (sent or received).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SwapRequestResponse>>> GetMySwaps()
    {
        var userId = User.GetUserId();
        var result = await _swapService.GetMySwapsAsync(userId);
        return HandleResult(result);
    }

    /// <summary>
    /// Creates a new shift swap request.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SwapRequestResponse>> CreateSwap([FromBody] CreateSwapDto dto)
    {
        var userId = User.GetUserId();
        var result = await _swapService.CreateSwapAsync(userId, dto);
        
        if (result.IsSuccess && result.Value != null)
        {
            return CreatedAtAction(nameof(GetMySwaps), new { id = result.Value.Id }, result.Value);
        }
        
        return HandleResult(result);
    }

    /// <summary>
    /// Accepts or rejects an incoming swap request.
    /// </summary>
    [HttpPut("{id}/respond")]
    public async Task<IActionResult> RespondToSwap(int id, [FromBody] SwapResponseDto dto)
    {
        var userId = User.GetUserId();
        var result = await _swapService.RespondToSwapAsync(id, userId, dto.Accepted);
        
        if (result.IsSuccess)
        {
            var message = dto.Accepted ? "Swap accepted and shifts transferred." : "Swap request rejected.";
            return Ok(new { message });
        }
        
        return HandleResult(result);
    }
}