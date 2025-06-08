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
        private readonly JwtTokenGenerator _jwt;

        public AuthService(IUserRepository userRepo, JwtTokenGenerator jwt)
        {
            _userRepo = userRepo;
            _jwt = jwt;
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userRepo.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new Exception("Email is already registered.");

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
                TennisExperienceYears = dto.TennisExperienceYears
            };


            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();

            var token = _jwt.GenerateToken(user);

            return new AuthResultDto
            {
                Token = token,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString()
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
                throw new Exception("Invalid email or password.");

            var token = _jwt.GenerateToken(user);

            return new AuthResultDto
            {
                Token = token,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString()
            };
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hash = Convert.ToBase64String(bytes);
            return hash == storedHash;
        }

    }
}
