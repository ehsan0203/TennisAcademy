namespace MTA.Domain.Entities;

/// <summary>
/// Base entity class that provides common properties for all entities in the MTA system.
/// This abstract class implements the common audit fields and primary key pattern
/// used across all domain entities.
/// </summary>
/// <remarks>
/// All domain entities should inherit from this base class to ensure consistency
/// in primary key naming and audit trail functionality. The Id property serves
/// as the primary key for all entities, while CreatedAt and UpdatedAt provide
/// automatic audit trail capabilities.
/// </remarks>
public abstract class BaseEntity
{
    public int Id { get; set; }
    

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; }
}
