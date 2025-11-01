using System.ComponentModel.DataAnnotations.Schema;
namespace MTA.Domain.Entities;

/// <summary>
/// Represents a support package in the MTA system that provides coaching services.
/// Packages define structured support offerings with specific credit allocations
/// that are spent when new support tickets are created.
/// </summary>
/// <remarks>
/// Support packages enable students to receive personalized coaching through a
/// ticket-based system. Each package includes a credit allowance and a duration
/// defined by administrators. Credits are only consumed when a ticket is opened,
/// and messaging within a ticket is unlimited until the ticket is closed.
/// </remarks>
public class Package : BaseEntity
{
    public required string Title { get; set; }

    public decimal Price { get; set; }

    /// <summary>
    /// Total credits that can be spent on creating tickets while the package is active.
    /// </summary>
    public int CreditCount { get; set; }

    /// <summary>
    /// Duration value that determines how long the package remains active after purchase.
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// Duration unit reference (e.g. Day, Month, Year).
    /// </summary>
    public int DurationUnitId { get; set; }

    [ForeignKey(nameof(DurationUnitId))]
    public virtual Lookup DurationUnit { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual ICollection<PackageHistory> PackageHistories { get; set; } = new List<PackageHistory>();
}
