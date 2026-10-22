using Gym.Business.ViewModels.Sessions;

namespace Gym.Business.Services;

public interface ISessionService
{
    Task<IReadOnlyList<SessionIndexViewModel>> GetIndexItemsAsync(CancellationToken cancellationToken = default);
}
