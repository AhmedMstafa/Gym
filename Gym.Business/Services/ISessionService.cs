using Gym.Business.Common;
using Gym.Business.ViewModels;
using Gym.Business.ViewModels.Sessions;

namespace Gym.Business.Services;

public interface ISessionService
{
    Task<IReadOnlyList<SessionIndexViewModel>> GetIndexItemsAsync(CancellationToken cancellationToken = default);
    Task<Result<SessionDetailsViewModel>> GetDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<Result> CreateAsync(CreateSessionViewModel model, CancellationToken cancellationToken = default);
    Task<Result<EditSessionViewModel>> GetForUpdateAsync(int id, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(int id, EditSessionViewModel model, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CategoryViewModel>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrainerViewModel>> GetTrainersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TrainerViewModel>> GetTrainersByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
}
