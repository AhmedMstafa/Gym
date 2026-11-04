using Gym.DataAccess.Data.Contexts;
using Gym.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gym.DataAccess.Repositories;

public sealed class MembershipRepository(GymDbContext dbContext)
    : Repository<MemberShip>(dbContext), IMembershipRepository
{
    public async Task<IReadOnlyList<MemberShip>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        => await dbContext.MemberShips.AsNoTracking()
            .Include(x => x.Member)
            .Include(x => x.Plan)
            .ToListAsync(cancellationToken);

    public Task<MemberShip?> GetActiveByMemberIdAsync(int memberId, DateTime today, CancellationToken cancellationToken = default)
        => dbContext.MemberShips
            .Where(x => x.MemberId == memberId && x.EndDate >= today)
            .OrderByDescending(x => x.EndDate)
            .FirstOrDefaultAsync(cancellationToken);
}
