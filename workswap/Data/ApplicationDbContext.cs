using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using workswap.Models;

namespace workswap.Data;

/// <summary>
/// Database context bridging our code to PostgreSQL.
/// Inherits from IdentityDbContext for built-in User/Role management.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Departments table - organizational units for grouping employees.
    /// </summary>
    public DbSet<Department> Departments { get; set; } = null!;

    /// <summary>
    /// Shifts table - work shifts that can be assigned to users.
    /// </summary>
    public DbSet<Shift> Shifts { get; set; } = null!;

    /// <summary>
    /// Shift offers table - shifts placed on the marketplace.
    /// </summary>
    public DbSet<ShiftOffer> ShiftOffers { get; set; } = null!;

    /// <summary>
    /// Swap requests table - direct trade requests between users.
    /// </summary>
    public DbSet<SwapRequest> SwapRequests { get; set; } = null!;

    /// <summary>
    /// Notifications table - system notifications for users.
    /// </summary>
    public DbSet<Notification> Notifications { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Department -> Employees (one-to-many)
        builder.Entity<Department>()
            .HasMany(d => d.Employees)
            .WithOne(u => u.Department)
            .HasForeignKey(u => u.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        // Department -> Shifts (one-to-many)
        builder.Entity<Department>()
            .HasMany(d => d.Shifts)
            .WithOne(s => s.Department)
            .HasForeignKey(s => s.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // User -> AssignedShifts (one-to-many)
        builder.Entity<ApplicationUser>()
            .HasMany(u => u.AssignedShifts)
            .WithOne(s => s.AssignedUser)
            .HasForeignKey(s => s.AssignedUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Index for efficient shift queries by date
        builder.Entity<Shift>()
            .HasIndex(s => s.Date);

        // Index for filtering shifts by department
        builder.Entity<Shift>()
            .HasIndex(s => s.DepartmentId);

        // ShiftOffer configurations
        builder.Entity<ShiftOffer>()
            .HasOne(so => so.Shift)
            .WithMany()
            .HasForeignKey(so => so.ShiftId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ShiftOffer>()
            .HasOne(so => so.OfferedBy)
            .WithMany()
            .HasForeignKey(so => so.OfferedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ShiftOffer>()
            .HasOne(so => so.ClaimedBy)
            .WithMany()
            .HasForeignKey(so => so.ClaimedById)
            .OnDelete(DeleteBehavior.Restrict);

        // SwapRequest configurations
        builder.Entity<SwapRequest>()
            .HasOne(sr => sr.SenderShift)
            .WithMany()
            .HasForeignKey(sr => sr.SenderShiftId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SwapRequest>()
            .HasOne(sr => sr.ReceiverShift)
            .WithMany()
            .HasForeignKey(sr => sr.ReceiverShiftId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SwapRequest>()
            .HasOne(sr => sr.Sender)
            .WithMany()
            .HasForeignKey(sr => sr.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SwapRequest>()
            .HasOne(sr => sr.Receiver)
            .WithMany()
            .HasForeignKey(sr => sr.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        // Notification configurations
        builder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
