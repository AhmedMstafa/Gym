using Gym.Presentation.Data.Contexts;
using Gym.Presentation.Models;
using Microsoft.EntityFrameworkCore;

namespace Gym.Presentation.Data.Seeder
{
    internal static class PlanSeeder
    {
        public static async Task SeedAsync()
        {
            using var dbContext = new GymDbContext();

           bool hasPlans = await dbContext.Plans.AnyAsync();

            if(hasPlans)
            {
                return; // Plans already seeded
            };

            var plans = new List<Plan>
            {
                new Plan
                {
                    Name = "Basic Plan",
                    Price = 300,
                    DurationDays  = 30,
                    Description = "Access to gym equipment during staffed hours"
                },
                new Plan
                {
                    Name = "Standard Plan",
                    Price = 500,
                    DurationDays = 60,
                    Description = "Includes gym equipment and 2 group classes per week"
                },
                new Plan
                {
                    Name = "Premium Plan",
                    Price = 900,
                    DurationDays = 90,
                    Description = "Unlimited access to equipment, classes, and sauna"
                },
                new Plan
                {
                    Name = "Annual Plan",
                    Price = 3000,
                    DurationDays = 365,
                    Description = "Full year access with personal trainer sessions"
                }
            };

            await dbContext.Plans.AddRangeAsync(plans);
            await dbContext.SaveChangesAsync();
        }
    }
}