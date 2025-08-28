using System.ComponentModel.DataAnnotations.Schema;

namespace MTA.Domain.Entities;

/// <summary>
/// Represents detailed profile information for a user account in the MTA system.
/// This entity contains personal information, tennis experience, and skill level data
/// that supplements the basic account information.
/// </summary>
/// <remarks>
/// The UserProfile entity is designed to store detailed personal information separately
/// from the authentication data in the Account entity. This separation provides better
/// data organization, privacy management, and allows for flexible profile management.
/// Each UserProfile is linked to exactly one Account through a one-to-one relationship.
/// 
/// The profile includes tennis-specific information such as experience level and skill
/// rating, which are used for course recommendations and coaching assignments.
/// </remarks>
public class UserProfile : BaseEntity
{
    /// <summary>
    /// Gets or sets the user's first name.
    /// This is used for personalization and identification purposes throughout the system.
    /// </summary>
    /// <value>
    /// A non-empty string containing the user's first name.
    /// This field is required for account completion.
    /// </value>
    /// <remarks>
    /// The first name is used for:
    /// - Personalizing user interfaces and communications
    /// - Generating display names and greetings
    /// - Coach-student interaction identification
    /// - Certificate and achievement generation
    /// Consider cultural naming conventions and Unicode support.
    /// </remarks>
    public required string FirstName { get; set; }
    
    /// <summary>
    /// Gets or sets the user's last name.
    /// This completes the user's full name for identification and formal communications.
    /// </summary>
    /// <value>
    /// A non-empty string containing the user's last name or surname.
    /// This field is required for account completion.
    /// </value>
    /// <remarks>
    /// The last name is used for:
    /// - Formal communications and documentation
    /// - Sorting and organizing user lists
    /// - Legal and billing purposes
    /// - Professional certification and records
    /// Consider cultural naming conventions and multiple surname support.
    /// </remarks>
    public required string LastName { get; set; }
    
    /// <summary>
    /// Gets or sets the user's date of birth.
    /// This is used for age verification, age-appropriate content, and demographics.
    /// </summary>
    /// <value>
    /// A DateTime value representing the user's birth date.
    /// This field is required for age verification and program eligibility.
    /// </value>
    /// <remarks>
    /// Date of birth is used for:
    /// - Age verification for program eligibility
    /// - Age-appropriate content and course recommendations
    /// - Junior vs adult program classification
    /// - Demographic analysis and reporting
    /// - Birthday notifications and celebrations
    /// 
    /// Privacy considerations:
    /// - Consider data protection regulations (GDPR, COPPA)
    /// - Implement appropriate access controls
    /// - May require parental consent for minors
    /// </remarks>
    public DateTime DateOfBirth { get; set; }
    
    /// <summary>
    /// Gets or sets the user's tennis experience in years.
    /// This indicates how many years the user has been playing tennis.
    /// </summary>
    /// <value>
    /// An integer representing the number of years of tennis experience.
    /// Zero indicates a complete beginner, while higher values indicate more experience.
    /// </value>
    /// <remarks>
    /// Experience level is used for:
    /// - Course and program recommendations
    /// - Skill level assessment and validation
    /// - Appropriate coaching assignment
    /// - Progress tracking and goal setting
    /// - Peer grouping and class organization
    /// 
    /// Experience ranges typically include:
    /// - 0 years: Complete beginner
    /// - 1-2 years: Novice player
    /// - 3-5 years: Intermediate player
    /// - 5+ years: Advanced player
    /// - 10+ years: Expert/competitive player
    /// </remarks>
    public int Experience { get; set; }

    /// <summary>
    /// Gets or sets the foreign key reference to the associated account.
    /// This creates a one-to-one relationship between UserProfile and Account entities.
    /// </summary>
    /// <value>
    /// An integer representing the ID of the associated Account entity.
    /// This field is required and must reference a valid account.
    /// </value>
    /// <remarks>
    /// This relationship ensures:
    /// - Each profile belongs to exactly one account
    /// - Account and profile data remain synchronized
    /// - Proper data integrity and referential constraints
    /// - Efficient querying of related account information
    /// </remarks>
    public int AccountId { get; set; }
    
    /// <summary>
    /// Gets or sets the navigation property to the associated Account entity.
    /// This provides access to the authentication and basic account information.
    /// </summary>
    /// <value>
    /// An Account entity that owns this profile.
    /// This property is populated by Entity Framework through the foreign key relationship.
    /// </value>
    /// <remarks>
    /// This navigation property allows access to:
    /// - Email address and contact information
    /// - Account status and permissions
    /// - Role and authorization details
    /// - Activity history and preferences
    /// </remarks>
    [ForeignKey("AccountId")]
    public virtual Account Account { get; set; } = null!;

    /// <summary>
    /// Gets or sets the foreign key reference to the user's current skill level.
    /// This determines the user's tennis playing ability and appropriate program placement.
    /// </summary>
    /// <value>
    /// An integer representing the ID of the skill level lookup value.
    /// This field is required and must reference a valid skill level entry.
    /// </value>
    /// <remarks>
    /// Skill levels typically include:
    /// - Beginner: New to tennis, learning basic strokes
    /// - Intermediate: Can rally and play basic games
    /// - Advanced: Consistent strokes, strategic play
    /// - Expert: Tournament level, advanced techniques
    /// - Professional: Competitive/teaching level
    /// 
    /// This is used for:
    /// - Course and lesson recommendations
    /// - Appropriate coaching assignment
    /// - Class and group organization
    /// - Progress tracking and advancement
    /// - Tournament and competition eligibility
    /// </remarks>
    public int SkillLevelId { get; set; }
    
    /// <summary>
    /// Gets or sets the navigation property to the skill level lookup entity.
    /// This provides detailed information about the user's tennis skill level.
    /// </summary>
    /// <value>
    /// A Lookup entity containing the skill level details and description.
    /// This property is populated by Entity Framework through the foreign key relationship.
    /// </value>
    /// <remarks>
    /// The skill level lookup provides:
    /// - Standardized skill level definitions
    /// - Localized descriptions and labels
    /// - Skill level progression pathways
    /// - Assessment criteria and requirements
    /// </remarks>
    [ForeignKey("SkillLevelId")]
    public virtual Level SkillLevel { get; set; } = null!;
}
