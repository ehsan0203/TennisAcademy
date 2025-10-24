using System.ComponentModel.DataAnnotations.Schema;

namespace MTA.Domain.Entities;

/// <summary>
/// Represents user course history in the system
/// </summary>
public class UserCourseHistory : BaseEntity
{

    public int CourseId { get; set; }
    [ForeignKey("CourseId")]
    public virtual Course Course { get; set; } = null!;

    public decimal PurchasePrice { get; set; }


    public int AccountId { get; set; }
    [ForeignKey("AccountId")]
    public virtual Account Account { get; set; } = null!;

    public int StatusId { get; set; } 
    [ForeignKey("StatusId")]
    public virtual Lookup Status { get; set; } = null!;

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
}
