using Microsoft.EntityFrameworkCore;
using Gym.DataAccess.Models;

namespace Gym.DataAccess.Data.Contexts
{
    public class GymDbContext : DbContext
    {
    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GymDbContext).Assembly);
        }

        public DbSet<Plan> Plans { get; set; } = default!;
    }
}
