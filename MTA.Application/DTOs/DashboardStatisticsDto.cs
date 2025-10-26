using System.Collections.Generic;
using System.Linq;

namespace MTA.Application.DTOs;

/// <summary>
/// Represents aggregated dashboard statistics for administrative insights.
/// </summary>
public class DashboardStatisticsDto
{
    /// <summary>
    /// Total number of registered users.
    /// </summary>
    public int TotalUsers { get; set; }

    /// <summary>
    /// Total number of purchased courses.
    /// </summary>
    public int TotalCoursePurchases { get; set; }

    /// <summary>
    /// Total number of purchased packages.
    /// </summary>
    public int TotalPackagePurchases { get; set; }

    /// <summary>
    /// Total number of received support tickets.
    /// </summary>
    public int TotalTickets { get; set; }

    /// <summary>
    /// Users with the highest activity in purchasing packages.
    /// </summary>
    public IEnumerable<TopUserPurchaseDto> TopPackageBuyers { get; set; } = Enumerable.Empty<TopUserPurchaseDto>();

    /// <summary>
    /// Users with the highest activity in purchasing courses.
    /// </summary>
    public IEnumerable<TopUserPurchaseDto> TopCourseBuyers { get; set; } = Enumerable.Empty<TopUserPurchaseDto>();
}

/// <summary>
/// Represents a single user's aggregated purchase information.
/// </summary>
public class TopUserPurchaseDto
{
    /// <summary>
    /// Unique identifier of the account.
    /// </summary>
    public int AccountId { get; set; }

    /// <summary>
    /// First name of the user.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Last name of the user.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Number of purchases made by the user.
    /// </summary>
    public int PurchaseCount { get; set; }
}
