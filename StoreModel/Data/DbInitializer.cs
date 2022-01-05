using StoreModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreModel.Data
{
    public class DbInitializer
    {
        public static void Initialize(LibraryContext context)
        {
            context.Database.EnsureCreated();
            if (context.Cards.Any())
            {
                return;
            }
            var cards = new Card[]
            {
                 new Card{Title="La multi ani",Designer="Alxandru Ion",Price=Decimal.Parse("22")},
                 new Card{Title="La multi ani!",Designer="George Mihailescu",Price=Decimal.Parse("18")},
                 new Card{Title="Craciun fericit!",Designer="Steluta Maria",Price=Decimal.Parse("27")},
                 new Card{Title="An nou fericit!!",Designer="NumeDeCod",Price=Decimal.Parse("45")},
                 new Card{Title="La multi ani :D",Designer="Vasile Gheorghe",Price=Decimal.Parse("38")},
                 new Card{Title="Paste fericit!",Designer="Paul Daniel",Price=Decimal.Parse("28")},
            };
            foreach (Card b in cards)
            {
                context.Cards.Add(b);
            }
            context.SaveChanges();
            var customers = new Customer[]
            {
                 new Customer{CustomerID=1050,Name="Mitrea Danila",BirthDate=DateTime.Parse("1979-09-01")},
                 new Customer{CustomerID=1045,Name="Vasiliu Vasile",BirthDate=DateTime.Parse("1845-07-08")},
            };
            foreach (Customer c in customers)
            {
                context.Customers.Add(c);
            }
            context.SaveChanges();
            var orders = new Order[]
            {
                 new Order{CardID=1,CustomerID=1050,OrderDate=DateTime.Parse("02-25-2020")},
                 new Order{CardID=3,CustomerID=1045,OrderDate=DateTime.Parse("09-28-2020")},
                 new Order{CardID=1,CustomerID=1045,OrderDate=DateTime.Parse("10-28-2020")},
                 new Order{CardID=2,CustomerID=1050,OrderDate=DateTime.Parse("09-28-2020")},
                 new Order{CardID=4,CustomerID=1050,OrderDate=DateTime.Parse("09-28-2020")},
                 new Order{CardID=6,CustomerID=1050,OrderDate=DateTime.Parse("10-28-2020")},
            };
            foreach (Order e in orders)
            {
                context.Orders.Add(e);
            }
            context.SaveChanges();
            var publishers = new Publisher[]
            {
                new Publisher{PublisherName="Publisher1",Adress="Str. Numarul 1, nr. 2, Galati"},
                new Publisher{PublisherName="Publisher3",Adress="Str. Numarul2, nr. 34,Vaslui"},
                new Publisher{PublisherName="Publisher3",Adress="Str. NUmarul3, nr.22, Bucuresti"},
            };
            foreach (Publisher p in publishers)
            {
                context.Publishers.Add(p);
            }
            context.SaveChanges();
            var publishedCards = new PublishedCard[]
            {
                 new PublishedCard { CardID = cards.Single(c => c.Title == "La multi ani" ).ID, PublisherID = publishers.Single(i => i.PublisherName =="Publisher1").ID },
                 new PublishedCard { CardID = cards.Single(c => c.Title == "An nout fericit" ).ID,PublisherID = publishers.Single(i => i.PublisherName =="Publisher1").ID },
                 new PublishedCard { CardID = cards.Single(c => c.Title == "La multi ani :D" ).ID, PublisherID = publishers.Single(i => i.PublisherName =="Publisher3").ID },
                 new PublishedCard { CardID = cards.Single(c => c.Title == "La multi ani!!!" ).ID,PublisherID = publishers.Single(i => i.PublisherName == "Publisher3").ID },
                 new PublishedCard {CardID = cards.Single(c => c.Title == "Craciun Fericit!" ).ID,PublisherID = publishers.Single(i => i.PublisherName == "Publisher3").ID },
                 new PublishedCard {CardID = cards.Single(c => c.Title == "Paste fericit!" ).ID, PublisherID = publishers.Single(i => i.PublisherName == "Publisher3").ID},
            };
            foreach (PublishedCard pb in publishedCards)
            {
                context.PublishedCards.Add(pb);
            }
            context.SaveChanges();
        }
    }
}
