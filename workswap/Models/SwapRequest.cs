using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace workswap.Models;

public class SwapRequest
{
    public int Id { get; set; }

    // The shift the requester wants to GIVE AWAY
    public int SenderShiftId { get; set; }

    // The shift the requester wants to GET (optional if just giving away to specific person)
    public int? ReceiverShiftId { get; set; }

    public int SenderId { get; set; }
    public int ReceiverId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Pending, Accepted, Rejected, Cancelled
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    // Navigation
    [ForeignKey(nameof(SenderShiftId))]
    public Shift SenderShift { get; set; } = null!;

    [ForeignKey(nameof(ReceiverShiftId))]
    public Shift? ReceiverShift { get; set; }

    [ForeignKey(nameof(SenderId))]
    public ApplicationUser Sender { get; set; } = null!;

    [ForeignKey(nameof(ReceiverId))]
    public ApplicationUser Receiver { get; set; } = null!;
}