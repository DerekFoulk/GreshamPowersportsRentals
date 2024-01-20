using Data.Husqvarna.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Husqvarna.Contexts
{
    public class HusqvarnaContext : DbContext
    {
        public HusqvarnaContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<HusqvarnaBicycleInfo> BicycleInfos { get; set; }
    }
}
