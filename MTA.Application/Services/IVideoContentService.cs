
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTA.Application.Services
{
    public interface IVideoContentService
    {
        Task<IEnumerable<VideoContent>> GetAllAsync();
        Task<VideoContent?> GetByIdAsync(int id);
        Task<VideoContent> CreateAsync(VideoContent video);
        Task<VideoContent> UpdateAsync(VideoContent video);
        Task<bool> DeleteAsync(int id);
    }

    public class VideoContentService : IVideoContentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VideoContentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<VideoContent>> GetAllAsync() =>
            await _unitOfWork.Repository<VideoContent>().GetAllAsync();

        public async Task<VideoContent?> GetByIdAsync(int id) =>
            await _unitOfWork.Repository<VideoContent>().GetByIdAsync(id);

        public async Task<VideoContent> CreateAsync(VideoContent video)
        {
            await _unitOfWork.Repository<VideoContent>().AddAsync(video);
            await _unitOfWork.SaveChangesAsync();
            return video;
        }

        public async Task<VideoContent> UpdateAsync(VideoContent video)
        {
            _unitOfWork.Repository<VideoContent>().UpdateAsync(video);
            await _unitOfWork.SaveChangesAsync();
            return video;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var repo = _unitOfWork.Repository<VideoContent>();
            var entity = await repo.GetByIdAsync(id);
            if (entity == null) return false;
            repo.DeleteAsync(entity.Id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }


}
