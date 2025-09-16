namespace MTA.Application.DTOs;

/// <summary>
/// Data Transfer Object for Permission entity
/// </summary>
public class PermissionDto : BaseDto
{
    /// <summary>
    /// Title of the permission
    /// </summary>
    public required string Title { get; set; }
    
    /// <summary>
    /// Description of what this permission allows
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Number of roles that have this permission
    /// </summary>
    public int RoleCount { get; set; }
}

/// <summary>
/// DTO for permission search
/// </summary>
public class PermissionSearchDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}