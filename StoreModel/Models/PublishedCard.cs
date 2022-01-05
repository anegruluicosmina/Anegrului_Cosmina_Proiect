using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreModel.Models
{
    public class PublishedCard
    {
        public int PublisherID { get; set; }
        public int CardID { get; set; }
        public Publisher Publisher { get; set; }
        public Card Card { get; set; }
    }
}
