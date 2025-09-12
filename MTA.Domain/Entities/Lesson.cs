using System.ComponentModel.DataAnnotations.Schema;

namespace MTA.Domain.Entities;

/// <summary>
/// Represents an individual lesson within a tennis course in the MTA system.
/// Lessons are the fundamental learning units that contain specific tennis instruction,
/// demonstrations, and educational content for students.
/// </summary>
/// <remarks>
/// Each lesson is part of a structured course and contains focused content on specific
/// tennis techniques, concepts, or skills. Lessons can include various media types
/// such as instructional videos, written guides, diagrams, and interactive exercises.
/// The lesson system supports both free preview content and premium paid content.
/// </remarks>
public class Lesson : BaseEntity
{
    public required string Title { get; set; }
    
    public string? Description { get; set; }

    public bool IsFree { get; set; } = false;
    public int Order { get; set; }

    public int CourseId { get; set; }

    [ForeignKey("CourseId")]
    public virtual Course Course { get; set; } = null!;
    
    public virtual ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();
}

