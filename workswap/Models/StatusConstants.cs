namespace workswap.Models;

/// <summary>
/// Status constants for swap requests.
/// Using const strings instead of enums to avoid needing EF value converters.
/// </summary>
public static class SwapStatus
{
    public const string Pending = "Pending";
    public const string Completed = "Completed";
    public const string Rejected = "Rejected";
    public const string Cancelled = "Cancelled";
}

/// <summary>
/// Status constants for shift offers.
/// </summary>
public static class OfferStatus
{
    public const string Active = "Active";
    public const string Claimed = "Claimed";
    public const string Expired = "Expired";
    public const string Cancelled = "Cancelled";
}
