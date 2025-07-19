using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisAcademy.Application.Interfaces.Repositories;
using TennisAcademy.Application.Interfaces.Services;
using TennisAcademy.Domain.Entities;
using BuildingBlocks.Exceptions;

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
            var media = await _repository.GetByCoachIdAsync(coachId);
            if (media == null || !media.Any())
                throw new NotFoundException("No coach media found.");
            return media;
        }

        public async Task AddAsync(CoachMedia media)
        {
            await _repository.AddAsync(media);
            await _repository.SaveChangesAsync();
        }
    }

}
