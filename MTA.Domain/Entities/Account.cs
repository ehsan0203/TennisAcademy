using System.ComponentModel.DataAnnotations.Schema;

namespace MTA.Domain.Entities;

/// <summary>
/// Represents a user account in the MTA (Modern Tennis Academy) system.
/// This entity serves as the primary authentication and authorization record for users,
/// containing essential login credentials and account management information.
/// </summary>
/// <remarks>
/// Each account represents a unique user in the system and is linked to a UserProfile
/// for detailed personal information. Accounts have roles that determine their permissions
/// and access levels within the system. The account also maintains relationships with
/// various user activities such as course purchases, package subscriptions, and support tickets.
/// </remarks>
public class Account : BaseEntity
{
    /// <summary>
    /// Gets or sets the user's email address.
    /// This serves as the unique identifier for login and communication purposes.
    /// </summary>
    /// <value>
    /// A valid email address that must be unique across the system.
    /// This field is required and used for authentication.
    /// </value>
    /// <remarks>
    /// The email address is used for:
    /// - User authentication and login
    /// - Password recovery communications
    /// - System notifications and updates
    /// - Unique identification of users
    /// Email validation should be performed at the application layer.
    /// </remarks>
    public required string Email { get; set; }
    
    /// <summary>
    /// Gets or sets the user's password hash.
    /// This should contain the hashed version of the user's password, never the plain text.
    /// </summary>
    /// <value>
    /// A hashed password string generated using a secure hashing algorithm.
    /// This field is required for authentication purposes.
    /// </value>
    /// <remarks>
    /// Security considerations:
    /// - This should never store plain text passwords
    /// - Use a strong hashing algorithm (e.g., bcrypt, Argon2)
    /// - Include salt to prevent rainbow table attacks
    /// - Consider implementing password complexity requirements
    /// </remarks>
    public required string Password { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the account is currently active.
    /// Active accounts can log in and access system features, while inactive accounts are blocked.
    /// </summary>
    /// <value>
    /// True if the account is active and can be used for login; otherwise, false.
    /// The default value is true for new accounts.
    /// </value>
    /// <remarks>
    /// This property is used for:
    /// - Account suspension by administrators
    /// - Temporary account deactivation
    /// - Account recovery processes
    /// - Soft deletion of accounts (preserving data while preventing access)
    /// </remarks>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the URL or path to the user's profile image.
    /// This is an optional field for user personalization.
    /// </summary>
    /// <value>
    /// A string containing the URL or file path to the user's profile image.
    /// Can be null or empty if no image is set.
    /// </value>
    /// <remarks>
    /// Image considerations:
    /// - Should support common image formats (JPEG, PNG, WebP)
    /// - Consider image size limitations for performance
    /// - May be stored locally or in cloud storage
    /// - Should have fallback/default image handling
    /// </remarks>
    public string? Image { get; set; }
    
    /// <summary>
    /// Gets or sets the foreign key reference to the user's role.
    /// This determines the user's permissions and access level within the system.
    /// </summary>
    /// <value>
    /// An integer representing the ID of the role assigned to this account.
    /// This field is required and must reference a valid role.
    /// </value>
    /// <remarks>
    /// Common roles in the system may include:
    /// - Student: Regular users who can purchase courses and packages
    /// - Coach: Instructors who can provide support and guidance
    /// - Admin: System administrators with full access
    /// - Moderator: Users with limited administrative capabilities
    /// </remarks>
    public int RoleId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the Role entity.
    /// This provides access to the role information and permissions for this account.
    /// </summary>
    /// <value>
    /// A Role entity that defines the permissions and access level for this account.
    /// This property is populated by Entity Framework through the foreign key relationship.
    /// </value>
    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; } = null!;

    /// <summary>
    /// Gets or sets the foreign key reference to the account's status.
    /// This provides additional status information beyond the simple IsActive flag.
    /// </summary>
    /// <value>
    /// An integer representing the ID of the status lookup value.
    /// This field is required and must reference a valid lookup entry.
    /// </value>
    /// <remarks>
    /// Status examples may include:
    /// - Active: Normal operational status
    /// - Suspended: Temporarily blocked due to violations
    /// - Pending: Awaiting verification or approval
    /// - Locked: Blocked due to security concerns
    /// </remarks>
    public int StatusId { get; set; }
    
    /// <summary>
    /// Gets or sets the navigation property to the Status lookup entity.
    /// This provides detailed status information for the account.
    /// </summary>
    /// <value>
    /// A Lookup entity containing the status details for this account.
    /// This property is populated by Entity Framework through the foreign key relationship.
    /// </value>
    [ForeignKey("StatusId")]
    public virtual Lookup Status { get; set; } = null!;

    /// <summary>
    /// Gets or sets the navigation property to the user's profile information.
    /// This contains detailed personal information about the user.
    /// </summary>
    /// <value>
    /// A UserProfile entity containing personal details such as name, experience, and skill level.
    /// This property represents a one-to-one relationship with the UserProfile entity.
    /// </value>
    /// <remarks>
    /// The UserProfile contains:
    /// - Personal information (name, date of birth)
    /// - Tennis experience and skill level
    /// - Preferences and additional details
    /// This separation allows for better data organization and privacy management.
    /// </remarks>
    public virtual UserProfile? UserProfile { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of course purchase history for this account.
    /// This tracks all courses that the user has purchased or accessed.
    /// </summary>
    /// <value>
    /// A collection of UserCourseHistory entities representing course purchases and access.
    /// This collection is populated by Entity Framework and represents a one-to-many relationship.
    /// </value>
    /// <remarks>
    /// This collection is used for:
    /// - Tracking course purchases and payment history
    /// - Determining course access permissions
    /// - Generating learning progress reports
    /// - Analytics and business intelligence
    /// </remarks>
    public virtual ICollection<UserCourseHistory> UserCourseHistory { get; set; } = new List<UserCourseHistory>();
    
    /// <summary>
    /// Gets or sets the collection of package purchase history for this account.
    /// This tracks all support packages that the user has purchased.
    /// </summary>
    /// <value>
    /// A collection of PackageHistory entities representing package purchases and usage.
    /// This collection is populated by Entity Framework and represents a one-to-many relationship.
    /// </value>
    /// <remarks>
    /// Package history includes:
    /// - Purchase dates and expiration information
    /// - Remaining tickets and messages
    /// - Usage tracking and analytics
    /// - Billing and payment history
    /// </remarks>
    public virtual ICollection<PackageHistory> PackageHistory { get; set; } = new List<PackageHistory>();
    
    /// <summary>
    /// Gets or sets the collection of support tickets created by this account.
    /// This includes all support requests and inquiries submitted by the user.
    /// </summary>
    /// <value>
    /// A collection of Ticket entities representing support requests from this user.
    /// This collection is populated by Entity Framework and represents a one-to-many relationship.
    /// </value>
    /// <remarks>
    /// Tickets are used for:
    /// - Customer support and issue resolution
    /// - Technical assistance requests
    /// - General inquiries and feedback
    /// - Communication between users and support staff
    /// </remarks>
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    
    /// <summary>
    /// Gets or sets the collection of messages sent by this account.
    /// This includes all messages sent in support tickets and communications.
    /// </summary>
    /// <value>
    /// A collection of Message entities representing messages sent by this user.
    /// This collection is populated by Entity Framework and represents a one-to-many relationship.
    /// </value>
    /// <remarks>
    /// Messages are used for:
    /// - Communication within support tickets
    /// - Responses to coach feedback
    /// - System notifications and updates
    /// - Audit trail of user communications
    /// </remarks>
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}

