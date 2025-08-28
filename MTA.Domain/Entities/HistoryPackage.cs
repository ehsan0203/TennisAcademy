using System.ComponentModel.DataAnnotations.Schema;

namespace MTA.Domain.Entities;

/// <summary>
/// Represents package history in the system
/// </summary>
public class PackageHistory : BaseEntity
{
    public DateTime ExpiredDate { get; set; }
    public int RemainingTickets { get; set; }
    public int RemainingMessages { get; set; }

    public int PackageId { get; set; }
    [ForeignKey("PackageId")]
    public virtual Package Package { get; set; } = null!;
    
    public int AccountId { get; set; }
    [ForeignKey("AccountId")]
    public virtual Account Account { get; set; } = null!;
}
