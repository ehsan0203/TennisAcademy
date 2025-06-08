using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisAcademy.Domain.Enums;

namespace TennisAcademy.Application.DTOs.CoachMedia
{
    public class CreateCoachMediaDto
    {
        public Guid CoachId { get; set; }
        public string Title { get; set; }
        public string FileUrl { get; set; }
        public MediaType Type { get; set; }
    }
}
