namespace Gym.DataAccess.Data.Seeder
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAllAsync()
        {
            await PlanSeeder.SeedAsync();
        }
    }
}