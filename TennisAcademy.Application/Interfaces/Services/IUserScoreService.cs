using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisAcademy.Application.Interfaces.Services
{
    public interface IUserScoreService
    {
        Task<int> GetUserCreditAsync(Guid userId);
        Task DecreaseCreditAsync(Guid userId, int amount);
        Task AddCreditAsync(Guid userId, int amount);
    }

}
