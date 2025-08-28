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
    /// <summary>
    /// Gets or sets the title of the lesson.
    /// This is the primary identifier and display name for the lesson content.
    /// </summary>
    /// <value>
    /// A non-empty string containing the lesson title.
    /// This field is required and should clearly describe the lesson focus.
    /// </value>
    /// <remarks>
    /// Examples: "Basic Forehand Technique", "Serve Mechanics and Power", 
    /// "Volley Positioning and Timing", "Match Strategy Fundamentals"
    /// </remarks>
    public required string Title { get; set; }
    
    /// <summary>
    /// Gets or sets the detailed description of the lesson content.
    /// This provides additional context and learning objectives for the lesson.
    /// </summary>
    /// <value>
    /// An optional string containing lesson description, objectives, and key concepts.
    /// Can be null or empty if no detailed description is provided.
    /// </value>
    /// <remarks>
    /// The description typically includes learning objectives, key techniques covered,
    /// prerequisites, and expected outcomes for the student.
    /// </remarks>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether this lesson is available for free access.
    /// Free lessons can be viewed without course purchase for preview purposes.
    /// </summary>
    /// <value>
    /// True if the lesson is freely accessible; false if it requires course purchase.
    /// The default value is false (premium content).
    /// </value>
    /// <remarks>
    /// Free lessons are typically used for:
    /// - Course previews and marketing
    /// - Basic introductory content
    /// - Sample lessons to demonstrate quality
    /// - Community engagement and brand building
    /// </remarks>
    public bool IsFree { get; set; } = false;
    public int Order { get; set; }


    /// <summary>
    /// Gets or sets the foreign key reference to the parent course.
    /// This establishes the lesson's membership within a specific course structure.
    /// </summary>
    /// <value>
    /// An integer representing the ID of the parent Course entity.
    /// This field is required and must reference a valid course.
    /// </value>
    /// <remarks>
    /// The course relationship provides lesson organization, access control,
    /// and sequential learning progression within the educational structure.
    /// </remarks>
    public int CourseId { get; set; }
    
    /// <summary>
    /// Gets or sets the navigation property to the parent Course entity.
    /// This provides access to course information and related lesson context.
    /// </summary>
    /// <value>
    /// A Course entity that contains this lesson.
    /// This property is populated by Entity Framework through the foreign key relationship.
    /// </value>
    [ForeignKey("CourseId")]
    public virtual Course Course { get; set; } = null!;
    
    /// <summary>
    /// Gets or sets the collection of media files associated with this lesson.
    /// This includes videos, documents, images, and other educational resources.
    /// </summary>
    /// <value>
    /// A collection of MediaFile entities containing lesson content and resources.
    /// This collection is populated by Entity Framework and represents a one-to-many relationship.
    /// </value>
    /// <remarks>
    /// Media files typically include:
    /// - Instructional videos and demonstrations
    /// - PDF guides and written materials
    /// - Images and diagrams
    /// - Audio commentary and explanations
    /// - Interactive content and exercises
    /// </remarks>
    public virtual ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();
}

