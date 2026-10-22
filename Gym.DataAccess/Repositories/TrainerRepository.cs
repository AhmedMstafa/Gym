using Gym.DataAccess.Data.Contexts;
using Gym.DataAccess.Entities;

namespace Gym.DataAccess.Repositories;

public class TrainerRepository(GymDbContext dbContext) : Repository<Trainer>(dbContext), ITrainerRepository
{
}
