namespace MTA.Application.DTOs.Course;

/// <summary>
/// Data Transfer Object for course statistics
/// </summary>
public class CourseStatisticsDto
{
    /// <summary>
    /// Total number of courses
    /// </summary>
    public int TotalCourses { get; set; }
    
    /// <summary>
    /// Number of active courses
    /// </summary>
    public int ActiveCourses { get; set; }
    
    /// <summary>
    /// Number of draft courses
    /// </summary>
    public int DraftCourses { get; set; }
    
    /// <summary>
    /// Number of archived courses
    /// </summary>
    public int ArchivedCourses { get; set; }
    
    /// <summary>
    /// Total number of enrollments
    /// </summary>
    public int TotalEnrollments { get; set; }
    
    /// <summary>
    /// Number of active enrollments
    /// </summary>
    public int ActiveEnrollments { get; set; }
    
    /// <summary>
    /// Number of completed courses
    /// </summary>
    public int CompletedCourses { get; set; }
    
    /// <summary>
    /// Total revenue from paid courses
    /// </summary>
    public decimal TotalRevenue { get; set; }
    
    /// <summary>
    /// Average course completion rate
    /// </summary>
    public double CompletionRate { get; set; }
    
    /// <summary>
    /// Most popular course level
    /// </summary>
    public string? MostPopularLevel { get; set; }
    
    /// <summary>
    /// Course with highest enrollment
    /// </summary>
    public string? MostEnrolledCourse { get; set; }
}
