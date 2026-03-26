using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace workswap.Models;

public class ShiftOffer
{
    public int Id { get; set; }

    public int ShiftId { get; set; }

    // Who offered it
    public int OfferedById { get; set; }

    // Who claimed it (optional, until claimed)
    public int? ClaimedById { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }

    // Active, Claimed, Expired, Cancelled
    [MaxLength(20)]
    public string Status { get; set; } = "Active";

    // Navigation
    [ForeignKey(nameof(ShiftId))]
    public Shift Shift { get; set; } = null!;

    [ForeignKey(nameof(OfferedById))]
    public ApplicationUser OfferedBy { get; set; } = null!;

    [ForeignKey(nameof(ClaimedById))]
    public ApplicationUser? ClaimedBy { get; set; }
}