using StoreModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anegrului_Cosmina_Proiect.Models.LibraryViewModels
{
    public class PublisherIndexData
    {
        public IEnumerable<Publisher> Publishers { get; set; }
        public IEnumerable<Card> Cards { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }
}
