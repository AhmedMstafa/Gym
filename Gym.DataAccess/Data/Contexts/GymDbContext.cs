using Microsoft.EntityFrameworkCore;
using Gym.DataAccess.Entities;

namespace Gym.DataAccess.Data.Contexts
{
    public class GymDbContext : DbContext
    {
            public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GymDbContext).Assembly);

            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<User>("User")
                .HasValue<Member>("Member");

            modelBuilder.Entity<User>()
                .HasQueryFilter(u => !u.IsDeleted);


        }

        public DbSet<Plan> Plans { get; set; } 
        public DbSet<Category> Categories { get;  set; }

        public DbSet<User> Users { get;  set; }

        public DbSet<Session> Sessions { get;  set; }

        public DbSet<MemberShip> MemberShips { get;  set; }

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<HealthRecord> HealthRecords { get; set; }


    }
}
