using System.ComponentModel.DataAnnotations.Schema;

namespace MTA.Domain.Entities;

public class UserProfile : BaseEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }    
    public int Experience { get; set; }
    public bool HealthCondition { get; set; }
    public string? HealthDescription { get; set; }

    public int AccountId { get; set; }
    [ForeignKey("AccountId")]
    public virtual Account Account { get; set; } = null!;

    public int SkillLevelId { get; set; }
    [ForeignKey("SkillLevelId")]
    public virtual Level SkillLevel { get; set; } = null!;
}
