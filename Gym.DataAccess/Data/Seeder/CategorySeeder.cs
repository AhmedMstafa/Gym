using Gym.DataAccess.Data.Contexts;
using Gym.DataAccess.Entities;
using Gym.DataAccess.Enums;
using Microsoft.EntityFrameworkCore;

namespace Gym.DataAccess.Data.Seeder
{
    internal static class CategorySeeder
    {
        public static async Task SeedAsync(GymDbContext dbContext)
        {
            if (await dbContext.Categories.AnyAsync())
            {
                return;
            }

            var categories = new List<Category>
            {
                new Category { Name = "Strength Training", RequiredSpeciality = Speciality.Fitness },
                new Category { Name = "Cardio", RequiredSpeciality = Speciality.Fitness },
                new Category { Name = "Flexibility", RequiredSpeciality = Speciality.Yoga },
                new Category { Name = "Balance", RequiredSpeciality = Speciality.Pilates },
                new Category { Name = "Endurance", RequiredSpeciality = Speciality.Fitness }
            };

            await dbContext.Categories.AddRangeAsync(categories);
            await dbContext.SaveChangesAsync();
        }
    }
}
