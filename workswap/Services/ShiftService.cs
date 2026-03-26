using Microsoft.EntityFrameworkCore;
using workswap.Data;
using workswap.DTOs;
using workswap.Models;
using workswap.Mapping;

namespace workswap.Services;

public class ShiftService : IShiftService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ShiftService> _logger;

    public ShiftService(ApplicationDbContext context, ILogger<ShiftService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ShiftResponse>> GetAllAsync(
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

        // Apply filters
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

        var shifts = await query
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .ToListAsync();

        return shifts.Select(s => s.ToResponse());
    }

    public async Task<ShiftResponse?> GetByIdAsync(int id)
    {
        var shift = await _context.Shifts
            .Include(s => s.Department)
            .Include(s => s.AssignedUser)
            .FirstOrDefaultAsync(s => s.Id == id);

        return shift?.ToResponse();
    }

    public async Task<ShiftResponse> CreateAsync(CreateShiftRequest request)
    {
        // Validate department exists
        var department = await _context.Departments.FindAsync(request.DepartmentId);
        if (department == null)
        {
            throw new ArgumentException($"Department with ID {request.DepartmentId} not found.");
        }

        // Validate user exists if provided
        ApplicationUser? assignedUser = null;
        if (request.AssignedUserId.HasValue)
        {
            assignedUser = await _context.Users.FindAsync(request.AssignedUserId.Value);
            if (assignedUser == null)
            {
                throw new ArgumentException($"User with ID {request.AssignedUserId} not found.");
            }
        }

        // Validate time range
        if (request.EndTime <= request.StartTime)
        {
            throw new ArgumentException("End time must be after start time.");
        }

        var shift = new Shift
        {
            Date = request.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            DepartmentId = request.DepartmentId,
            AssignedUserId = request.AssignedUserId,
            Notes = request.Notes
        };

        _context.Shifts.Add(shift);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Shift {ShiftId} created for department {DepartmentId}", shift.Id, shift.DepartmentId);

        // Reload with navigation properties
        await _context.Entry(shift).Reference(s => s.Department).LoadAsync();
        if (shift.AssignedUserId.HasValue)
        {
            await _context.Entry(shift).Reference(s => s.AssignedUser).LoadAsync();
        }

        return shift.ToResponse();
    }

    public async Task<ShiftResponse?> UpdateAsync(int id, UpdateShiftRequest request)
    {
        var shift = await _context.Shifts
            .Include(s => s.Department)
            .Include(s => s.AssignedUser)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (shift == null)
        {
            return null;
        }

        // Validate department exists
        var department = await _context.Departments.FindAsync(request.DepartmentId);
        if (department == null)
        {
            throw new ArgumentException($"Department with ID {request.DepartmentId} not found.");
        }

        // Validate user exists if provided
        if (request.AssignedUserId.HasValue)
        {
            var assignedUser = await _context.Users.FindAsync(request.AssignedUserId.Value);
            if (assignedUser == null)
            {
                throw new ArgumentException($"User with ID {request.AssignedUserId} not found.");
            }
        }

        // Validate time range
        if (request.EndTime <= request.StartTime)
        {
            throw new ArgumentException("End time must be after start time.");
        }

        // Update fields
        shift.Date = request.Date;
        shift.StartTime = request.StartTime;
        shift.EndTime = request.EndTime;
        shift.DepartmentId = request.DepartmentId;
        shift.AssignedUserId = request.AssignedUserId;
        shift.Notes = request.Notes;
        shift.IsAvailableForSwap = request.IsAvailableForSwap;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Shift {ShiftId} updated", shift.Id);

        // Reload navigation properties if department changed
        await _context.Entry(shift).Reference(s => s.Department).LoadAsync();
        if (shift.AssignedUserId.HasValue)
        {
            await _context.Entry(shift).Reference(s => s.AssignedUser).LoadAsync();
        }

        return shift.ToResponse();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var shift = await _context.Shifts.FindAsync(id);

        if (shift == null)
        {
            return false;
        }

        _context.Shifts.Remove(shift);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Shift {ShiftId} deleted", id);

        return true;
    }
}
