namespace MTA.Application.DTOs;

/// <summary>
/// Data Transfer Object for UserCourseHistory entity
/// </summary>
public class CreateUserCourseHistoryDto
{
    /// <summary>
    /// Course ID
    /// </summary>
    public int CourseId { get; set; }

    /// <summary>
    /// Account ID
    /// </summary>
    public int AccountId { get; set; }

    /// <summary>
    /// Price paid for the course (if omitted the current course price will be used)
    /// </summary>
    public decimal? PurchasePrice { get; set; }
}

public class UpdateUserCourseHistoryDto
{
    public int Id { get; set; }
    /// <summary>
    /// Course ID
    /// </summary>
    public int CourseId { get; set; }

    /// <summary>
    /// Account ID
    /// </summary>
    public int AccountId { get; set; }

    /// <summary>
    /// وضعیت دوره برای کاربر (اختیاری)
    /// </summary>
    public int StatusId { get; set; }

    /// <summary>
    /// Price that the user paid when enrolling in the course
    /// </summary>
    public decimal? PurchasePrice { get; set; }
}


