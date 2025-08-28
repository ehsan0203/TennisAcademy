using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTA.Domain.Entities
{
    public class FAQCategory : BaseEntity
    {
        /// <summary>
        /// عنوان دسته‌بندی
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// توضیحات مربوط به دسته‌بندی (اختیاری)
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// ترتیب نمایش دسته‌بندی‌ها
        /// </summary>
        public int SortOrder { get; set; } = 0;

        /// <summary>
        /// وضعیت فعال یا غیر فعال بودن دسته
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// FAQهای مربوط به این دسته
        /// </summary>
        public virtual ICollection<QuestionFAQ> Questions { get; set; } = new List<QuestionFAQ>();
    }

}
