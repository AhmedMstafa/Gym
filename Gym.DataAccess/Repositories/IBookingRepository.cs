using Gym.DataAccess.Entities;

namespace Gym.DataAccess.Repositories
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<bool> HasUpcommingActiveSessionsAsync(int memberId, DateTime utcNow, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Booking>> GetBySessionIdAsync(int sessionId, CancellationToken cancellationToken = default);
        Task<Booking?> GetByMemberAndSessionAsync(int memberId, int sessionId, CancellationToken cancellationToken = default);
        Task<int> CountBySessionIdAsync(int sessionId, CancellationToken cancellationToken = default);
    }
}
