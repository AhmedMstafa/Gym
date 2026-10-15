using Gym.Business.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Gym.Business.DependencyInjection;

public static class BusinessServiceCollectionExtensions
{
    public static IServiceCollection AddGymBusinessLogic(this IServiceCollection services)
    {
        services.AddScoped<IMemberServices, MemberService>();
        return services;
    }
}