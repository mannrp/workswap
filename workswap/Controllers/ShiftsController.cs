using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using workswap.DTOs;
using workswap.Services;

namespace workswap.Controllers;

/// <summary>
/// CRUD operations for managing shifts.
/// All endpoints require authentication.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ShiftsController : ControllerBase
{
    private readonly IShiftService _shiftService;
    private readonly ILogger<ShiftsController> _logger;

    public ShiftsController(IShiftService shiftService, ILogger<ShiftsController> logger)
    {
        _shiftService = shiftService;
        _logger = logger;
    }

    /// <summary>
    /// Get shifts with optional filters for department, date range, and user.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShiftResponse>>> GetAll(
        [FromQuery] int? departmentId = null,
        [FromQuery] int? userId = null,
        [FromQuery] DateOnly? startDate = null,
        [FromQuery] DateOnly? endDate = null,
        [FromQuery] bool? availableForSwap = null)
    {
        var shifts = await _shiftService.GetAllAsync(departmentId, userId, startDate, endDate, availableForSwap);
        return Ok(shifts);
    }

    /// <summary>
    /// Get a specific shift by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ShiftResponse>> GetById(int id)
    {
        var shift = await _shiftService.GetByIdAsync(id);

        if (shift == null)
        {
            return NotFound(new { message = $"Shift with ID {id} not found." });
        }

        return Ok(shift);
    }

    /// <summary>
    /// Create a new shift.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ShiftResponse>> Create(CreateShiftRequest request)
    {
        try
        {
            var shift = await _shiftService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = shift.Id }, shift);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing shift.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ShiftResponse>> Update(int id, UpdateShiftRequest request)
    {
        try
        {
            var shift = await _shiftService.UpdateAsync(id, request);

            if (shift == null)
            {
                return NotFound(new { message = $"Shift with ID {id} not found." });
            }

            return Ok(shift);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a shift.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _shiftService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new { message = $"Shift with ID {id} not found." });
        }

        return NoContent();
    }
}
