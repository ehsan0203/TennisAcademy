namespace MTA.Domain.Entities;

/// <summary>
/// Represents a user role in the MTA system that defines access levels and permissions.
/// Roles determine what actions users can perform and what areas of the system they can access.
/// </summary>
/// <remarks>
/// The role-based access control system provides security and functionality segregation.
/// Common roles include Student (course access), Coach (support provision), and Admin (system management).
/// Roles are linked to permissions through a many-to-many relationship for flexible authorization.
/// </remarks>
public class Role : BaseEntity
{
    /// <summary>
    /// Gets or sets the title of the role.
    /// This is the display name used for role identification and assignment.
    /// </summary>
    /// <value>
    /// A non-empty string containing the role title (e.g., "Student", "Coach", "Admin").
    /// This field is required and should be unique within the system.
    /// </value>
    /// <remarks>
    /// Role titles should be clear and reflect the user's function within the system.
    /// They are used in user interfaces, reports, and authorization logic.
    /// </remarks>
    public required string Title { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of accounts that have been assigned this role.
    /// This represents all users who currently hold this role.
    /// </summary>
    /// <value>
    /// A collection of Account entities that are assigned to this role.
    /// This collection is populated by Entity Framework and represents a one-to-many relationship.
    /// </value>
    /// <remarks>
    /// This relationship enables role-based user management, reporting, and
    /// bulk operations on users with specific roles.
    /// </remarks>
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    
    /// <summary>
    /// Gets or sets the collection of role-permission relationships.
    /// This defines what permissions are granted to users with this role.
    /// </summary>
    /// <value>
    /// A collection of PermissionsRole entities that link this role to specific permissions.
    /// This collection is populated by Entity Framework and represents a many-to-many relationship.
    /// </value>
    /// <remarks>
    /// The role-permission system enables fine-grained access control and
    /// flexible authorization management without hardcoding permissions.
    /// </remarks>
    public virtual ICollection<PermissionsRole> RolePermissions { get; set; } = new List<PermissionsRole>();
}
