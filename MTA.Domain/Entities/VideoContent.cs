using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTA.Domain.Entities
{
    public class VideoContent:BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public bool IsVideo => Extension.ToLower() is "mp4" or "mov" or "avi";
    }

}
