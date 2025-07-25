using BuildingBlocks.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisAcademy.Application.DTOs.Purchase;
using TennisAcademy.Application.Interfaces.Repositories;
using TennisAcademy.Application.Interfaces.Services;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.Application.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IPurchaseRepository _purchaseRepo;
        private readonly IPlanRepository _planRepository;
        private readonly IUserScoreService _userScoreService;

        public PurchaseService(IPurchaseRepository purchaseRepo,
            IPlanRepository planRepository,
            IUserScoreService userScoreService)
        {
            _purchaseRepo = purchaseRepo;
            _planRepository = planRepository;
            _userScoreService = userScoreService;
        }

        public async Task AddPurchaseAsync(Guid userId, CreatePurchaseDto dto)
        {
            var purchase = new Purchase
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CourseId = dto.CourseId,
                PlanId = dto.PlanId,
                PurchaseDate = DateTime.UtcNow
            };

            await _purchaseRepo.AddAsync(purchase);
            await _purchaseRepo.SaveChangesAsync();

            if (dto.PlanId.HasValue)
            {
                var plan = await _planRepository.GetByIdAsync(dto.PlanId.Value);
                if (plan == null)
                    throw new NotFoundException("Plan not found.");

                await _userScoreService.AddCreditAsync(userId, plan.Credit);
            }
        }

        public async Task<List<PurchaseResultDto>> GetUserPurchasesAsync(Guid userId, string? type)
        {
            var purchases = await _purchaseRepo.GetByUserIdAsync(userId);
            if (purchases == null || !purchases.Any())
                throw new NotFoundException("No purchases found.");
            var result = purchases.Select(p =>
            {
                if (p.Course != null)
                {
                    return new PurchaseResultDto
                    {
                        PurchaseId = p.Id,
                        Type = "Course",
                        ItemId = p.Course.Id,
                        Title = p.Course.Title,
                        Price = p.Course.Price,
                        PurchaseDate = p.PurchaseDate
                    };
                }
                else if (p.Plan != null)
                {
                    return new PurchaseResultDto
                    {
                        PurchaseId = p.Id,
                        Type = "Plan",
                        ItemId = p.Plan.Id,
                        Title = p.Plan.Title,
                        Price = p.Plan.Price,
                        PurchaseDate = p.PurchaseDate
                    };
                }

                return null;
            }).Where(x => x != null).ToList()!;

            // ✅ اعمال فیلتر
            if (!string.IsNullOrEmpty(type))
            {
                result = result
                    .Where(r => r.Type.Equals(type, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return result;
        }
        public async Task<List<PurchaseResultDto>> GetAllAsync()
        {
            var purchases = await _purchaseRepo.GetAllAsync();
            if (purchases == null || !purchases.Any())
                throw new NotFoundException("No purchases found.");
            var result = purchases.Select(p =>
            {
                if (p.Course != null)
                {
                    return new PurchaseResultDto
                    {
                        PurchaseId = p.Id,
                        Type = "Course",
                        ItemId = p.Course.Id,
                        Title = p.Course.Title,
                        Price = p.Course.Price,
                        PurchaseDate = p.PurchaseDate
                    };
                }
                else if (p.Plan != null)
                {
                    return new PurchaseResultDto
                    {
                        PurchaseId = p.Id,
                        Type = "Plan",
                        ItemId = p.Plan.Id,
                        Title = p.Plan.Title,
                        Price = p.Plan.Price,
                        PurchaseDate = p.PurchaseDate
                    };
                }

                return null;
            }).Where(x => x != null).ToList()!;

            return result;
        }

    }
}


