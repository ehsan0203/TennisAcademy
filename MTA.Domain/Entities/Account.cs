
using System.ComponentModel.DataAnnotations.Schema;

namespace MTA.Domain.Entities;

public class Account : BaseEntity
{

    public required string Email { get; set; }
    
    public required string Password { get; set; }
    
    public bool IsActive { get; set; } = true;

    public string? ProfileImagePath { get; set; }

    public int? MediaFileId { get; set; }
    [ForeignKey(nameof(MediaFileId))]
    public virtual MediaFile? MediaFile { get; set; }

    public int RoleId { get; set; }

    [ForeignKey(nameof(RoleId))]
    public virtual Role Role { get; set; } = null!;

    public int StatusId { get; set; }
    
    [ForeignKey(nameof(StatusId))]
    public virtual Lookup Status { get; set; } = null!;

    public virtual UserProfile? UserProfile { get; set; }
    
    public virtual ICollection<UserCourseHistory> UserCourseHistory { get; set; } = new List<UserCourseHistory>();
    
    public virtual ICollection<PackageHistory> PackageHistory { get; set; } = new List<PackageHistory>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

