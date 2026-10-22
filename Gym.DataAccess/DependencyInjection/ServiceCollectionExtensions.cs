using Gym.DataAccess.Data.Contexts;
using Gym.DataAccess.Interceptors;
using Gym.DataAccess.Queries;
using Gym.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Gym.DataAccess.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGymDataAccess(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<AuditEntitiesInterceptor>();

        services.AddDbContext<GymDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString)
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
            options.AddInterceptors(sp.GetRequiredService<AuditEntitiesInterceptor>());
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IPlanRepository, PlanRepository>();

        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<ITrainerRepository, TrainerRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ISessionQueryService, SessionQueryService>();

        return services;
    }
}