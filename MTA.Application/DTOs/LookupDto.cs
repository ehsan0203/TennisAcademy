namespace MTA.Application.DTOs;

/// <summary>
/// Data Transfer Object for Lookup entity
/// </summary>
public class LookupDto
{
    /// <summary>
    /// Unique identifier for the lookup
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Category of the lookup (e.g., AccountStatus, FileType, CourseStatus)
    /// </summary>
    public required string Category { get; set; }
    
    /// <summary>
    /// Key of the lookup (e.g., Active, Draft)
    /// </summary>
    public required string Key { get; set; }
    
    /// <summary>
    /// Value of the lookup (e.g., فعال, پیش‌نویس)
    /// </summary>
    public required string Value { get; set; }
}
