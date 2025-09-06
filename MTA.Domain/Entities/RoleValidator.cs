using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTA.Domain.Entities
{
    public class RoleValidator : AbstractValidator<Role>
    {
        public RoleValidator()
        {
            RuleFor(r => r.Title)
                .NotEmpty().WithMessage("Role title cannot be empty.")
                .Must(t => !string.IsNullOrWhiteSpace(t))
                .WithMessage("Role title cannot be null or whitespace.")
                .Must(t => t.ToLower() != "student")
                .WithMessage("Default role 'student' should not be created manually.");
        }
    }

    public class GetStudentRoleValidator : AbstractValidator<IEnumerable<Role>>
    {
        public GetStudentRoleValidator()
        {
            RuleFor(r => r.FirstOrDefault(x => x.Title.ToLower() == "student"))
                .NotNull().WithMessage("Default role 'student' not found in database.");
        }
    }
}
