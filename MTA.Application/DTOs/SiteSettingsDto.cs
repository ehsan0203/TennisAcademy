namespace MTA.Application.DTOs;

public class SiteImageDto
{
    public string Key { get; set; } = string.Empty;
    public string? Url { get; set; }
}

public class FooterContactItemDto
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public class UpsertFooterContactItemDto
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public class SiteTextDto
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class UpdateSiteTextDto
{
    public string Value { get; set; } = string.Empty;
}
