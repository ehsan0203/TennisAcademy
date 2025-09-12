namespace MTA.Domain.Entities;

public class Lookup : BaseEntity
{
   
    public required string Category { get; set; }
    
    public required string Key { get; set; }
    
    public required string Value { get; set; }
}
