namespace MTA.Application.DTOs;

/// <summary>
/// Data Transfer Object for Course entity
/// </summary>
public class CourseDto : BaseDto
{
    /// <summary>
    /// Title of the course
    /// </summary>
    public required string Title { get; set; }
    
    /// <summary>
    /// Detailed description of the course content
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// URL to the course icon image
    /// </summary>
    public string? ImageIcon { get; set; }
    
    /// <summary>
    /// URL to the course poster image
    /// </summary>
    public string? Poster { get; set; }
    
    /// <summary>
    /// Course price in the system's currency
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// Level ID for this course
    /// </summary>
    public int LevelId { get; set; }
    
    /// <summary>
    /// Level title
    /// </summary>
    public string? LevelTitle { get; set; }
    
    /// <summary>
    /// Status ID for this course
    /// </summary>
    public int StatusId { get; set; }
    
    /// <summary>
    /// Status value
    /// </summary>
    public string? StatusValue { get; set; }
    
    /// <summary>
    /// Number of lessons in this course
    /// </summary>
    public int LessonCount { get; set; }
    
    /// <summary>
    /// Number of users who purchased this course
    /// </summary>
    public int PurchaseCount { get; set; }
}
