using Gym.DataAccess.Data.Contexts;

namespace Gym.DataAccess.Data.Seeder
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAllAsync(GymDbContext dbContext)
        {
            await PlanSeeder.SeedAsync(dbContext);
        }
    }
}