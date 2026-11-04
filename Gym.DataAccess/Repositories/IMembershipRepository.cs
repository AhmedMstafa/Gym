using Gym.DataAccess.Entities;

namespace Gym.DataAccess.Repositories;

public interface IMembershipRepository : IRepository<MemberShip>
{
    Task<IReadOnlyList<MemberShip>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<MemberShip?> GetActiveByMemberIdAsync(int memberId, DateTime today, CancellationToken cancellationToken = default);
}
