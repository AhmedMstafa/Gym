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

        public async Task<IReadOnlyList<Booking>> GetBySessionIdAsync(int sessionId, CancellationToken cancellationToken = default)
            => await _DbContext.Bookings.AsNoTracking().Include(x => x.Member)
                .Where(x => x.SessionId == sessionId).OrderBy(x => x.Member.Name).ToListAsync(cancellationToken);

        public Task<Booking?> GetByMemberAndSessionAsync(int memberId, int sessionId, CancellationToken cancellationToken = default)
            => _DbContext.Bookings.FirstOrDefaultAsync(x => x.MemberId == memberId && x.SessionId == sessionId, cancellationToken);

        public Task<int> CountBySessionIdAsync(int sessionId, CancellationToken cancellationToken = default)
            => _DbContext.Bookings.CountAsync(x => x.SessionId == sessionId, cancellationToken);
    }
}
