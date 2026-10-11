using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Gym.DataAccess.Entities;

namespace Gym.DataAccess.Interceptors
{
    public class AuditEntitiesInterceptor : SaveChangesInterceptor
    {
        private void UpdateAuditProperties(DbContext? context)
        {
            if (context == null)
                return;

            var now = DateTime.UtcNow;

            foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.IsDeleted = false;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                }

                // Soft-delete support: when an entity is deleted, convert to a soft delete
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = now;
                }
            }
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateAuditProperties(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateAuditProperties(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
