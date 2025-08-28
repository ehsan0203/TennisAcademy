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
    /// <summary>
    /// Gets or sets the title of the course.
    /// This is the main identifier and display name for the course.
    /// </summary>
    /// <value>
    /// A non-empty string containing the course title.
    /// This field is required and used throughout the system for course identification.
    /// </value>
    /// <remarks>
    /// The course title should be:
    /// - Clear and descriptive of the course content
    /// - Appropriate for the target skill level
    /// - Unique within the system for easy identification
    /// - Optimized for search and discovery
    /// 
    /// Examples:
    /// - "Tennis Fundamentals for Beginners"
    /// - "Advanced Serve Techniques"
    /// - "Match Strategy and Mental Game"
    /// - "Junior Tennis Development Program"
    /// </remarks>
    public required string Title { get; set; }
    
    /// <summary>
    /// Gets or sets the detailed description of the course.
    /// This provides comprehensive information about course content, objectives, and expectations.
    /// </summary>
    /// <value>
    /// An optional string containing the course description and details.
    /// Can be null or empty if no detailed description is provided.
    /// </value>
    /// <remarks>
    /// The course description typically includes:
    /// - Learning objectives and outcomes
    /// - Course prerequisites and skill level requirements
    /// - Overview of topics and lessons covered
    /// - Expected time commitment and duration
    /// - Equipment or preparation requirements
    /// - Benefits and skills students will gain
    /// 
    /// This information helps students make informed decisions about course enrollment
    /// and sets appropriate expectations for the learning experience.
    /// </remarks>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the URL or path to the course icon image.
    /// This small image is used for course identification in lists and menus.
    /// </summary>
    /// <value>
    /// An optional string containing the URL or file path to the course icon.
    /// Can be null or empty if no icon is specified.
    /// </value>
    /// <remarks>
    /// Course icons are used for:
    /// - Quick visual identification in course lists
    /// - Navigation menus and course selectors
    /// - Mobile app interfaces where space is limited
    /// - Category and skill level visual indicators
    /// 
    /// Icon considerations:
    /// - Should be small and recognizable at various sizes
    /// - Use consistent visual style across courses
    /// - Consider accessibility and color contrast
    /// - Support common image formats (PNG, SVG, WebP)
    /// </remarks>
    public string? ImageIcon { get; set; }
    
    /// <summary>
    /// Gets or sets the URL or path to the course poster image.
    /// This larger promotional image is used for course marketing and detailed views.
    /// </summary>
    /// <value>
    /// An optional string containing the URL or file path to the course poster.
    /// Can be null or empty if no poster is specified.
    /// </value>
    /// <remarks>
    /// Course posters are used for:
    /// - Course detail pages and promotional materials
    /// - Featured course displays and banners
    /// - Social media sharing and marketing
    /// - Course catalog and browsing interfaces
    /// 
    /// Poster considerations:
    /// - Should be high quality and professionally designed
    /// - Include relevant tennis imagery or graphics
    /// - Maintain consistent branding and style
    /// - Optimize for various display sizes and devices
    /// - Consider loading performance and file size
    /// </remarks>
    public string? Poster { get; set; }
    
    /// <summary>
    /// Gets or sets the price of the course in the system's base currency.
    /// This determines the cost for students to access the course content.
    /// </summary>
    /// <value>
    /// A decimal value representing the course price.
    /// Zero indicates a free course, while positive values indicate paid courses.
    /// </value>
    /// <remarks>
    /// Pricing considerations:
    /// - Zero price indicates a free course available to all users
    /// - Pricing should reflect course value and market positioning
    /// - Consider tiered pricing for different access levels
    /// - May include promotional pricing and discounts
    /// - Currency handling should be consistent across the system
    /// 
    /// Price is used for:
    /// - Course access control and enrollment validation
    /// - Payment processing and billing
    /// - Revenue tracking and business analytics
    /// - Course filtering and search functionality
    /// - Promotional and marketing displays
    /// </remarks>
    public decimal Price { get; set; }
    
    /// <summary>
    /// Gets or sets the foreign key reference to the course's target skill level.
    /// This determines which students the course is appropriate for based on their tennis ability.
    /// </summary>
    /// <value>
    /// An integer representing the ID of the target skill level.
    /// This field is required and must reference a valid level entry.
    /// </value>
    /// <remarks>
    /// Skill level targeting is used for:
    /// - Course recommendation algorithms
    /// - Student eligibility and enrollment validation
    /// - Course categorization and organization
    /// - Progress pathway planning and sequencing
    /// - Appropriate content difficulty and pacing
    /// 
    /// Typical skill levels include:
    /// - Beginner: First-time tennis players
    /// - Intermediate: Players with basic skills
    /// - Advanced: Experienced players seeking refinement
    /// - Expert: High-level competitive players
    /// </remarks>
    public int LevelId { get; set; }
    
    /// <summary>
    /// Gets or sets the navigation property to the Level entity.
    /// This provides access to the detailed skill level information for the course.
    /// </summary>
    /// <value>
    /// A Level entity that defines the target skill level for this course.
    /// This property is populated by Entity Framework through the foreign key relationship.
    /// </value>
    [ForeignKey("LevelId")]
    public virtual Level Level { get; set; } = null!;

    /// <summary>
    /// Gets or sets the foreign key reference to the course's current status.
    /// This determines the availability and visibility of the course to students.
    /// </summary>
    /// <value>
    /// An integer representing the ID of the status lookup value.
    /// This field is required and must reference a valid status entry.
    /// </value>
    /// <remarks>
    /// Course status examples include:
    /// - Draft: Course is being developed and not yet available
    /// - Published: Course is live and available for enrollment
    /// - Archived: Course is no longer actively promoted but still accessible
    /// - Suspended: Course is temporarily unavailable due to issues
    /// - Retired: Course is permanently discontinued
    /// 
    /// Status affects:
    /// - Course visibility in catalogs and search results
    /// - Student enrollment availability
    /// - Content access and streaming permissions
    /// - Administrative and reporting functions
    /// </remarks>
    public int StatusId { get; set; }
    /// <summary>
    /// Gets or sets the navigation property to the Status lookup entity.
    /// This provides detailed status information for the course.
    /// </summary>
    /// <value>
    /// A Lookup entity containing the status details and description.
    /// This property is populated by Entity Framework through the foreign key relationship.
    /// </value>
    [ForeignKey("StatusId")]
    public virtual Lookup Status { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of lessons that belong to this course.
    /// This represents the structured learning content within the course.
    /// </summary>
    /// <value>
    /// A collection of Lesson entities that make up the course curriculum.
    /// This collection is populated by Entity Framework and represents a one-to-many relationship.
    /// </value>
    /// <remarks>
    /// The lessons collection provides:
    /// - Sequential learning content and progression
    /// - Individual lesson access and tracking
    /// - Content organization and navigation
    /// - Progress measurement and completion tracking
    /// 
    /// Lessons typically include:
    /// - Video content and demonstrations
    /// - Written materials and guides
    /// - Interactive exercises and drills
    /// - Assessment and evaluation components
    /// 
    /// The order and structure of lessons is crucial for effective learning progression.
    /// </remarks>
    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    
    /// <summary>
    /// Gets or sets the collection of user course history records for this course.
    /// This tracks all students who have enrolled in or purchased access to this course.
    /// </summary>
    /// <value>
    /// A collection of UserCourseHistory entities representing student enrollments and purchases.
    /// This collection is populated by Entity Framework and represents a one-to-many relationship.
    /// </value>
    /// <remarks>
    /// User course history is used for:
    /// - Enrollment tracking and access control
    /// - Student progress monitoring and reporting
    /// - Course popularity and success metrics
    /// - Revenue tracking and business analytics
    /// - Student communication and support
    /// 
    /// This data enables:
    /// - Personalized learning recommendations
    /// - Course completion certificates
    /// - Performance analytics and improvements
    /// - Customer support and assistance
    /// - Marketing and promotional insights
    /// </remarks>
    public virtual ICollection<UserCourseHistory> UserCourseHistory { get; set; } = new List<UserCourseHistory>();
}
