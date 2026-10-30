using Gym.Business.Common;
using Gym.Business.Mapping;
using Gym.Business.Services;
using Gym.Business.Settings;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gym.Business.DependencyInjection;

public static class BusinessServiceCollectionExtensions
{
    public static IServiceCollection AddGymBusinessLogic(this IServiceCollection services, IConfiguration? configuration = null)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(MappingConfig).Assembly);
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        if (configuration != null)
        {
            services.Configure<CloudinarySettings>(configuration.GetSection(CloudinarySettings.SectionName));
        }

        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<IMemberServices, MemberService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IHomeService, HomeService>();
        services.AddScoped<IMembershipService, MembershipService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<ITrainerService, TrainerService>();

        return services;
    }
}
