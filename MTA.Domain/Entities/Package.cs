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
    /// <summary>
    /// Gets or sets the title of the support package.
    /// This is the display name used for marketing and identification.
    /// </summary>
    /// <value>
    /// A non-empty string containing the package title.
    /// This field is required and should clearly describe the package offering.
    /// </value>
    /// <remarks>
    /// Examples: "Basic Support Package", "Premium Coaching Bundle", "Advanced Training Support"
    /// </remarks>
    public required string Title { get; set; }
    
    /// <summary>
    /// Gets or sets the price of the package in the system's base currency.
    /// This determines the cost for students to purchase the support package.
    /// </summary>
    /// <value>
    /// A decimal value representing the package price.
    /// Should be a positive value reflecting the package's value and market positioning.
    /// </value>
    public decimal Price { get; set; }
    
    /// <summary>
    /// Gets or sets the number of support tickets included in this package.
    /// Each ticket represents one support request or coaching session.
    /// </summary>
    /// <value>
    /// An integer representing the total number of tickets available in the package.
    /// Should be a positive value defining the package's support capacity.
    /// </value>
    public int TicketCount { get; set; }
    
    /// <summary>
    /// Gets or sets the number of messages included in this package.
    /// Messages represent communication exchanges within support tickets.
    /// </summary>
    /// <value>
    /// An integer representing the total number of messages available in the package.
    /// Should be a positive value defining the communication capacity.
    /// </value>
    public int MessageCount { get; set; }
    
    /// <summary>
    /// Gets or sets the duration value for package validity.
    /// This works with DurationUnit to define the total package lifespan.
    /// </summary>
    /// <value>
    /// An integer representing the duration value (e.g., 3 for "3 months").
    /// Should be a positive value defining the package's active period.
    /// </value>
    public int Duration { get; set; }

    /// <summary>
    /// Gets or sets the foreign key reference to the duration unit lookup.
    /// This defines the time unit for the package duration (days, weeks, months, etc.).
    /// </summary>
    /// <value>
    /// An integer representing the ID of the duration unit lookup value.
    /// This field is required and must reference a valid duration unit.
    /// </value>
    public int DurationUnitId { get; set; }
    
    /// <summary>
    /// Gets or sets the navigation property to the duration unit lookup entity.
    /// This provides the time unit information for the package duration.
    /// </summary>
    /// <value>
    /// A Lookup entity containing the duration unit details (e.g., "Days", "Months").
    /// This property is populated by Entity Framework through the foreign key relationship.
    /// </value>
    [ForeignKey("DurationUnitId")]
    public virtual Lookup DurationUnit { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of tickets associated with this package.
    /// This represents all support requests made using this package type.
    /// </summary>
    /// <value>
    /// A collection of Ticket entities that reference this package.
    /// This collection is populated by Entity Framework and represents a one-to-many relationship.
    /// </value>
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    
    /// <summary>
    /// Gets or sets the collection of package purchase history records.
    /// This tracks all instances where students have purchased this package.
    /// </summary>
    /// <value>
    /// A collection of PackageHistory entities representing purchases of this package.
    /// This collection is populated by Entity Framework and represents a one-to-many relationship.
    /// </value>
    public virtual ICollection<PackageHistory> PackageHistories { get; set; } = new List<PackageHistory>();
}
