using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using workswap.DTOs;
using workswap.Extensions;
using workswap.Services;

namespace workswap.Controllers;

/// <summary>
/// Endpoints for managing shift offers on the marketplace.
/// </summary>
[Authorize]
public class ShiftOffersController : ApiControllerBase
{
    private readonly IShiftOfferService _shiftOfferService;
    private readonly ILogger<ShiftOffersController> _logger;

    public ShiftOffersController(IShiftOfferService shiftOfferService, ILogger<ShiftOffersController> logger)
    {
        _shiftOfferService = shiftOfferService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all active shift offers, optionally filtered by department.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShiftOfferResponse>>> GetShiftOffers([FromQuery] int? departmentId = null)
    {
        var result = await _shiftOfferService.GetActiveOffersAsync(departmentId);
        return HandleResult(result);
    }

    /// <summary>
    /// Places a shift on the open marketplace for others to claim.
    /// </summary>
    [HttpPost("~/api/shifts/{shiftId}/offer")]
    public async Task<ActionResult<ShiftOfferResponse>> CreateOffer(int shiftId, [FromBody] CreateOfferDto dto)
    {
        var userId = User.GetUserId();
        var result = await _shiftOfferService.CreateOfferAsync(shiftId, userId, dto.ExpiresAt);
        
        if (result.IsSuccess && result.Value != null)
        {
            return CreatedAtAction(nameof(GetShiftOffers), new { id = result.Value.Id }, result.Value);
        }
        
        return HandleResult(result);
    }

    /// <summary>
    /// Claims an available shift offer from the marketplace.
    /// </summary>
    [HttpPost("{id}/claim")]
    public async Task<IActionResult> ClaimOffer(int id)
    {
        var userId = User.GetUserId();
        var result = await _shiftOfferService.ClaimOfferAsync(id, userId);
        
        if (result.IsSuccess)
        {
            return Ok(new { message = "Offer claimed successfully and shift transferred.", offer = result.Value });
        }
        
        return HandleResult(result);
    }
}