using Gym.Business.Common;
using Gym.Business.Mapping;
using Gym.Business.Services;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Gym.Business.DependencyInjection;

public static class BusinessServiceCollectionExtensions
{
    public static IServiceCollection AddGymBusinessLogic(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(MappingConfig).Assembly);
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IMemberServices, MemberService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IHomeService, HomeService>();

        return services;
    }
}