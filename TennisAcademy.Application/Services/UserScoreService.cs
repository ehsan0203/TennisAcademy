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
        private readonly ICreditHistoryRepository _creditHistoryRepository;

        public UserScoreService(IUserScoreRepository repository, ICreditHistoryRepository creditHistoryRepository)
        {
            _repository = repository;
            _creditHistoryRepository = creditHistoryRepository;
        }

        public async Task<int> GetUserCreditAsync(Guid userId)
        {
            var userScore = await _repository.GetByUserIdAsync(userId);
            return userScore?.Credit ?? 0;
        }

        public async Task AddCreditAsync(Guid userId, int amount)
        {
            var userScore = await _repository.GetByUserIdAsync(userId)
                ?? new UserScore { Id = Guid.NewGuid(), UserId = userId, Credit = 0 };

            userScore.Credit += amount;

            await _creditHistoryRepository.AddAsync(new CreditHistory
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = amount,
                Description = "افزایش توسط ادمین",
                CreatedAt = DateTime.UtcNow
            });

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
