using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using workswap.DTOs;
using workswap.Services;

namespace workswap.Controllers;

/// <summary>
/// Endpoints for managing shifts.
/// </summary>
[Authorize]
public class ShiftsController : ApiControllerBase
{
    private readonly IShiftService _shiftService;
    private readonly ILogger<ShiftsController> _logger;

    public ShiftsController(IShiftService shiftService, ILogger<ShiftsController> logger)
    {
        _shiftService = shiftService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all shifts matching the specified criteria.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShiftResponse>>> GetAll(
        [FromQuery] int? departmentId = null,
        [FromQuery] int? userId = null,
        [FromQuery] DateOnly? startDate = null,
        [FromQuery] DateOnly? endDate = null,
        [FromQuery] bool? availableForSwap = null)
    {
        var result = await _shiftService.GetAllAsync(departmentId, userId, startDate, endDate, availableForSwap);
        return HandleResult(result);
    }

    /// <summary>
    /// Retrieves a specific shift by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ShiftResponse>> GetById(int id)
    {
        var result = await _shiftService.GetByIdAsync(id);
        return HandleResult(result);
    }

    /// <summary>
    /// Creates a new shift.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ShiftResponse>> Create(CreateShiftRequest request)
    {
        var result = await _shiftService.CreateAsync(request);
        
        if (result.IsSuccess && result.Value != null)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }
        
        return HandleResult(result);
    }

    /// <summary>
    /// Updates an existing shift.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ShiftResponse>> Update(int id, UpdateShiftRequest request)
    {
        var result = await _shiftService.UpdateAsync(id, request);
        return HandleResult(result);
    }

    /// <summary>
    /// Deletes a shift.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _shiftService.DeleteAsync(id);
        return HandleResult(result);
    }
}
