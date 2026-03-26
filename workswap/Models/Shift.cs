using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace workswap.Models;

/// <summary>
/// Represents a work shift that can be assigned to an employee.
/// Shifts belong to a department and have a specific date and time range.
/// </summary>
public class Shift
{
    /// <summary>
    /// Primary key for the shift.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The date of the shift (without time component).
    /// </summary>
    [Required]
    public DateOnly Date { get; set; }

    /// <summary>
    /// The start time of the shift.
    /// </summary>
    [Required]
    public TimeOnly StartTime { get; set; }

    /// <summary>
    /// The end time of the shift.
    /// </summary>
    [Required]
    public TimeOnly EndTime { get; set; }

    /// <summary>
    /// Optional notes or description for this shift.
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Indicates if this shift is available for swap/pickup.
    /// </summary>
    public bool IsAvailableForSwap { get; set; } = false;

    /// <summary>
    /// Timestamp when the shift was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign keys

    /// <summary>
    /// The department this shift belongs to.
    /// </summary>
    [Required]
    public int DepartmentId { get; set; }

    /// <summary>
    /// The user assigned to this shift. Null if unassigned.
    /// </summary>
    public int? AssignedUserId { get; set; }

    // Navigation properties

    /// <summary>
    /// The department entity for this shift.
    /// </summary>
    [ForeignKey(nameof(DepartmentId))]
    public Department Department { get; set; } = null!;

    /// <summary>
    /// The user assigned to this shift.
    /// </summary>
    [ForeignKey(nameof(AssignedUserId))]
    public ApplicationUser? AssignedUser { get; set; }
}
