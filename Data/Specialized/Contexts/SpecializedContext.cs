using Data.Specialized.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Specialized.Contexts
{
    public class SpecializedContext : DbContext
    {
        public SpecializedContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Model> Models { get; set; }
    }
}
