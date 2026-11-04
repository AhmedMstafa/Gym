using Gym.DataAccess.Data.Contexts;
using Gym.DataAccess.Data.Seeder;
using Gym.DataAccess.DependencyInjection;
using Gym.Business.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Gym.DataAccess.Entities.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

LoadDotEnv(builder.Environment.ContentRootPath);
builder.Configuration.AddEnvironmentVariables();

//builder.Services.AddControllersWithViews(options =>
//{
//    var policy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
//        .RequireAuthenticatedUser()
//        .Build();

//    options.Filters.Add(new AuthorizeFilter(policy));
//});

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string not found.");
builder.Services.AddGymDataAccess(connectionString);
builder.Services.AddGymBusinessLogic(builder.Configuration);
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(conf => {
    conf.Password.RequireDigit = true;
    conf.Password.RequiredLength = 8;
    conf.User.RequireUniqueEmail = true;
    conf.SignIn.RequireConfirmedEmail = true;

    conf.Lockout.MaxFailedAccessAttempts = 5;

    conf.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(60);
})
    .AddEntityFrameworkStores<GymDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{

    options.ExpireTimeSpan = TimeSpan.FromHours(10);

    options.SlidingExpiration = true;

});

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
app.UseAuthentication();
app.UseAuthorization();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateAsyncScope())
{
    var startupLogger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

    startupLogger.LogInformation("Applaying database migrations");

    startupLogger.LogInformation("Seeding database");
    await dbContext.Database.MigrateAsync();


    await DatabaseSeeder.SeedAllAsync(userManager,roleManager,app.Configuration,dbContext);
    startupLogger.LogInformation("Database ready");
}

app.Run();

static void LoadDotEnv(string contentRootPath)
{
    var filePath = Path.Combine(contentRootPath, ".env");

    if (!File.Exists(filePath))
        return;

    foreach (var line in File.ReadLines(filePath))
    {
        var trimmedLine = line.Trim();

        if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith('#'))
            continue;

        var separatorIndex = trimmedLine.IndexOf('=');

        if (separatorIndex <= 0)
            continue;

        var key = trimmedLine[..separatorIndex].Trim();
        var value = trimmedLine[(separatorIndex + 1)..].Trim().Trim('"', '\'');
        Environment.SetEnvironmentVariable(key, value);
    }
}
