using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace workswap.Models;

/// <summary>
/// Custom User class that extends ASP.NET Identity.
/// Uses integer IDs (1, 2, 3...) for readability instead of GUIDs.
/// </summary>
public class ApplicationUser : IdentityUser<int>
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// The department this user belongs to. Null if unassigned.
    /// </summary>
    public int? DepartmentId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties

    /// <summary>
    /// The department entity for this user.
    /// </summary>
    [ForeignKey(nameof(DepartmentId))]
    public Department? Department { get; set; }

    /// <summary>
    /// Collection of shifts assigned to this user.
    /// </summary>
    public ICollection<Shift> AssignedShifts { get; set; } = new List<Shift>();
}
