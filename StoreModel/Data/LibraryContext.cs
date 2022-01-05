using StoreModel.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreModel.Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
        {
        }

        public DbSet<Card> Cards { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<PublishedCard> PublishedCards { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<Card>().ToTable("Card");
            modelBuilder.Entity<Publisher>().ToTable("Publisher");
            modelBuilder.Entity<PublishedCard>().ToTable("PublishedCard");
            modelBuilder.Entity<PublishedCard>()
                .HasKey(c => new { c.CardID, c.PublisherID });
        }
    }
}
