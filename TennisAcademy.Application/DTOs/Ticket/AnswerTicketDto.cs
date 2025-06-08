using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisAcademy.Application.DTOs.Ticket
{
    public class AnswerTicketDto
    {
        public Guid TicketId { get; set; }
        public string? CoachReply { get; set; }

        // فایل‌های آپلود شده مستقیم
        public string? CoachReplyVoiceUrl { get; set; }
        public string? CoachReplyVideoUrl { get; set; }

        // فایل‌های انتخاب شده از فایل‌های آماده مربی
        public Guid? CoachVoiceMediaId { get; set; }
        public Guid? CoachVideoMediaId { get; set; }
    }

}
