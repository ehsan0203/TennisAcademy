namespace MTA.Application.DTOs;

/// <summary>
/// Basic system-wide aggregate counts
/// </summary>
public class SystemStatisticsDto
{
    public int RegisteredUsers { get; set; }
    public int PurchasedCourses { get; set; }
    public int PurchasedPackages { get; set; }
    public int PendingTickets { get; set; }
}
