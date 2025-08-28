namespace MTA.Domain.Entities;

/// <summary>
/// Represents a system permission that defines specific actions or access rights.
/// Permissions are granular capabilities that can be assigned to roles for precise access control.
/// </summary>
/// <remarks>
/// The permission system enables fine-grained authorization control by defining specific
/// actions users can perform. Permissions are assigned to roles, which are then assigned
/// to users, creating a flexible and manageable security model.
/// </remarks>
public class Permission : BaseEntity
{
    /// <summary>
    /// Gets or sets the title of the permission.
    /// This is the display name used for permission identification and management.
    /// </summary>
    /// <value>
    /// A non-empty string containing the permission title (e.g., "Create Course", "Manage Users").
    /// This field is required and should clearly describe the permitted action.
    /// </value>
    /// <remarks>
    /// Permission titles should be descriptive and follow a consistent naming convention
    /// to ensure clarity in role and access management.
    /// </remarks>
    public required string Title { get; set; }
    
    /// <summary>
    /// Gets or sets the detailed description of what this permission allows.
    /// This provides additional context for administrators managing permissions.
    /// </summary>
    /// <value>
    /// An optional string containing a detailed description of the permission's scope and function.
    /// Can be null or empty if the title is sufficiently descriptive.
    /// </value>
    /// <remarks>
    /// Descriptions help administrators understand the implications of granting
    /// specific permissions and ensure appropriate access control decisions.
    /// </remarks>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of role-permission relationships.
    /// This defines which roles have been granted this permission.
    /// </summary>
    /// <value>
    /// A collection of PermissionsRole entities that link this permission to specific roles.
    /// This collection is populated by Entity Framework and represents a many-to-many relationship.
    /// </value>
    /// <remarks>
    /// This relationship enables flexible permission management where permissions
    /// can be granted to multiple roles and roles can have multiple permissions.
    /// </remarks>
    public virtual ICollection<PermissionsRole> PermissionsRoles { get; set; } = new List<PermissionsRole>();
}
