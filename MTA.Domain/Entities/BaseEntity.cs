using MTA.Domain.Interfaces;

namespace MTA.Domain.Entities;

public abstract class BaseEntity : IAuditable
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? ModifiedBy { get; set; }
}
