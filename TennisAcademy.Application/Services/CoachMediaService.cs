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
    public class CoachMediaService : ICoachMediaService
    {
        private readonly ICoachMediaRepository _repository;

        public CoachMediaService(ICoachMediaRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CoachMedia>> GetByCoachAsync(Guid coachId)
        {
            return await _repository.GetByCoachIdAsync(coachId);
        }

        public async Task AddAsync(CoachMedia media)
        {
            await _repository.AddAsync(media);
            await _repository.SaveChangesAsync();
        }
    }

}
