using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StoreModel.Models
{
    public class Card
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Designer { get; set; }

        [Column(TypeName = "decimal(6, 2)")]
        public decimal Price { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<PublishedCard> PublishedCards { get; set; }
    }
}
