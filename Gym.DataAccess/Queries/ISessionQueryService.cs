namespace Gym.DataAccess.Queries
{
    public interface ISessionQueryService
    {
        Task<IReadOnlyList<SessionIndexQueryService>> GetIndexItemsAsync(CancellationToken cancellationToken);
    }
}
