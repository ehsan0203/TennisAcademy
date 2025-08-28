namespace MTA.Application.DTOs;

/// <summary>
/// Data Transfer Object for Account entity
/// </summary>
public class AccountDto : BaseDto
{
    /// <summary>
    /// User's email address
    /// </summary>
    public required string Email { get; set; }
    
    /// <summary>
    /// Whether the account is currently active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// URL to user's profile image
    /// </summary>
    public string? Image { get; set; }
    
    /// <summary>
    /// Role ID for this account
    /// </summary>
    public int RoleId { get; set; }
    
    /// <summary>
    /// Role title
    /// </summary>
    public string? RoleTitle { get; set; }
    
    /// <summary>
    /// Status ID for this account
    /// </summary>
    public int StatusId { get; set; }
    
    /// <summary>
    /// Status value
    /// </summary>
    public string? StatusValue { get; set; }
    
    /// <summary>
    /// User profile information
    /// </summary>
    public UserProfileDto? UserProfile { get; set; }
}

/// <summary>
/// Data Transfer Object for UserProfile entity
/// </summary>
public class UserProfileDto : BaseDto
{
    /// <summary>
    /// User's first name
    /// </summary>
    public required string FirstName { get; set; }
    
    /// <summary>
    /// User's last name
    /// </summary>
    public required string LastName { get; set; }
    
    /// <summary>
    /// User's date of birth
    /// </summary>
    public DateTime DateOfBirth { get; set; }
    
    /// <summary>
    /// User's tennis experience in years
    /// </summary>
    public int Experience { get; set; }
    
    /// <summary>
    /// Account ID for this profile
    /// </summary>
    public int AccountId { get; set; }
    
    /// <summary>
    /// Skill level ID for this profile
    /// </summary>
    public int SkillLevelId { get; set; }
    
    /// <summary>
    /// Skill level value
    /// </summary>
    public string? SkillLevelValue { get; set; }
}
