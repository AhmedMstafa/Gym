using Gym.DataAccess.Data.Contexts;
using Gym.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gym.DataAccess.Repositories
{
    public class Repository<TEntity>(GymDbContext dbContext) : IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly GymDbContext _dbContext = dbContext;

        private readonly DbSet<TEntity> _dbset = dbContext.Set<TEntity>();

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        =>await _dbset.AsNoTracking().ToListAsync(cancellationToken);
        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbset.AnyAsync(predicate, cancellationToken);

        public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbset.FirstOrDefaultAsync(predicate,cancellationToken);


        public async Task<TEntity?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default,
            bool trackChanges = true,
            Expression<Func<TEntity, bool>>? predicate = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var query = ApplayIncludes(_dbset.AsQueryable(), includes);
            if (!trackChanges)
                 query =  query.AsNoTracking();

            if (predicate != null)
                query = query.Where(predicate);

            return await query.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<TEntity?> GetByIdIncludingDeletedAsync(int id, CancellationToken cancellationToken = default)
        => await _dbset.IgnoreQueryFilters().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);


        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        => await _dbset.AddAsync(entity, cancellationToken);

        public  Task SoftDeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            entity.IsDeleted = true;
             _dbset.Update(entity);

            return Task.CompletedTask;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        => await UpdateAsync(entity, cancellationToken);

      public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);

        private static IQueryable<TEntity> ApplayIncludes(IQueryable<TEntity> query, IEnumerable<Expression<Func<TEntity, object>>>? includes)
        {
            if (includes is null) return query;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query;
        }
    }
}
