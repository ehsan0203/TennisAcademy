using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisAcademy.Application.DTOs.Purchase;

namespace TennisAcademy.Application.Interfaces.Services
{
    public interface IPurchaseService
    {
        Task<List<PurchaseResultDto>> GetAllAsync();
        Task AddPurchaseAsync(Guid userId, CreatePurchaseDto dto);

        Task<List<PurchaseResultDto>> GetUserPurchasesAsync(Guid userId, string? type);


    }

}
