using System;
namespace MTA.Domain.Entities;

/// <summary>
/// Represents a support package in the MTA system that provides coaching services.
/// Packages define structured support offerings with specific credit allocations
/// that are spent when new support tickets are created.
/// </summary>
/// <remarks>
/// Support packages enable students to receive personalized coaching through a
/// ticket-based system. Each package includes a credit allowance and an expiration
/// date defined by administrators. Credits are only consumed when a ticket is opened,
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
    /// Date when this package expires and can no longer be used to create new tickets.
    /// </summary>
    public DateTime ExpirationDate { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual ICollection<PackageHistory> PackageHistories { get; set; } = new List<PackageHistory>();
}
