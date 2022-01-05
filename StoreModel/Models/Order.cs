using System;

namespace StoreModel.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public int CardID { get; set; }
        public Customer Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public Card Card { get; set; }
    }
}