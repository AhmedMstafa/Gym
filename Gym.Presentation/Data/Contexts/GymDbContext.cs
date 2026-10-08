using Microsoft.EntityFrameworkCore;
using Gym.Presentation.Models;

namespace Gym.Presentation.Data.Contexts
{
    public class GymDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.; Database=GymTestProject; Trusted_Connection=true; TrustServerCertificate=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GymDbContext).Assembly);
        }

        public DbSet<Plan> Plans { get; set; } = default!;
    }
}
