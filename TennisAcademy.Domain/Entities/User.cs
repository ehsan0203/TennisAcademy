using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TennisAcademy.Domain.Enums;

namespace TennisAcademy.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public DateTime BirthYear { get; set; }

        public string? Description { get; set; }

        public int? TennisLevel { get; set; }
        public int? TennisExperienceYears { get; set; }

        public UserRole Role { get; set; } = UserRole.User;

        public int Credit { get; set; } = 0;

        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<Purchase> Purchases { get; set; }
        public UserScore UserScore { get; set; }
    }

}
