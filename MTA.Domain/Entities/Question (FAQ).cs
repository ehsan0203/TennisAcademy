using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTA.Domain.Entities
{
    public class QuestionFAQ:BaseEntity
    {
        public string QuestionText { get; set; }
        public string AnswerText { get; set; }
        public bool IsActive { get; set; }

        /// <summary>
        /// ForeignKey به جدول دسته‌بندی
        /// </summary>
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual FAQCategory Category { get; set; } = null!;
    }
}
