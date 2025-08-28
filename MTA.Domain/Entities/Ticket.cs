using System.ComponentModel.DataAnnotations.Schema;

namespace MTA.Domain.Entities;

/// <summary>
/// Represents a ticket in the system
/// </summary>
public class Ticket : BaseEntity
{
    public required string Topic { get; set; }


    public int StatusId { get; set; }
    [ForeignKey("StatusId")]
    public virtual Lookup Status { get; set; }

    public int AccountId { get; set; }
    [ForeignKey("AccountId")]
    public virtual Account Account { get; set; } = null!;

    public int PackageId { get; set; }
    [ForeignKey("PackageId")]
    public virtual Package Package { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
