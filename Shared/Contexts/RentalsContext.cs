using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Shared.Contexts
{
    public class RentalsContext : DbContext
    {
        public RentalsContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Bike> Bikes { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Manufacturer> Manufacturers { get; set; }

        public DbSet<Model> Models { get; set; }

        public DbSet<Rental> Rentals { get; set; }
    }
}
