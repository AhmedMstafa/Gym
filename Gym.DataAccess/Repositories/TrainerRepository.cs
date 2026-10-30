using Gym.DataAccess.Data.Contexts;
using Gym.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gym.DataAccess.Repositories;

public class TrainerRepository(GymDbContext dbContext) : Repository<Trainer>(dbContext), ITrainerRepository
{
    public Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellationToken = default)
        => dbContext.Users.AnyAsync(x => x.Email == email, cancellationToken);

    public Task<bool> IsPhoneTakenAsync(string phone, CancellationToken cancellationToken = default)
        => dbContext.Users.AnyAsync(x => x.Phone == phone, cancellationToken);
}
