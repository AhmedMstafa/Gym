using Gym.DataAccess.Entities;

namespace Gym.DataAccess.Repositories;

public interface ITrainerRepository : IRepository<Trainer>
{
    Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> IsPhoneTakenAsync(string phone, CancellationToken cancellationToken = default);
}
