using System.ComponentModel.DataAnnotations.Schema;

namespace MTA.Domain.Entities;

/// <summary>
/// Represents a tennis course in the MTA (Modern Tennis Academy) system.
/// A course is a structured learning program that contains multiple lessons
/// and is designed for specific skill levels and learning objectives.
/// </summary>
/// <remarks>
/// Courses are the primary educational content in the MTA system. Each course
/// is designed for a specific skill level and contains a series of lessons
/// that progressively build tennis skills and knowledge. Courses can be free
/// or paid, and may include various media types such as videos, documents,
/// and interactive content.
/// 
/// The course entity manages:
/// - Course metadata and presentation information
/// - Pricing and access control
/// - Skill level requirements and targeting
/// - Lesson organization and sequencing
/// - Student enrollment and progress tracking
/// </remarks>
public class Course : BaseEntity
{
    public required string Title { get; set; }
    
    public string? Description { get; set; }
    
    public string? ImageIcon { get; set; }
    
    public string? Poster { get; set; }
    
    public decimal Price { get; set; }
    
    public int LevelId { get; set; }
    
    [ForeignKey("LevelId")]
    public virtual Level Level { get; set; } = null!;

    public int StatusId { get; set; }
    [ForeignKey("StatusId")]
    public virtual Lookup Status { get; set; } = null!;

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    
    public virtual ICollection<UserCourseHistory> UserCourseHistory { get; set; } = new List<UserCourseHistory>();
}
