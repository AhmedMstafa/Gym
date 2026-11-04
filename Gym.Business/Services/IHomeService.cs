using Gym.Business.ViewModels.Home;

namespace Gym.Business.Services;

public interface IHomeService
{
    Task<HomeStatisticsViewModel> GetStatisticsAsync(CancellationToken cancellationToken = default);
}
