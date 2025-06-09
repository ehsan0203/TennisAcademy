using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisAcademy.Application.Interfaces.Repositories;
using TennisAcademy.Application.Interfaces.Services;
using TennisAcademy.Domain.Entities;

namespace TennisAcademy.Application.Services
{
    public class UserScoreService : IUserScoreService
    {
        private readonly IUserScoreRepository _repository;

        public UserScoreService(IUserScoreRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> GetUserCreditAsync(Guid userId)
        {
            var userScore = await _repository.GetByUserIdAsync(userId);
            return userScore?.Credit ?? 0;
        }

        public async Task AddCreditAsync(Guid userId, int amount)
        {
            var userScore = await _repository.GetByUserIdAsync(userId)
                ?? new UserScore { UserId = userId, Credit = 0 };

            userScore.Credit += amount;
            await _repository.AddOrUpdateAsync(userScore);
        }

        public async Task DecreaseCreditAsync(Guid userId, int amount)
        {
            var userScore = await _repository.GetByUserIdAsync(userId);
            if (userScore == null || userScore.Credit < amount)
                throw new InvalidOperationException("کردیت کافی نیست");

            userScore.Credit -= amount;
            await _repository.AddOrUpdateAsync(userScore);
        }
    }

}
