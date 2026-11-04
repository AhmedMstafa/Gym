using System.Collections.Concurrent;
using Gym.DataAccess.Data.Contexts;
using Gym.DataAccess.Entities;

namespace Gym.DataAccess.Repositories;

public class UnitOfWork(GymDbContext dbContext) : IUnitOfWork
{
    private readonly GymDbContext _dbContext = dbContext;
    private readonly ConcurrentDictionary<Type, object> _repositories = new();

    private IMemberRepository? _members;
    private IPlanRepository? _plans;
    private IBookingRepository? _bookings;
    private IMembershipRepository? _memberships;
    private ITrainerRepository? _trainers;
    private ISessionRepository? _sessions;
    private IRepository<HealthRecord>? _healthRecords;
    private IRepository<Category>? _categories;

    public IMemberRepository Members => _members ??= new MemberRepository(_dbContext);
    public IPlanRepository Plans => _plans ??= new PlanRepository(_dbContext);
    public IBookingRepository Bookings => _bookings ??= new BookingRepository(_dbContext);
    public IMembershipRepository Memberships => _memberships ??= new MembershipRepository(_dbContext);
    public ITrainerRepository Trainers => _trainers ??= new TrainerRepository(_dbContext);
    public ISessionRepository Sessions => _sessions ??= new SessionRepository(_dbContext);
    public IRepository<HealthRecord> HealthRecords => _healthRecords ??= new Repository<HealthRecord>(_dbContext);
    public IRepository<Category> Categories => _categories ??= new Repository<Category>(_dbContext);

    public IRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
    {
        return (IRepository<TEntity>)_repositories.GetOrAdd(
            typeof(TEntity),
            _ => new Repository<TEntity>(_dbContext));
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }
}
