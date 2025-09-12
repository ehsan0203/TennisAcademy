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

    public required string Title { get; set; }
    
    public string? Description { get; set; }
    
    public virtual ICollection<PermissionsRole> PermissionsRoles { get; set; } = new List<PermissionsRole>();
}
