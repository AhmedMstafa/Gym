using Gym.DataAccess.Data.Contexts;
using Gym.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gym.DataAccess.Repositories
{
    public class BookingRepository(GymDbContext dbContext) : Repository<Booking>(dbContext), IBookingRepository
    {
        private readonly GymDbContext _DbContext = dbContext;

        public async Task<bool> HasUpcommingActiveSessionsAsync(int memberId, DateTime utcNow, CancellationToken cancellationToken = default)
        {
            return await _DbContext.Set<Booking>().AnyAsync(b => b.MemberId == memberId && b.Session.EndDate >= utcNow, cancellationToken);
        }
    }
}
