using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MTA.Application.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string type, string? placement = null);
        Task<bool> DeleteFileAsync(string relativePath);
    }

    public class FileStorageService : IFileStorageService
    {
        private readonly string _rootPath;
        private readonly ILogger<FileStorageService> _logger;

        public FileStorageService(ILogger<FileStorageService> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _rootPath = Path.Combine(env.WebRootPath); 
            if (!Directory.Exists(_rootPath))
                Directory.CreateDirectory(_rootPath);
        }


        public async Task<string> SaveFileAsync(IFormFile file, string mediaType, string? mediaPlacement = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            var folderPath = string.IsNullOrWhiteSpace(mediaPlacement)
                ? Path.Combine(_rootPath, mediaType)
                : Path.Combine(_rootPath, mediaType, mediaPlacement);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileExtension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var fullPath = Path.Combine(folderPath, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = Path.GetRelativePath(_rootPath, fullPath);
            return relativePath.Replace("\\", "/");
        }


        public Task<bool> DeleteFileAsync(string relativePath)
        {
            try
            {
                var fullPath = Path.Combine(_rootPath, relativePath);
                if (File.Exists(fullPath))
                    File.Delete(fullPath);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting file: {relativePath}");
                return Task.FromResult(false);
            }
        }
    }
}
