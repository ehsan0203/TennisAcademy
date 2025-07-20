using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisAcademy.Domain.Enums;

namespace TennisAcademy.Domain.Entities
{
    public class CoachMedia
    {
        public Guid Id { get; set; }

        public Guid CoachId { get; set; }
        public User Coach { get; set; }

        public string Title { get; set; }           // مثلاً "حرکت صحیح بک‌هند"
        public string FileUrl { get; set; }         // آدرس فایل
        public MediaType Type { get; set; }         // Voice یا Video
        public DateTime CreatedAt { get; set; }
    }
}
