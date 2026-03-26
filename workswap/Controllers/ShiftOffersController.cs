using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using workswap.DTOs;
using workswap.Services;

namespace workswap.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShiftOffersController : ControllerBase
{
    private readonly IShiftOfferService _shiftOfferService;
    private readonly ILogger<ShiftOffersController> _logger;

    public ShiftOffersController(IShiftOfferService shiftOfferService, ILogger<ShiftOffersController> logger)
    {
        _shiftOfferService = shiftOfferService;
        _logger = logger;
    }

    // GET: api/shiftoffers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShiftOfferResponse>>> GetShiftOffers([FromQuery] int? departmentId = null)
    {
        var offers = await _shiftOfferService.GetActiveOffersAsync(departmentId);
        return Ok(offers);
    }

    // POST: api/shifts/{shiftId}/offer
    [HttpPost("~/api/shifts/{shiftId}/offer")]
    public async Task<ActionResult<ShiftOfferResponse>> CreateOffer(int shiftId, [FromBody] CreateOfferDto dto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            var expiresAt = dto.ExpiresAt ?? DateTime.UtcNow.AddDays(7);
            var offer = await _shiftOfferService.CreateOfferAsync(shiftId, userId, expiresAt);
            return CreatedAtAction(nameof(GetShiftOffers), new { id = offer.Id }, offer);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST: api/shiftoffers/{id}/claim
    [HttpPost("{id}/claim")]
    public async Task<IActionResult> ClaimOffer(int id)
    {
        try
        {
            var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            var offer = await _shiftOfferService.ClaimOfferAsync(id, userId);

            if (offer == null)
            {
                return NotFound(new { message = "Offer not found" });
            }

            return Ok(new { message = "Offer claimed successfully", offer });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}