using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisAcademy.Application.DTOs.CoachMedia
{
    public class CoachMediaDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string FileUrl { get; set; }
        public string Type { get; set; }
    }
}
