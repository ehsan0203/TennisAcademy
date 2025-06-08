using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisAcademy.Application.DTOs.Purchase
{
    public class PurchaseResultDto
    {
        public Guid PurchaseId { get; set; }

        public string Type { get; set; } // Course یا Plan

        public Guid ItemId { get; set; }

        public string Title { get; set; }

        public decimal Price { get; set; }

        public DateTime PurchaseDate { get; set; }
    }

}
