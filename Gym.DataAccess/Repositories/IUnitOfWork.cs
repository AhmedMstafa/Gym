using Gym.DataAccess.Entities;

namespace Gym.DataAccess.Repositories;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IMemberRepository Members { get; }
    IPlanRepository Plans { get; }
    IBookingRepository Bookings { get; }
    IMembershipRepository Memberships { get; }
    ITrainerRepository Trainers { get; }
    ISessionRepository Sessions { get; }
    IRepository<HealthRecord> HealthRecords { get; }
    IRepository<Category> Categories { get; }

    IRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
