using Gym.DataAccess.Data.Contexts;
using Gym.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Gym.DataAccess.Repositories
{
    public class PlanRepository : IPlanRepository
        {

        public GymDbContext dbContext = new GymDbContext();
        public async Task<IEnumerable<Plan>> GetAllAsync()
            => await dbContext.Plans.ToListAsync();

        public async Task<Plan?>GetPlanByIdAsync(int planId)
            => await dbContext.Plans.FirstOrDefaultAsync(p => p.Id == planId);

        public   void Add(Plan plan)
            =>  dbContext.Plans.Add(plan);

        public  void Update(Plan plan)
            =>  dbContext.Update(plan);
        
        public  void Delete(Plan plan)
            =>  dbContext.Plans.Remove(plan);

        public async Task<int> SaveChangesAsync()
         => await dbContext.SaveChangesAsync();
    }
}
