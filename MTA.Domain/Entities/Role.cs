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
    public required string Title { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    
    public virtual ICollection<PermissionsRole> RolePermissions { get; set; } = new List<PermissionsRole>();
}
