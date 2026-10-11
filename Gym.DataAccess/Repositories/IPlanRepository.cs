using Gym.DataAccess.Entities;

namespace Gym.DataAccess.Repositories
{
    public interface IPlanRepository  
    {

        Task<IEnumerable<Plan>> GetAllAsync();


        Task<Plan?> GetPlanByIdAsync(int planId);
       
        void Add(Plan plan);

        void Update(Plan plan);

        void Delete(Plan plan);

        Task<int> SaveChangesAsync();
    }
}
