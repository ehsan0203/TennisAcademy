namespace MTA.Domain.Entities;

/// <summary>
/// Represents a lookup table entry for system-wide enumeration values.
/// Lookup entities provide configurable reference data for dropdowns,
/// status values, categories, and other standardized system values.
/// </summary>
/// <remarks>
/// The lookup system provides a flexible way to manage reference data
/// without hardcoding values in the application. This enables easy
/// localization, configuration changes, and dynamic value management.
/// Common categories include status values, file types, duration units, etc.
/// </remarks>
public class Lookup : BaseEntity
{
   
    /// <summary>
    /// Gets or sets the category that groups related lookup values.
    /// This enables logical organization of lookup data.
    /// </summary>
    /// <value>
    /// A string representing the lookup category (e.g., "AccountStatus", "FileType", "CourseStatus").
    /// This field is required for proper data organization.
    /// </value>
    /// <remarks>
    /// Categories help organize lookup values and prevent conflicts between
    /// different types of reference data. Examples: AccountStatus, FileType, CourseStatus
    /// </remarks>
    public required string Category { get; set; }
    
    /// <summary>
    /// Gets or sets the key identifier for the lookup value.
    /// This is typically used in code for programmatic access.
    /// </summary>
    /// <value>
    /// A string representing the lookup key (e.g., "Active", "Draft", "Expired").
    /// This field is required and should be unique within each category.
    /// </value>
    /// <remarks>
    /// Keys are used for programmatic access and should be stable over time.
    /// Examples: Active, Draft, Published, Suspended
    /// </remarks>
    public required string Key { get; set; }
    
    /// <summary>
    /// Gets or sets the display value for the lookup entry.
    /// This is the human-readable text shown to users.
    /// </summary>
    /// <value>
    /// A string representing the display value (e.g., "فعال", "پیش‌نویس").
    /// This field is required and can be localized for different languages.
    /// </value>
    /// <remarks>
    /// Values are user-facing and can be localized for different languages
    /// and cultures. They provide meaningful descriptions for system states.
    /// Examples: فعال, پیش‌نویس, منقضی شده
    /// </remarks>
    public required string Value { get; set; }
}
