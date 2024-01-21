using Microsoft.EntityFrameworkCore;
using Model = Data.Specialized.Entities.Model;

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
