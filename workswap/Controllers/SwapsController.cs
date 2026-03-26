using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using workswap.DTOs;
using workswap.Services;

namespace workswap.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SwapsController : ControllerBase
{
    private readonly ISwapService _swapService;
    private readonly ILogger<SwapsController> _logger;

    public SwapsController(ISwapService swapService, ILogger<SwapsController> logger)
    {
        _swapService = swapService;
        _logger = logger;
    }

    // GET: api/swaps
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SwapRequestResponse>>> GetMySwaps()
    {
        var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
        var swaps = await _swapService.GetMySwapsAsync(userId);
        return Ok(swaps);
    }

    // POST: api/swaps
    [HttpPost]
    public async Task<ActionResult<SwapRequestResponse>> CreateSwap([FromBody] CreateSwapDto dto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            var swap = await _swapService.CreateSwapAsync(userId, dto);
            return CreatedAtAction(nameof(GetMySwaps), new { id = swap.Id }, swap);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/swaps/{id}/respond
    [HttpPut("{id}/respond")]
    public async Task<IActionResult> RespondToSwap(int id, [FromBody] SwapResponseDto dto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            await _swapService.RespondToSwapAsync(id, userId, dto.Accepted);
            
            var message = dto.Accepted ? "Swap accepted" : "Swap rejected";
            return Ok(new { message });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}