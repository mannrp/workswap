using Microsoft.EntityFrameworkCore;
using workswap.Common;
using workswap.Data;
using workswap.DTOs;
using workswap.Mapping;
using workswap.Models;

namespace workswap.Services;

/// <summary>
/// Implementation of IShiftService for core shift management.
/// </summary>
public class ShiftService : IShiftService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ShiftService> _logger;

    public ShiftService(ApplicationDbContext context, ILogger<ShiftService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<ShiftResponse>>> GetAllAsync(
        int? departmentId = null,
        int? userId = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        bool? availableForSwap = null)
    {
        var query = _context.Shifts
            .Include(s => s.Department)
            .Include(s => s.AssignedUser)
            .AsQueryable();

        if (departmentId.HasValue)
            query = query.Where(s => s.DepartmentId == departmentId.Value);

        if (userId.HasValue)
            query = query.Where(s => s.AssignedUserId == userId.Value);

        if (startDate.HasValue)
            query = query.Where(s => s.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(s => s.Date <= endDate.Value);

        if (availableForSwap.HasValue)
            query = query.Where(s => s.IsAvailableForSwap == availableForSwap.Value);

        var shifts = await query.OrderBy(s => s.Date).ThenBy(s => s.StartTime).ToListAsync();

        return Result<IEnumerable<ShiftResponse>>.Success(
            shifts.Select(s => s.ToResponse())
        );
    }

    public async Task<Result<ShiftResponse>> GetByIdAsync(int id)
    {
        var shift = await _context.Shifts
            .Include(s => s.Department)
            .Include(s => s.AssignedUser)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (shift == null)
            return Result<ShiftResponse>.NotFound($"Shift with ID {id} not found.");

        return Result<ShiftResponse>.Success(shift.ToResponse());
    }

    public async Task<Result<ShiftResponse>> CreateAsync(CreateShiftRequest request)
    {
        // Basic validation
        if (request.StartTime >= request.EndTime)
            return Result<ShiftResponse>.Failure("Start time must be before end time");

        var shift = new Shift
        {
            Date = request.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Notes = request.Notes,
            DepartmentId = request.DepartmentId,
            AssignedUserId = request.AssignedUserId,
            IsAvailableForSwap = request.IsAvailableForSwap
        };

        _context.Shifts.Add(shift);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Shift {ShiftId} created", shift.Id);

        // Reload for response
        var resultShift = await _context.Shifts
            .Include(s => s.Department)
            .Include(s => s.AssignedUser)
            .FirstAsync(s => s.Id == shift.Id);

        return Result<ShiftResponse>.Success(resultShift.ToResponse());
    }

    public async Task<Result<ShiftResponse>> UpdateAsync(int id, UpdateShiftRequest request)
    {
        var shift = await _context.Shifts
            .Include(s => s.Department)
            .Include(s => s.AssignedUser)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (shift == null)
            return Result<ShiftResponse>.NotFound($"Shift with ID {id} not found.");

        if (request.StartTime >= request.EndTime)
            return Result<ShiftResponse>.Failure("Start time must be before end time");

        shift.Date = request.Date;
        shift.StartTime = request.StartTime;
        shift.EndTime = request.EndTime;
        shift.Notes = request.Notes;
        shift.IsAvailableForSwap = request.IsAvailableForSwap;
        
        if (request.DepartmentId.HasValue)
            shift.DepartmentId = request.DepartmentId.Value;
            
        if (request.AssignedUserId.HasValue)
            shift.AssignedUserId = request.AssignedUserId.Value;

        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Shift {ShiftId} updated", id);

        return Result<ShiftResponse>.Success(shift.ToResponse());
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var shift = await _context.Shifts.FindAsync(id);

        if (shift == null)
            return Result.NotFound($"Shift with ID {id} not found.");

        _context.Shifts.Remove(shift);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Shift {ShiftId} deleted", id);

        return Result.Success();
    }
}
