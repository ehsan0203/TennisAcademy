using FluentValidation;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTA.Application.Services
{
    public class RoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<IEnumerable<Role>> _studentRoleValidator;

        public RoleService(IUnitOfWork unitOfWork, IValidator<IEnumerable<Role>> studentRoleValidator)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _studentRoleValidator = studentRoleValidator ?? throw new ArgumentNullException(nameof(studentRoleValidator));
        }

        /// <summary>
        /// Get the default role "Student"
        /// </summary>
        /// <returns>Role entity</returns>
        /// <exception cref="ValidationException">Thrown if validation fails</exception>
        /// <exception cref="InvalidOperationException">Thrown if role not found or duplicates exist</exception>
        public async Task<Role> GetDefaultStudentRoleAsync()
        {
            var roles = await _unitOfWork.Repository<Role>().GetAllAsync() ?? new List<Role>();

            var role = roles.FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.Title) && r.Title.ToLower() == "student");

            if (role == null)
            {
                role = roles.FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.Title) &&
                                                 (r.Title.ToLower() == "user" || r.Title.ToLower() == "کاربر"));
            }

            if (role == null)
                throw new InvalidOperationException("No suitable default role found (student or user).");

            return role;
        }


    }
}
