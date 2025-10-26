using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MTA.Application.DTOs;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;

namespace MTA.Application.Services;

/// <summary>
/// Service responsible for providing dashboard statistics.
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(IUnitOfWork unitOfWork, ILogger<DashboardService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<DashboardStatisticsDto> GetStatisticsAsync(int topCount = 5)
    {
        if (topCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(topCount), "Top count must be greater than zero.");
        }

        try
        {
            var totalUsersTask = _unitOfWork.Repository<Account>()
                .GetQueryable()
                .AsNoTracking()
                .CountAsync();

            var courseQuery = _unitOfWork.Repository<UserCourseHistory>()
                .GetQueryable()
                .AsNoTracking();

            var packageQuery = _unitOfWork.Repository<PackageHistory>()
                .GetQueryable()
                .AsNoTracking();

            var totalCoursePurchasesTask = courseQuery.CountAsync();
            var totalPackagePurchasesTask = packageQuery.CountAsync();

            var totalTicketsTask = _unitOfWork.Repository<Ticket>()
                .GetQueryable()
                .AsNoTracking()
                .CountAsync();

            var topPackageBuyersTask = packageQuery
                .GroupBy(ph => ph.AccountId)
                .Select(group => new TopUserAggregate(group.Key, group.Count()))
                .OrderByDescending(result => result.PurchaseCount)
                .ThenBy(result => result.AccountId)
                .Take(topCount)
                .ToListAsync();

            var topCourseBuyersTask = courseQuery
                .GroupBy(uch => uch.AccountId)
                .Select(group => new TopUserAggregate(group.Key, group.Count()))
                .OrderByDescending(result => result.PurchaseCount)
                .ThenBy(result => result.AccountId)
                .Take(topCount)
                .ToListAsync();

            await Task.WhenAll(
                totalUsersTask,
                totalCoursePurchasesTask,
                totalPackagePurchasesTask,
                totalTicketsTask,
                topPackageBuyersTask,
                topCourseBuyersTask);

            var aggregatedAccounts = topPackageBuyersTask.Result
                .Concat(topCourseBuyersTask.Result)
                .Select(item => item.AccountId)
                .Distinct()
                .ToList();

            var accountIdentities = aggregatedAccounts.Count > 0
                ? await _unitOfWork.Repository<Account>()
                    .GetQueryable()
                    .AsNoTracking()
                    .Where(account => aggregatedAccounts.Contains(account.Id))
                    .Select(account => new AccountIdentity(
                        account.Id,
                        account.UserProfile != null ? account.UserProfile.FirstName : null,
                        account.UserProfile != null ? account.UserProfile.LastName : null))
                    .ToListAsync()
                : new List<AccountIdentity>();

            var accountLookup = accountIdentities.ToDictionary(identity => identity.AccountId);

            return new DashboardStatisticsDto
            {
                TotalUsers = totalUsersTask.Result,
                TotalCoursePurchases = totalCoursePurchasesTask.Result,
                TotalPackagePurchases = totalPackagePurchasesTask.Result,
                TotalTickets = totalTicketsTask.Result,
                TopPackageBuyers = MapAggregates(topPackageBuyersTask.Result, accountLookup),
                TopCourseBuyers = MapAggregates(topCourseBuyersTask.Result, accountLookup)
            };
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to retrieve dashboard statistics.");
            throw;
        }
    }

    private static IEnumerable<TopUserPurchaseDto> MapAggregates(
        IEnumerable<TopUserAggregate> aggregates,
        IReadOnlyDictionary<int, AccountIdentity> accountLookup) =>
        aggregates.Select(aggregate =>
        {
            accountLookup.TryGetValue(aggregate.AccountId, out var identity);

            return new TopUserPurchaseDto
            {
                AccountId = aggregate.AccountId,
                FirstName = identity?.FirstName,
                LastName = identity?.LastName,
                PurchaseCount = aggregate.PurchaseCount
            };
        });

    private sealed record TopUserAggregate(int AccountId, int PurchaseCount);

    private sealed record AccountIdentity(int AccountId, string? FirstName, string? LastName);
}
