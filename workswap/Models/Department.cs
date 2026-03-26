using System.ComponentModel.DataAnnotations;

namespace workswap.Models;

/// <summary>
/// Represents a department within the organization.
/// Employees are assigned to departments, and shifts are scheduled per department.
/// </summary>
public class Department
{
    /// <summary>
    /// Primary key for the department.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The display name of the department (e.g., "Kitchen", "Front Desk").
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description providing more details about the department.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Timestamp when the department was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties

    /// <summary>
    /// Collection of employees who belong to this department.
    /// </summary>
    public ICollection<ApplicationUser> Employees { get; set; } = new List<ApplicationUser>();

    /// <summary>
    /// Collection of shifts scheduled for this department.
    /// </summary>
    public ICollection<Shift> Shifts { get; set; } = new List<Shift>();
}
