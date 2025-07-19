using BuildingBlocks.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TennisAcademy.Application.DTOs.Auth;
using TennisAcademy.Application.Helpers;
using TennisAcademy.Application.Interfaces.Repositories;
using TennisAcademy.Application.Interfaces.Services;
using TennisAcademy.Domain.Entities;
using TennisAcademy.Domain.Enums;

namespace TennisAcademy.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserScoreRepository _userScoreRepository;
        private readonly JwtTokenGenerator _jwt;
        private readonly JwtSettings _settings;
        public AuthService(IUserRepository userRepo, JwtTokenGenerator jwt, IUserScoreRepository userScoreRepository, Microsoft.Extensions.Options.IOptions<JwtSettings> options)
        {
            _userRepo = userRepo;
            _jwt = jwt;
            _userScoreRepository = userScoreRepository;
            _settings = options.Value;
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userRepo.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new BadRequestException("Email is already registered.");
              

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                Role = Enum.Parse<UserRole>(dto.Role, ignoreCase: true),
                BirthYear = dto.BirthYear,
                TennisLevel = dto.TennisLevel,
                TennisExperienceYears = dto.TennisExperienceYears,
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpireDays)
            };


            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();
            // ساخت UserScore با 1 کردیت
            var userScore = new UserScore
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Credit = 1
            };

            await _userScoreRepository.AddOrUpdateAsync(userScore); 

            var token = _jwt.GenerateToken(user);

            return new AuthResultDto
            {
                Token = token,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                RefreshToken = user.RefreshToken
            };

        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email);

            if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
                throw new FileNotFoundException("Invalid email or password.");

            var token = _jwt.GenerateToken(user);
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpireDays);
            _userRepo.Update(user);
            await _userRepo.SaveChangesAsync();

            return new AuthResultDto
            {
                Token = token,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                RefreshToken = user.RefreshToken
            };
        }

        public async Task<AuthResultDto> RefreshTokenAsync(string token, string refreshToken)
        {
            var user = await _userRepo.GetByRefreshTokenAsync(refreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid refresh token.");

            var newToken = _jwt.GenerateToken(user);
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpireDays);
            _userRepo.Update(user);
            await _userRepo.SaveChangesAsync();

            return new AuthResultDto
            {
                Token = newToken,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                RefreshToken = user.RefreshToken
            };
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hash = Convert.ToBase64String(bytes);
            return hash == storedHash;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

    }
}
