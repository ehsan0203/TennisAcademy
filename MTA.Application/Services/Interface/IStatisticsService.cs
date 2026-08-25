using MTA.Application.DTOs;

namespace MTA.Application.Services;

public interface IStatisticsService
{
    Task<SystemStatisticsDto> GetSystemStatisticsAsync(CancellationToken ct = default);
}
