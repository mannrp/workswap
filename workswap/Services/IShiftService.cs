using workswap.DTOs;

namespace workswap.Services;

public interface IShiftService
{
    Task<IEnumerable<ShiftResponse>> GetAllAsync(
        int? departmentId = null,
        int? userId = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        bool? availableForSwap = null);
    
    Task<ShiftResponse?> GetByIdAsync(int id);
    Task<ShiftResponse> CreateAsync(CreateShiftRequest request);
    Task<ShiftResponse?> UpdateAsync(int id, UpdateShiftRequest request);
    Task<bool> DeleteAsync(int id);
}
