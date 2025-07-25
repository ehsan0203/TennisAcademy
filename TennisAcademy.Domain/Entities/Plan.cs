using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisAcademy.Domain.Entities
{
    public class Plan
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        /// <summary>
        /// Amount of credit granted to the user after purchasing this plan
        /// </summary>
        public int Credit { get; set; }

        public ICollection<Purchase> Purchases { get; set; }
    }

}
