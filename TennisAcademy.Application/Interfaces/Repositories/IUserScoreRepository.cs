using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.Application.Interfaces.Repositories
{
    public interface IUserScoreRepository
    {
        Task<UserScore> GetByUserIdAsync(Guid userId);
        Task AddOrUpdateAsync(UserScore userScore);
    }

}
