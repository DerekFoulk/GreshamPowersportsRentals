using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Shared.Contexts
{
    public class RentalsContext : DbContext
    {
        public RentalsContext(DbContextOptions options) : base(options)
        {
        }
    }
}
