using Gym.DataAccess.Data.Contexts;
using Gym.DataAccess.Data.Seeder.Models;
using Gym.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gym.DataAccess.Data.Seeder
{
    internal static class PlanSeeder
    {
        private const string SeederFileName = "plans.json";

        public static async Task SeedAsync(GymDbContext dbContext)
        {
            if (await dbContext.Plans.AnyAsync())
            {
                return;
            }

            var seederPlans = await SeedJsonLoader.LoadAsync<PlanSeedModel>(SeederFileName);

            if (seederPlans == null || !seederPlans.Any())
            {
                return;
            }

            var plans = seederPlans.Select(p => new Plan
            {
                DurationDays = p.DurationDays,
                Description = p.Description,
                Name = p.Name,
                Price = p.Price,
                IsActive = p.IsActive,
            });

            await dbContext.Plans.AddRangeAsync(plans);
            await dbContext.SaveChangesAsync();
        }
    }
}
