namespace MTA.Domain.Interfaces;

public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    int? CreatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    int? ModifiedBy { get; set; }
}
