using Gym.DataAccess.Data.Contexts;
using Gym.DataAccess.Entities;

namespace Gym.DataAccess.Repositories
{
    public class PlanRepository(GymDbContext dbContext) : Repository<Plan>(dbContext), IPlanRepository
    {

    }
}
