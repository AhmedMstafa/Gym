using Gym.DataAccess.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Gym.DataAccess.Data.Seeder
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration config)
        {
            await EnsureRoleExists(roleManager, IdentityRoleNames.Admin, "Admin");
            await EnsureRoleExists(roleManager, IdentityRoleNames.SuperAdmin, "SuperAdmin");

            var SuperAdminEmail = config["IdentitySeed:SuperAdminEmail"] ?? throw new InvalidOperationException("Failed to get SuperAdmin email");
            var superAdminPassword = config["IdentitySeed:SuperAdminPassword"] ?? throw new InvalidOperationException("Failed to get SuperAdmin password");

            var AdminEmail = config["IdentitySeed:AdminEmail"] ?? throw new InvalidOperationException("Failed to get Admin email");
            var AdminPassword = config["IdentitySeed:AdminPassword"] ?? throw new InvalidOperationException("Failed to get Admin password");

            ApplicationUser? superAdmin = await EnsureUserCreated(userManager, SuperAdminEmail, superAdminPassword);
            ApplicationUser? Admin = await EnsureUserCreated(userManager, AdminEmail, AdminPassword);

            if (superAdmin != null)
                await AssignUserToRole(userManager, superAdmin, IdentityRoleNames.SuperAdmin);

            if (Admin != null)
                await AssignUserToRole(userManager, Admin, IdentityRoleNames.Admin);
        }

        private static async Task AssignUserToRole(UserManager<ApplicationUser> userManager, ApplicationUser user, string roleName)
        {
            if (!await userManager.IsInRoleAsync(user, roleName))
            {
                await userManager.AddToRoleAsync(user, roleName);
            }
        }

        private static async Task<ApplicationUser?> EnsureUserCreated(UserManager<ApplicationUser> userManager, string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = "Admin",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    user = await userManager.FindByEmailAsync(email);
                }
            }

            return user;
        }

        private static async Task EnsureRoleExists(RoleManager<ApplicationRole> roleManager, string roleName, string displayName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = roleName,
                    DisplayName = displayName,
                });
            }
        }
    }
}