using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MTA.Domain.Entities;

/// <summary>
/// Represents a purchased package instance for an account and tracks credit usage.
/// </summary>
public class PackageHistory : BaseEntity
{
    public DateTime ExpiredDate { get; set; }

    /// <summary>
    /// Total credits granted when the package history was created.
    /// </summary>
    public int TotalCredits { get; set; }

    /// <summary>
    /// Remaining credits that can still be used to open tickets.
    /// </summary>
    public int RemainingCredits { get; set; }

    public decimal PurchasePrice { get; set; }

    public int PackageId { get; set; }
    [ForeignKey("PackageId")]
    public virtual Package Package { get; set; } = null!;

    public int AccountId { get; set; }
    [ForeignKey("AccountId")]
    public virtual Account Account { get; set; } = null!;
}
