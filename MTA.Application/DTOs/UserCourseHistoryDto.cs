namespace MTA.Application.DTOs;

/// <summary>
/// Data Transfer Object for UserCourseHistory entity
/// </summary>
public class UserCourseHistoryDto : BaseDto
{
    /// <summary>
    /// Course ID
    /// </summary>
    public int CourseId { get; set; }
    
    /// <summary>
    /// Course title
    /// </summary>
    public string? CourseTitle { get; set; }
    
    /// <summary>
    /// Course description
    /// </summary>
    public string? CourseDescription { get; set; }
    
    /// <summary>
    /// Course image icon
    /// </summary>
    public string? CourseImageIcon { get; set; }
    
    /// <summary>
    /// Course price
    /// </summary>
    public decimal CoursePrice { get; set; }
    
    /// <summary>
    /// Account ID
    /// </summary>
    public int AccountId { get; set; }
    
    /// <summary>
    /// User's first name
    /// </summary>
    public string? UserFirstName { get; set; }
    
    /// <summary>
    /// User's last name
    /// </summary>
    public string? UserLastName { get; set; }
    
    /// <summary>
    /// User's email
    /// </summary>
    public string? UserEmail { get; set; }
}
