using Gym.DataAccess.Data.Contexts;
using Gym.DataAccess.Data.Seeder;
using Gym.DataAccess.DependencyInjection;
using Gym.Business.DependencyInjection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string not found.");
builder.Services.AddGymDataAccess(connectionString);
builder.Services.AddGymBusinessLogic();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRouting();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
    await dbContext.Database.MigrateAsync();
    await DatabaseSeeder.SeedAllAsync(dbContext);
}

app.Run();