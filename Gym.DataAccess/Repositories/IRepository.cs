using Gym.DataAccess.Entities;
using System.Linq.Expressions;

namespace Gym.DataAccess.Repositories
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task <IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        Task <TEntity?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default,
            bool trackChanges = true,
            Expression<Func<TEntity, bool>>? predicate = null,
            params Expression<Func<TEntity, object>>[] includes);
        
        Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        Task <TEntity?> GetByIdIncludingDeletedAsync(int id, CancellationToken cancellationToken = default);
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task SoftDeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
