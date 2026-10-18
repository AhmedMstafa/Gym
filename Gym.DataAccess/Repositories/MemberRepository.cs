using Gym.DataAccess.Data.Contexts;
using Gym.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gym.DataAccess.Repositories
{
    public class MemberRepository(GymDbContext GymDbContext) : Repository<Member>(GymDbContext), IMemberRepository
    {
        private readonly GymDbContext _DbContext = GymDbContext;

        public Task<Member?> GetWithMembershipAsync(int id, CancellationToken cancellationToken = default)
        {
            return _DbContext.Set<Member>()
                .AsNoTracking()
                .Include(m => m.MemberShips)
                .ThenInclude(ms => ms.Plan)
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<bool> HasUpcommingActiveSessionsAsync(int id, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            return await _DbContext.Set<Member>().Where(m => m.Id == id).SelectMany(m => m.Bookings).AnyAsync(b => b.Session.EndDate >= now, cancellationToken);
        }

        public async Task<bool> IsEmailTakenAsync(string normalizedEmail, int? excludeId = null, CancellationToken cancellationToken = default)
        => await _DbContext.Set<Member>().AnyAsync(m => m.Email == normalizedEmail && (!excludeId.HasValue || m.Id != excludeId.Value), cancellationToken);

        public async Task<bool> IsPhoneTakenAsync(string phone, int? excludeId = null, CancellationToken cancellationToken = default)
        => await _DbContext.Set<Member>().AnyAsync(m => m.Phone == phone && (!excludeId.HasValue || m.Id != excludeId.Value), cancellationToken);
    }
}
