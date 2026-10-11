using Gym.DataAccess.Data.Contexts;
using Gym.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gym.DataAccess.Data.Seeder
{
    internal static class CategorySeeder
    {
        public static async Task SeedAsync(GymDbContext dbContext)
        {
            if (!dbContext.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Strength Training" },
                    new Category { Name = "Cardio" },
                    new Category { Name = "Flexibility" },
                    new Category { Name = "Balance" },
                    new Category { Name = "Endurance" }
                };
                await dbContext.Categories.AddRangeAsync(categories);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
