using Gym.DataAccess.Data.Contexts;
using Gym.DataAccess.Interceptors;
using Gym.DataAccess.Data.Seeder;
using Gym.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IPlanRepository, PlanRepository>();

builder.Services.AddSingleton<AuditEntitiesInterceptor>();

builder.Services.AddDbContext<GymDbContext>((sp,options) =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.AddInterceptors(sp.GetRequiredService<AuditEntitiesInterceptor>());
});

var app = builder.Build();


app.UseRouting();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

var scope = app.Services.CreateScope();
await using var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();

await DatabaseSeeder.SeedAllAsync(dbContext);

app.Run();
