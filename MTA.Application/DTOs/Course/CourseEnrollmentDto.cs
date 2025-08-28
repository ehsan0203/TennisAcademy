namespace MTA.Application.DTOs.Course;

/// <summary>
/// Data Transfer Object for course enrollment
/// </summary>
public class CourseEnrollmentDto
{
    /// <summary>
    /// Course ID to enroll in
    /// </summary>
    public int CourseId { get; set; }
    
    /// <summary>
    /// User ID enrolling in the course
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// Enrollment date
    /// </summary>
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Enrollment status (Active, Completed, Cancelled, etc.)
    /// </summary>
    public int StatusId { get; set; } = 1; // Assuming 1 is Active status
}
