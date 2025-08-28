namespace MTA.Application.DTOs.Course;

/// <summary>
/// Data Transfer Object for filtering courses
/// </summary>
public class CourseFilterDto
{
    /// <summary>
    /// Page number for pagination
    /// </summary>
    public int Page { get; set; } = 1;
    
    /// <summary>
    /// Page size for pagination
    /// </summary>
    public int PageSize { get; set; } = 10;
    
    /// <summary>
    /// Search term for title or description
    /// </summary>
    public string? SearchTerm { get; set; }
    
    /// <summary>
    /// Filter by level ID
    /// </summary>
    public int? LevelId { get; set; }
    
    /// <summary>
    /// Filter by status ID
    /// </summary>
    public int? StatusId { get; set; }
    
    /// <summary>
    /// Filter by minimum price
    /// </summary>
    public decimal? MinPrice { get; set; }
    
    /// <summary>
    /// Filter by maximum price
    /// </summary>
    public decimal? MaxPrice { get; set; }
    
    /// <summary>
    /// Filter by free courses only
    /// </summary>
    public bool? FreeOnly { get; set; }
    
    /// <summary>
    /// Sort by field (Title, Price, CreatedAt, etc.)
    /// </summary>
    public string? SortBy { get; set; }
    
    /// <summary>
    /// Sort direction (asc or desc)
    /// </summary>
    public string? SortDirection { get; set; } = "asc";
}
