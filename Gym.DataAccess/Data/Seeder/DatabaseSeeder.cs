using Gym.DataAccess.Data.Contexts;
using Gym.DataAccess.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Gym.DataAccess.Data.Seeder
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAllAsync(UserManager<ApplicationUser>userManager, RoleManager<ApplicationRole> roleManger, IConfiguration config,GymDbContext dbContext)
        {
            await IdentitySeeder.SeedAsync(userManager,roleManger,config);
            await PlanSeeder.SeedAsync(dbContext);
            await CategorySeeder.SeedAsync(dbContext);
        }
    }
}