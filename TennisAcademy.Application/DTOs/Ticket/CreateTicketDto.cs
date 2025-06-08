using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisAcademy.Application.DTOs.Ticket
{
    public class CreateTicketDto
    {
        public string Subject { get; set; }      // مثل "فورهند"
        public string Description { get; set; }
        public string? FileUrl { get; set; }     // آدرس فایل ویدیویی یا تصویری
        public string? VoiceUrl { get; set; }    // آدرس ویس

        public Guid UserId { get; set; }
        public Guid? CoachId { get; set; }       // قابل انتخاب هنگام ارسال
    }
}
