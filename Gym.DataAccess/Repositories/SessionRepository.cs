using Gym.DataAccess.Data.Contexts;
using Gym.DataAccess.Entities;

namespace Gym.DataAccess.Repositories;

public class SessionRepository(GymDbContext dbContext) : Repository<Session>(dbContext), ISessionRepository
{
}
