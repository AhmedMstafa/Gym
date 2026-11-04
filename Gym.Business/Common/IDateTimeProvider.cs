namespace Gym.Business.Common
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
        DateTime Now { get; }
        DateOnly Today { get; }
        DateTime DateTimeToday { get; }
    }
}
