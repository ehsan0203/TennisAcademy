using Microsoft.EntityFrameworkCore;
using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;

namespace MTA.Application.Services.Service;

public class StatisticsService : IStatisticsService
{
    private readonly IUnitOfWork _unitOfWork;

    public StatisticsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SystemStatisticsDto> GetSystemStatisticsAsync(CancellationToken ct = default)
    {
        var pendingStatusId = await _unitOfWork.Repository<Lookup>()
            .GetQueryable()
            .AsNoTracking()
            .Where(l => l.Category == "TicketStatus" && l.Key == "Pending")
            .Select(l => (int?)l.Id)
            .FirstOrDefaultAsync(ct);

        return new SystemStatisticsDto
        {
            RegisteredUsers = await _unitOfWork.Repository<Account>().CountAsync(ct: ct),
            PurchasedCourses = await _unitOfWork.Repository<UserCourseHistory>().CountAsync(ct: ct),
            PurchasedPackages = await _unitOfWork.Repository<PackageHistory>().CountAsync(ct: ct),
            PendingTickets = pendingStatusId.HasValue
                ? await _unitOfWork.Repository<Ticket>().CountAsync(t => t.StatusId == pendingStatusId.Value, ct)
                : 0
        };
    }
}
