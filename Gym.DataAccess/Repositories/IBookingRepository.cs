using Gym.DataAccess.Entities;

namespace Gym.DataAccess.Repositories
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<bool> HasUpcommingActiveSessionsAsync(int memberId, DateTime utcNow, CancellationToken cancellationToken = default);
    }
}
