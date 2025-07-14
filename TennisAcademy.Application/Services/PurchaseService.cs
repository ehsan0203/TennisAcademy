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

        public PurchaseService(IPurchaseRepository purchaseRepo)
        {
            _purchaseRepo = purchaseRepo;
        }

        public async Task AddPurchaseAsync(Guid userId, CreatePurchaseDto dto)
        {
            var purchase = new Purchase
            {
                Id = Guid.NewGuid(),
                UserId = userId, // 👈 حالا مستقیم از پارامتر
                CourseId = dto.CourseId,
                PlanId = dto.PlanId,
                PurchaseDate = DateTime.UtcNow
            };

            await _purchaseRepo.AddAsync(purchase);
            await _purchaseRepo.SaveChangesAsync();
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


