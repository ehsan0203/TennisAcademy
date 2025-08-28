namespace MTA.Domain.Entities;

/// <summary>
/// Represents a skill level classification in the MTA tennis academy system.
/// Levels define tennis playing ability categories used for course targeting,
/// student assessment, and appropriate content delivery.
/// </summary>
/// <remarks>
/// Skill levels provide a standardized way to categorize tennis players based on
/// their abilities and experience. This enables proper course recommendations,
/// appropriate coaching assignments, and effective learning progressions.
/// Common levels include Beginner, Intermediate, Advanced, and Expert classifications.
/// </remarks>
public class Level : BaseEntity
{
    /// <summary>
    /// Gets or sets the title of the skill level.
    /// This is the display name used throughout the system for level identification.
    /// </summary>
    /// <value>
    /// A non-empty string containing the level title (e.g., "Beginner", "Intermediate", "Advanced").
    /// This field is required and should be unique within the system.
    /// </value>
    /// <remarks>
    /// Level titles should be clear, standardized, and easily understood by both
    /// students and instructors. They are used in course descriptions, user profiles,
    /// and recommendation algorithms.
    /// </remarks>
    public required string Title { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of courses designed for this skill level.
    /// This represents all courses that target students at this particular level.
    /// </summary>
    /// <value>
    /// A collection of Course entities that are appropriate for this skill level.
    /// This collection is populated by Entity Framework and represents a one-to-many relationship.
    /// </value>
    /// <remarks>
    /// This relationship enables:
    /// - Course filtering and discovery by skill level
    /// - Appropriate content recommendations for students
    /// - Curriculum organization and progression planning
    /// - Analytics on course distribution across skill levels
    /// </remarks>
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    
    /// <summary>
    /// Gets or sets the collection of user profiles that have this skill level.
    /// This represents all students who are currently classified at this level.
    /// </summary>
    /// <value>
    /// A collection of UserProfile entities representing students at this skill level.
    /// This collection is populated by Entity Framework and represents a one-to-many relationship.
    /// </value>
    /// <remarks>
    /// This relationship supports:
    /// - Student grouping and class organization
    /// - Skill level distribution analytics
    /// - Peer matching and social features
    /// - Progress tracking and advancement planning
    /// - Targeted communications and promotions
    /// </remarks>
    public virtual ICollection<UserProfile> Profiles { get; set; } = new List<UserProfile>();
}
