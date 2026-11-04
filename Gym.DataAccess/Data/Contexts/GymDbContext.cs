using Microsoft.EntityFrameworkCore;
using Gym.DataAccess.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Gym.DataAccess.Entities.Identity;

namespace Gym.DataAccess.Data.Contexts
{
    public class GymDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,Guid>
    {
            public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GymDbContext).Assembly);
            base.OnModelCreating(modelBuilder);

        }

        public DbSet<Plan> Plans { get; set; } 
        public DbSet<Category> Categories { get;  set; }

        public DbSet<User> Users { get;  set; }

        public DbSet<Member> Members { get; set; }

        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Session> Sessions { get;  set; }

        public DbSet<MemberShip> MemberShips { get;  set; }

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<HealthRecord> HealthRecords { get; set; }


    }
}
