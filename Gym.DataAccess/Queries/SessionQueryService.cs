using Gym.DataAccess.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Gym.DataAccess.Queries
{
    public class SessionQueryService(GymDbContext dbContext) : ISessionQueryService
    {
        async Task<IReadOnlyList<SessionIndexQueryService>> ISessionQueryService.GetIndexItemsAsync(CancellationToken cancellationToken)
        {
            return await dbContext.Sessions
                .AsNoTracking()
                .OrderBy(s => s.StartDate)
                .Select(s => new SessionIndexQueryService
                {
                    Id = s.Id,
                    Speciality = s.Category.Name,
                    Description = s.Description,
                    TrainerName = s.Trainer.Name,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    Capacity = s.Capacity,
                    BookedCount = s.Bookings.Count,
                }).ToListAsync();
        }
    }
}
