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
}


