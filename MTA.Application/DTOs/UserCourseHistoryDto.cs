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

/// <summary>
/// Detailed representation of a course purchase made by a user
/// </summary>
public class UserCourseHistoryDetailDto : BaseDto
{
    /// <summary>
    /// Course ID
    /// </summary>
    public int CourseId { get; set; }

    /// <summary>
    /// Course title
    /// </summary>
    public string? CourseTitle { get; set; }

    /// <summary>
    /// Account ID of the purchaser
    /// </summary>
    public int AccountId { get; set; }

    /// <summary>
    /// First name of the purchaser
    /// </summary>
    public string? UserFirstName { get; set; }

    /// <summary>
    /// Last name of the purchaser
    /// </summary>
    public string? UserLastName { get; set; }

    /// <summary>
    /// Date when the course was purchased
    /// </summary>
    public DateTime PurchasedAt { get; set; }

    /// <summary>
    /// Price that the user paid when enrolling in the course
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


