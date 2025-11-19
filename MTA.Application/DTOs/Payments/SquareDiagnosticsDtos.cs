namespace MTA.Application.DTOs;

public class LocationDiagnosticsDto
{
    public bool Ok { get; set; }
    public string Environment { get; set; } = string.Empty;
    public string ConfiguredLocationId { get; set; } = string.Empty;
    public bool LocationFound { get; set; }
    public string? MatchedLocationName { get; set; }
    public string? Message { get; set; }
}

