using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisAcademy.Application.DTOs.Auth
{
    public class RegisterDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public DateTime BirthYear { get; set; }

        public string? Description { get; set; }
        public int? TennisLevel { get; set; }
        public int? TennisExperienceYears { get; set; }

        public string Role { get; set; } = "User";
    }


}
