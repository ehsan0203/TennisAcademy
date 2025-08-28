namespace MTA.Application.DTOs;

/// <summary>
/// Base DTO class that provides common properties for all DTOs
/// </summary>
public abstract class BaseDto
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Date when the entity was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Date when the entity was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
