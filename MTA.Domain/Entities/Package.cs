using System.ComponentModel.DataAnnotations.Schema;

namespace MTA.Domain.Entities;

/// <summary>
/// Represents a support package in the MTA system that provides coaching services.
/// Packages define structured support offerings with specific allocations of tickets
/// and messages for personalized tennis coaching and guidance.
/// </summary>
/// <remarks>
/// Support packages enable students to receive personalized coaching through a
/// ticket-based system. Each package includes a specific number of support tickets
/// and messages, valid for a defined duration. This provides flexible coaching
/// options beyond standard course content.
/// </remarks>
public class Package : BaseEntity
{
    public required string Title { get; set; }
    
    public decimal Price { get; set; }
    
    public int TicketCount { get; set; }
    
    public int MessageCount { get; set; }
    
    public int Duration { get; set; }

    public int DurationUnitId { get; set; }
    
    [ForeignKey("DurationUnitId")]
    public virtual Lookup DurationUnit { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    
    public virtual ICollection<PackageHistory> PackageHistories { get; set; } = new List<PackageHistory>();
}
