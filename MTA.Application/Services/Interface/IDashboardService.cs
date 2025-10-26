using MTA.Application.DTOs;

namespace MTA.Application.Services;

/// <summary>
/// Provides methods for retrieving aggregated dashboard statistics.
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Retrieves dashboard statistics including user and purchase insights.
    /// </summary>
    /// <param name="topCount">Number of top users to include in rankings.</param>
    /// <returns>The populated dashboard statistics DTO.</returns>
    Task<DashboardStatisticsDto> GetStatisticsAsync(int topCount = 5);
}
