namespace MTA.Application.DTOs;

/// <summary>
/// Data Transfer Object for Lookup entity
/// </summary>
public class LookupDto
{
    public int Id { get; set; }
    public string Category { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}

public class CreateLookupDto
{
    public int Id { get; set; }
    public string Category { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}
